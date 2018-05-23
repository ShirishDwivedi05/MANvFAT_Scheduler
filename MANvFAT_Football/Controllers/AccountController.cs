using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models;
using MANvFAT_Football.Models.Enumerations;
using MANvFAT_Football.Models.Repositories;
using System;
using System.Linq;
using System.Web.Mvc;
using System.Web.Security;

namespace MANvFAT_Football.Controllers
{
    public class AccountController : BaseController
    {
        #region Logon

        public ActionResult LogOn()
        {
            RemoveUserfromSession();

            Response.StatusCode = 200;

            return View();
        }

        public void RemoveUserfromSession()
        {
            FormsAuthentication.SignOut();
            Session.RemoveAll();
        }

        //
        // POST: /Account/LogOn

        [HttpPost]
        public ActionResult LogOn(LogOnModel model, string returnUrl, FormCollection formcollection)
        {
            RemoveUserfromSession();

            if (returnUrl != null)
                if (returnUrl.Contains("%2f"))
                    returnUrl = Server.UrlDecode(returnUrl);
            bool locked = false, IsMobileNumVerified = false;

            if (ModelState.IsValid)
            {
                if (ValidateUser(model.UserName, model.Password, ref locked, ref IsMobileNumVerified))
                {
                    //Disable Mobile Verification On TEST and Development from System Settings
                    SystemSettingsRepository systemSettingsRepo = new SystemSettingsRepository();
                    var Enable_MobileVerification = systemSettingsRepo.GetSystemSettings().Enable_MobileVerification;
                   
                    if (Enable_MobileVerification == false)
                    {
                        //If Mobile Verification is Disabled, then Don't redirect to Mobile Verification Page and force the system to assume that Mobile is Verified
                        IsMobileNumVerified = true;
                    }


                    if (!locked && IsMobileNumVerified)
                    {
                        FormsAuthentication.SetAuthCookie(model.UserName, model.RememberMe);

                        if ((long)Session["RoleID"] == (long)Permissions.LeagueViewer)
                        {
                            return RedirectToAction("LocalAuthLeagues", "Leagues");
                        }
                        else
                        {
                            if (returnUrl != null)
                            {
                                return Redirect(returnUrl);
                            }
                            else
                            {
                                return RedirectToAction("Index", "Admin");
                            }
                        }
                    }
                    else if (IsMobileNumVerified == false)
                    {
                        return RedirectToAction("MobileVerification", "Account");
                    }
                    else
                    {
                        ModelState.AddModelError("", "You cannot log on the System, because your status is Locked. Please contact site Administrator.");
                    }
                }
                else
                {
                    ModelState.AddModelError("", "The user name or password provided is incorrect.");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        //*********The Following Method can be used if Authentication take place from custom users Table***********//

        public Boolean ValidateUser(string Username, string password, ref bool locked, ref bool IsMobileNumVerified)
        {
            Boolean status = false;
            string Decrypted_password = SecurityUtils.EncryptText(password);
            Users users;

            using (DBEntities db = new DBEntities())
            {
                //TODO: Uncomment the Following Line and remove ->  = null;
                users = db.Users.FirstOrDefault(u => u.EmailAddress == Username && u.Password == Decrypted_password && u.Deleted != true);

                if (users != null)
                {
                    locked = users.Locked;

                    //Get the Cookie for Mobile Number Verification, if cookie is created then Proceed without Mobile Verification, otherwise user must verify their mobile phone by using 2 Factor Authentication
                    CookiesRespository cookiesRespo = new CookiesRespository();
                    var modelCookie = cookiesRespo.GetMobileNumVerification_Cookie(this);

                    if (users.IsMobileNumVerified && !string.IsNullOrEmpty(modelCookie.EmailAddress_MobVerification) 
                        && modelCookie.EmailAddress_MobVerification.ToLower().Equals(users.EmailAddress.ToLower()))
                    {
                        IsMobileNumVerified = true;
                    }
                    else
                    {
                        IsMobileNumVerified = false;

                        SecurityUtils.AddAuditLog("Log On", "User Log On but Mobile Verification Required: " + users.EmailAddress, users.UserID);
                        Session["UserID"] = users.UserID;
                        Session["RoleID"] = users.RoleID;
                    }

                    if (!locked && IsMobileNumVerified)
                    {
                        Session["UserID"] = users.UserID;
                        Session["RoleID"] = users.RoleID;

                        SecurityUtils.AddAuditLog("Log On", "Log On: " + users.EmailAddress, users.UserID);
                    }

                    status = true;
                }
                else
                    status = false;
            }
            return status;
        }

        #endregion Logon

        #region Mobile Number Verification

        public ActionResult MobileVerification()
        {
            try
            {
                long UserID = Convert.ToInt64(Session["UserID"]);
                UsersRepository modelRepo = new UsersRepository();
                var model = modelRepo.ReadOne(UserID);
                bool IsMobileNumExists = true;
                Random rndm = new Random();
                int MobileVerificationCode = rndm.Next(111111, 999999);
                string ErrorMsg = "";
                //Save and Send this Verification Code To User's Mobile Number if Mobile Number Exists
                if (modelRepo.ValidateMobileNumber(model.Mobile, ref ErrorMsg))
                {
                    //Save the Code for User
                    modelRepo.UpdateUserMobileVerificationCode(model.UserID, MobileVerificationCode);

                    //Send the Code to user's Mobile Number
                    EmailsRepository emailRepo = new EmailsRepository();
                    //TODO: Enable for Test or Live Site
                    emailRepo.SendEmail("football@manvfat.com", model.Mobile + "@textmagic.com", "", MobileVerificationCode.ToString() + " is your MANvFAT verification code.");
                }
                else
                {
                    //If We didn't have Mobile number for this user then display the message, and Only Valid Admin users can update their Mobile Number under Maintenance=>Users
                    IsMobileNumExists = false; ;
                }

                //will be used to Return to function to verify the Mobile Verification Code entered by User
                ViewBag.UserEmailAddress = model.EmailAddress;
                ViewBag.UserMobilePhoneEndsWith = !string.IsNullOrEmpty(model.Mobile) && model.Mobile.Length > 3 ? "07** **** " + model.Mobile.Substring(model.Mobile.Length - 3) : "";
                ViewBag.IsMobileNumExists = IsMobileNumExists;
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);

                return RedirectToAction("Logon", "Account");
            }
            return View();
        }

        public ActionResult VerifyMobileCode(string UserEmailAddress, int VerificationCode, bool RememberMe)
        {
            bool _status = false;
            string _returnUrl = "", _ErrorMsg = "";
            UsersRepository modelRepo = new UsersRepository();
            var model = modelRepo.ReadOne(UserEmailAddress);

            if (model.MobileVerficationCode == VerificationCode)
            {
                _status = true;
                modelRepo.UpdateUserMobileVerification(model.UserID, true);

                if (RememberMe)
                {
                    //Create Cookie and Remember User
                    CookiesRespository cookiesRepo = new CookiesRespository();
                    var httpcookie = cookiesRepo.CreateMobileNumVerrification_Cookie(model.EmailAddress);

                    Response.SetCookie(httpcookie);
                }

                FormsAuthentication.SetAuthCookie(model.EmailAddress, RememberMe);

                SecurityUtils.AddAuditLog("Log On With Mobile Code", "User " + UserEmailAddress + " Logged in. VerificationCode = " + VerificationCode + " RememberMe = " + RememberMe, this);

                if (model.RoleID == (long)Permissions.LeagueViewer)
                {
                    _returnUrl = "/Leagues/LocalAuthLeagues";
                }
                else
                {
                    _returnUrl = "/Admin/Index";
                }
            }
            else
            {
                _ErrorMsg = "Wrong Verification code, Please try again or contact us at football@manvfat.com";
            }

            return Json(new { status = _status, ErrorMsg = _ErrorMsg, returnUrl = _returnUrl }, JsonRequestBehavior.AllowGet);
        }

        #endregion Mobile Number Verification

        #region LogOff

        //
        // GET: /Account/LogOff

        public ActionResult LogOff()
        {
            SecurityUtils.AddAuditLog("Log Off", "Log Off", this);
            FormsAuthentication.SignOut();

            Session.RemoveAll();
            return RedirectToAction("LogOn", "Account");
        }

        #endregion LogOff

        #region Forgotten & Reset Password

        public ActionResult ForgottenPassword()
        {
            ViewBag.status = "";
            return View();
        }

        [HttpPost]
        public ActionResult ForgottenPassword(ForgottenPasswordModel model)
        {
            bool status = true;
            string exMsg = "";
            if (ModelState.IsValid)
            {
                SystemSettingsRepository sysRepo = new SystemSettingsRepository();
                var sys = sysRepo.GetSystemSettings();
                using (DBEntities db = new DBEntities())
                {
                    Users user = db.Users.FirstOrDefault(u => u.EmailAddress == model.Email);
                    if (user != null)
                    {
                        //Now Send an Email to User for Username and Password!
                        if (sys.EmailsEnabled)
                        {
                            try
                            {
                                //Random rndm = new Random();
                                //var RandomNum = rndm.Next(10001, int.MaxValue);
                                Guid guid = Guid.NewGuid();
                                string EncryptedRandomNum = guid.ToString();
                                SecurityUtils.CheckforInvalidFileNameChar(ref EncryptedRandomNum); //it will remove any unsupported characters
                                user.PasswordResetCode = EncryptedRandomNum;
                                user.ResetCodeExpiry = DateTime.Now.AddHours(2);
                                db.SaveChanges();

                                status = SendEmail(user, EncryptedRandomNum);
                            }
                            catch (Exception ex)
                            {
                                ViewBag.status = "Failed to send an Email. Please contact your Site Administrator" + ex.Message;
                                exMsg = ex.Message;
                            }

                            if (status)
                            {
                                //Add To Log
                                SecurityUtils.AddAuditLog("Requested for Forgotten Password", "User \"" + user.FullName + "\" requested for Forgotten Password , Email Sent to: \"" + user.EmailAddress + "\"", this);
                                ViewBag.status = "Your Login Details has been sent to above email address. <a style='color:black' href='/Account/Logon'> Log On </a>";
                            }
                            else
                            {
                                ViewBag.status = "Failed to send an Email. Please contact your Site Administrator." + exMsg + " - " + EmailsRepository.EmailErrorMsg;
                            }
                        }
                    }
                    else
                    {
                        ViewBag.status = "Your provided email address is not valid. Please contact site Administrator.";
                    }
                }
            }

            return View();
        }

        public bool SendEmail(Users user, string EncryptedRandomNum)
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();

            string ResetPasswordLink = sys.CurrentDomain + "/Account/ResetPassword?id=" + EncryptedRandomNum;
            string Body = "Dear " + user.FullName + "," +
                "<br/>" +
                "<br/> Please <a href='" + ResetPasswordLink + "'>Click here</a> to Reset your Password at MANvFAT Football";

            EmailsRepository emailRepo = new EmailsRepository();

            return emailRepo.SendEmail(SecurityUtils.SiteAdminEmail, user.EmailAddress, "Reset Password at MANvFAT Football", Body);
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            string errorMsg = "";
            bool status = true;
            long userId = 0;
            if (id != null)
            {
                using (DBEntities db = new DBEntities())
                {
                    Users user = db.Users.FirstOrDefault(u => u.PasswordResetCode == id);
                    if (user != null)
                    {
                        if (DateTime.Now < user.ResetCodeExpiry) //when Reset Code time didn't expired
                        {
                            userId = user.UserID;
                            status = true;
                        }
                        else
                        {
                            //When Reset Code Time Expired
                            status = false;
                            errorMsg = "Password Reset Code is Expired.";
                        }
                    }
                    else
                    {
                        status = false;
                        errorMsg = "Password Reset Code is not valid.";
                    }
                }
            }
            else
            {
                status = false;
                errorMsg = "Password Reset Code is not valid.";
            }

            ResetPasswordModel model = new ResetPasswordModel()
            {
                UserID = userId,
                status = status,
                Reason = errorMsg
            };

            return View(model);
        }

        [AllowAnonymous]
        [HttpPost]
        public ActionResult ResetPassword(ResetPasswordModel model)
        {
            if (ModelState.IsValid)
            {
                using (DBEntities db = new DBEntities())
                {
                    Users user = db.Users.FirstOrDefault(u => u.UserID == model.UserID);
                    if (user != null)
                    {
                        user.PasswordResetCode = null;
                        user.ResetCodeExpiry = null;
                        user.Password = SecurityUtils.EncryptText(model.ConfirmPassword);
                        db.SaveChanges();

                        return RedirectToAction("ResetPasswordSuccess");
                    }
                    else
                    {
                        ModelState.AddModelError("", "Password Reset Code is not valid.");
                    }
                }
            }

            return View(model);
        }

        public ActionResult ResetPasswordSuccess()
        {
            return View();
        }

        #endregion Forgotten & Reset Password

        #region Change Password

        ////
        //// GET: /Account/ChangePassword

        [Authorization(Permissions.AllUsers)]
        public ActionResult ChangePassword()
        {
            return View();
        }

        ////
        //// POST: /Account/ChangePassword

        [Authorization(Permissions.AllUsers)]
        [HttpPost]
        public ActionResult ChangePassword(ChangePasswordModel model)
        {
            if (ModelState.IsValid)
            {
                // ChangePassword will throw an exception rather
                // than return false in certain failure scenarios.
                bool changePasswordSucceeded;
                using (DBEntities db = new DBEntities())
                {
                    try
                    {
                        Users user = db.Users.Where(u => u.EmailAddress == User.Identity.Name).FirstOrDefault();

                        if (user != null)
                        {
                            string oldPassword = SecurityUtils.DecryptCypher(user.Password);
                            if (oldPassword.Equals(model.OldPassword))
                            {
                                user.Password = SecurityUtils.EncryptText(model.ConfirmPassword);
                                db.SaveChanges();
                                //Add To Log
                                SecurityUtils.AddAuditLog("User password Changed", "User password Changed Username: \"" + user.EmailAddress + "\"", this);
                                changePasswordSucceeded = true;
                            }
                            else
                            {
                                changePasswordSucceeded = false;
                            }
                        }
                        else
                        {
                            changePasswordSucceeded = false;
                        }

                        //MembershipUser currentUser = Membership.GetUser(User.Identity.Name, true /* userIsOnline */);
                        //changePasswordSucceeded = currentUser.ChangePassword(model.OldPassword, model.NewPassword);
                    }
                    catch (Exception)
                    {
                        changePasswordSucceeded = false;
                    }
                }

                if (changePasswordSucceeded)
                {
                    ViewBag.status = "Password Successfully Changed";
                }
                else
                {
                    ModelState.AddModelError("", "The current password is incorrect");
                }
            }

            // If we got this far, something failed, redisplay form
            return View(model);
        }

        ////
        //// GET: /Account/ChangePasswordSuccess

        public ActionResult ChangePasswordSuccess()
        {
            return View();
        }

        #endregion Change Password

        #region Status Codes

        private static string ErrorCodeToString(MembershipCreateStatus createStatus)
        {
            // See http://go.microsoft.com/fwlink/?LinkID=177550 for
            // a full list of status codes.
            switch (createStatus)
            {
                case MembershipCreateStatus.DuplicateUserName:
                    return "User name already exists. Please enter a different user name.";

                case MembershipCreateStatus.DuplicateEmail:
                    return "A user name for that e-mail address already exists. Please enter a different e-mail address.";

                case MembershipCreateStatus.InvalidPassword:
                    return "The password provided is invalid. Please enter a valid password value.";

                case MembershipCreateStatus.InvalidEmail:
                    return "The e-mail address provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidAnswer:
                    return "The password retrieval answer provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidQuestion:
                    return "The password retrieval question provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.InvalidUserName:
                    return "The user name provided is invalid. Please check the value and try again.";

                case MembershipCreateStatus.ProviderError:
                    return "The authentication provider returned an error. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                case MembershipCreateStatus.UserRejected:
                    return "The user creation request has been canceled. Please verify your entry and try again. If the problem persists, please contact your system administrator.";

                default:
                    return "An unknown error occurred. Please verify your entry and try again. If the problem persists, please contact your system administrator.";
            }
        }

        #endregion Status Codes

        protected override void OnException(ExceptionContext filterContext)
        { ErrorController.filterContext = filterContext; }
    }
}