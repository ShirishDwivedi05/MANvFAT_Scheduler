using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models;
using MANvFAT_Football.Models.Repositories;
using System;
using System.Web.Mvc;

namespace MANvFAT_Football.Controllers
{
    public class HomeController : BaseController
    {
        #region Front End and Registration

        public ActionResult Index(string id)
        {
            //return RedirectToAction("Registration");
            return View();
        }

        #region Registration & PayNow

        [HttpGet]
        public ActionResult Registration()
        {
            RegistrationViewModel vmb = new RegistrationViewModel();

            var model = vmb.NewRegistration(this, null);

            return View(model);
        }

        [HttpPost]
        public ActionResult Registration(RegistrationExt model)
        {
            //Redirect the User to Error/RestrictedEmailDomain if user enter any Restricted Email Domain while Registration
            string _Msg = "";
            bool _status = true;
            if (ModelState.IsValid)
            {

                if (SecurityUtils.IsRestrictedEmailDomainDetected(model.EmailAddress))
                {
                    SecurityUtils.AddAuditLog("Restricted Email Address Detected", "Restricted Email Address: " + model.EmailAddress);

                    return Json(new { status = false, Msg = "Restricted Email Address: " + model.EmailAddress }, JsonRequestBehavior.AllowGet);
                }

                PlayersExt player = MAP_Registration(model);
                PlayersRepository playerRepo = new PlayersRepository();
                playerRepo.CreateOrUpdate(ref player, ref _Msg, ref _status, this);


                return Json(new { status = _status, Msg = _Msg }, JsonRequestBehavior.AllowGet);
            }

            foreach (ModelState modelState in ViewData.ModelState.Values)
            {
                foreach (ModelError error in modelState.Errors)
                {
                    _Msg = _Msg+" " +error.ErrorMessage;
                }
            }

            return Json(new { status = false, Msg = _Msg }, JsonRequestBehavior.AllowGet);

        }

        public PlayersExt MAP_Registration(RegistrationExt RegExt)
        {
            PlayersExt model = new PlayersExt()
            {
                FirstName = RegExt.FirstName,
                LastName = RegExt.LastName,
                EmailAddress = RegExt.EmailAddress,
            };

            return model;
        }

        //When user click on Login Link from Home Page, then he can enter his Email Address and Password to login into Dashboard
        [HttpGet]
        public ActionResult Login()
        {
            PlayerDashboardLogin model = new PlayerDashboardLogin();
            ViewBag.ModelIsLogin = true;
            return View(model);
        }

        [HttpPost]
        public ActionResult Login(PlayerDashboardLogin model)
        {
            string _Reason = "";
            PlayerDashboardRepository playerDashboardRepo = new PlayerDashboardRepository();
            var playerDashboard = playerDashboardRepo.ValidatePlayerDashboard_HomeLogin(model.EmailAddress, model.Password, ref _Reason);

            if(playerDashboard!=null)
            {
                CookiesRespository cookieRepo = new CookiesRespository();
                var httpcookie = cookieRepo.CreateDashboardLoginCookie(playerDashboard.PlayerEmailAddress, model.IsRememberFor2Weeks);

                Response.SetCookie(httpcookie);

                System.Web.Routing.RouteValueDictionary rt = new System.Web.Routing.RouteValueDictionary();
                rt.Add("id", playerDashboard.DashboardURL);

                SecurityUtils.AddAuditLog("Premium Dashboard Member", "Log In - Dashboard URL Id = " + playerDashboard.DashboardURL, this);

                return RedirectToAction("Index","Member", rt);
            }
            else
            {
                ViewBag.ModelIsLogin = true;
                model.Reason = "Unable to login, please correct your Username/Password, OR contact us at progress@manvfat.com";
                return View(model);
            }
        }

        #endregion Registration & PayNow

        public ActionResult About()
        {
            //ViewBag.Is64BitProcess = Environment.Is64BitProcess;
            //ViewBag.Is64BitOperatingSystem = Environment.Is64BitOperatingSystem;

            return View();
        }
        [HttpGet]
        public ActionResult NewIndex()
        {
            RegistrationViewModel vmb = new RegistrationViewModel();

            var model = vmb.NewRegistration(this, null);

            return View(model);

    
        }

        [HttpPost]
        public ActionResult NewIndex(RegistrationExt model)
        {

            //if (SecurityUtils.IsRestrictedEmailDomainDetected(model.EmailAddress))
            //{
            //    SecurityUtils.AddAuditLog("Restricted Email Address Detected", "Restricted Email Address: " + model.EmailAddress);

            //    return RedirectToAction("RestrictedEmailDomain", "Error");
            //}
            PlayersExt pModel = MAP_Registration(model);
            string Msg = "";
            bool status = true;
            ModelState.Remove("id");
            pModel.Notes = "";
            PlayersRepository modelRepo = new PlayersRepository();

            pModel.PlayerID = modelRepo.CreateOrUpdate(ref pModel, ref Msg, ref status, this, false, 0);
            if(pModel.PlayerID>0)
            {
                ViewBag.RegSuccess = true;
            }
            else
            {
                ViewBag.RegSuccess = false;
            }
            return View();
        }
        public ActionResult Contact(FormCollection formcollection)
        {
            string txtContactName = formcollection["txtContactName"].ToString();
            string txtContactEmail = formcollection["txtContactEmail"].ToString();
            string txtContactMsg = formcollection["txtContactMsg"].ToString();

            SecurityUtils.AddAuditLog("Contact Us", "Contact us email Received: Name: " + txtContactName + " Email: " + txtContactEmail);

            EmailsRepository emailRepo = new EmailsRepository();
            emailRepo.ContactEmail(txtContactName, txtContactEmail, txtContactMsg, this);

            return View();
        }

        #endregion Front End and Registration

        #region Footer Subscription

        public ActionResult Subscribe_FooterSignup(string EmailAddress)
        {
            MailChimpRepository mailChimpRepo = new MailChimpRepository();
            bool _status = true;// mailChimpRepo.Subscribe_SingleSignUp(EmailAddress);

            return Json(new { status = _status }, JsonRequestBehavior.AllowGet);
        }

        #endregion Footer Subscription

        #region Email Testing Functions

        //public ActionResult TestEmail()
        //{
        //    return View();
        //}

        //[HttpPost]
        //public ActionResult TestEmail(FormCollection formcollection)
        //{
        //    string To = formcollection["txtTo"].ToString();
        //    string Subject = formcollection["txtSubject"].ToString();
        //    string Body = formcollection["txtEmailBody"].ToString();

        //    SecurityUtils.AddAuditLog("Attempt to send Email", "Attempt to send Email using Test Function");

        //    EmailsRepository emailRepo = new EmailsRepository();
        //    if (emailRepo.SendEmail(SecurityUtils.SiteAdminEmail, To, Subject, Body))
        //    {
        //        ViewBag.Status = "Email Sent Success";
        //        SecurityUtils.AddAuditLog("Success to send Email", "Attempt to send Email using Test Function Success from " + SecurityUtils.SiteAdminEmail + " to : " + To);
        //    }
        //    else
        //    {
        //        SecurityUtils.AddAuditLog("Email-SMS Sent Failed", "\" Exception Error Message:  \"" + EmailsRepository.EmailErrorMsg + "\"");
        //        ViewBag.Status = "Email Sent Failed \"" + EmailsRepository.EmailErrorMsg + "\" ";
        //    }

        //    return View();
        //}

        #endregion Email Testing Functions
    }
}