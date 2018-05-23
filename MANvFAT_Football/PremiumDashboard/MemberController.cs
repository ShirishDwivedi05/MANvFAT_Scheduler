using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models;
using MANvFAT_Football.Models.Enumerations;
using MANvFAT_Football.Models.Repositories;
using System;
using System.Linq;
using System.Threading;
using System.Web.Mvc;

namespace MANvFAT_Football.Controllers
{
    public class MemberController : BaseController
    {
        private Thread AchievementThread;

        public ActionResult PaymentSuccess(string id)
        {
            return View();
        }

        #region Member Home Page

        // GET: Member
        //id compare from PlayerDashboard.Dashboard URL to get PlayerID
        public ActionResult Index(string id)
        {
            if(SecurityUtils.Enable_PremiumDashboard==false)
            {
                return RedirectToAction("Index", "Home");
            }

            PlayerDashboardRepository modelRepo = new PlayerDashboardRepository();
            //Check if Player is Allowed to Access Premium Dashboard
            var playerDashboard = modelRepo.ReadOne(id);
            //if Didn't found any record then difinitely Player is not registered for Premium Dashboard
            if (playerDashboard == null)
            {
                return RedirectToAction("NoPremiumMembership");
            }

            //If system found a Cookie and Login Cookie Session is similar then let the user Login to Player Dashboard without Displaying Login Screen
            string Reason = "";
            if (modelRepo.ValidateLogin(playerDashboard, this, ref Reason))
            {
                if (playerDashboard.IsFirstLogin)
                {
                    System.Web.Routing.RouteValueDictionary rt = new System.Web.Routing.RouteValueDictionary();
                    rt.Add("id", id);
                    return RedirectToAction("Settings", rt);
                }

                PlayerProgressGallery model = new PlayerProgressGallery();

                model.PlayerID = playerDashboard.PlayerID;
                PlayerImagesRepository pImgRepo = new PlayerImagesRepository();
                model.playerImages = pImgRepo.ReadAll(model.PlayerID.Value, false, false);

                ViewBag.PlayerProgressGallery = model;

                return View(playerDashboard);
            }
            else
            {
                //Otherwise Redirect to Login Screen, if cookie didn't match then redirect to Login Page
                System.Web.Routing.RouteValueDictionary rt = new System.Web.Routing.RouteValueDictionary();
                rt.Add("id", id);
                rt.Add("Reason", Reason);
                return RedirectToAction("Login", rt);
            }

            //We need to Implement Cookies to store Login Session ID.
            //Member can be login for upto 1 week after that they must enter their Password to View their Dashboard

            //Must have Paid Out payment of type "Premium Dashboard" in last 31 days

            //var dashboard = modelRepo.va
        }

        #endregion Member Home Page

        #region Daily Activities

        public ActionResult DailyActivities(string id)
        {
            PlayerDashboardRepository modelRepo = new PlayerDashboardRepository();
            var playerDashboard = modelRepo.ReadOne(id);

            string Reason = "";
            if (modelRepo.ValidateLogin(playerDashboard, this, ref Reason))
            {
                return View(playerDashboard);
            }
            else
            {
                //Otherwise Redirect to Login Screen, if cookie didn't match then redirect to Login Page
                System.Web.Routing.RouteValueDictionary rt = new System.Web.Routing.RouteValueDictionary();
                rt.Add("id", id);
                rt.Add("Reason", Reason);
                return RedirectToAction("Login", "Member", rt);
            }
        }

        public ActionResult _GetPlayerDailyActivity(long PlayerID, DateTime ActivityDate)
        {
            PlayerDailyActivityRepository pdaRepo = new PlayerDailyActivityRepository();

            var activity = pdaRepo.ReadOne(PlayerID, ActivityDate, this);

            return PartialView(activity);
        }

        public ActionResult SaveActivityData(PlayerDailyActivityExt model)
        {
            string Msg = "";
            bool status = true;
            PlayerDailyActivityRepository pdaRepo = new PlayerDailyActivityRepository();
            pdaRepo.CreateOrUpdate(model, ref Msg, ref status, this);

            if (status)
            {
                AchievementsRepository achRepo = new AchievementsRepository();

                Guid guid = Guid.NewGuid();
                AchievementThread = new Thread(() => achRepo.DailyActivity_AchievementPoints(model.PlayerID, AchievementThread));
                AchievementThread.Name = "DailyActivity_AchievementPoints_" + guid.ToString();
                AchievementThread.Start();
            }

            return new JsonResult { Data = true };
        }

        #region AutoComplete Function

        public ActionResult GetDailyActivityAutoComplete(string id, long paramPlayerID, enumDailyActivityTypes activityType)
        {
            PlayerDailyActivityRepository playerDailyActivityRepo = new PlayerDailyActivityRepository();
            var data = playerDailyActivityRepo.GetDailyActivityAutoComplete(paramPlayerID, activityType);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion AutoComplete Function

        #endregion Daily Activities

        #region Weekly Activities

        public ActionResult WeeklyActivities(string id)
        {
            PlayerDashboardRepository modelRepo = new PlayerDashboardRepository();
            var playerDashboard = modelRepo.ReadOne(id);

            string Reason = "";
            if (modelRepo.ValidateLogin(playerDashboard, this, ref Reason))
            {
                return View(playerDashboard);
            }
            else
            {
                //Otherwise Redirect to Login Screen, if cookie didn't match then redirect to Login Page
                System.Web.Routing.RouteValueDictionary rt = new System.Web.Routing.RouteValueDictionary();
                rt.Add("id", id);
                rt.Add("Reason", Reason);
                return RedirectToAction("Login", "Member", rt);
            }
        }

        public ActionResult _GetPlayerWeeklyActivity(long PlayerID, DateTime ActivityDate)
        {
            PlayerWeeklyActivityRepository pweeklyRepo = new PlayerWeeklyActivityRepository();

            var activity = pweeklyRepo.GetWeeklyActivityData(PlayerID, ActivityDate, this);

            return PartialView(activity);
        }

        public ActionResult SaveWeeklyActivityData(PlayerWeeklyActivityExt model)
        {
            string Msg = "";
            bool status = true;
            PlayerWeeklyActivityRepository pweeklyRepo = new PlayerWeeklyActivityRepository();
            pweeklyRepo.CreateOrUpdate(model, ref Msg, ref status, this);

            if (status)
            {
                AchievementsRepository achRepo = new AchievementsRepository();

                Guid guid = Guid.NewGuid();
                AchievementThread = new Thread(() => achRepo.WeeklyActivity_AchievementPoints(model.PlayerID, AchievementThread));
                AchievementThread.Name = "WeeklyActivity_AchievementPoints_" + guid.ToString();
                AchievementThread.Start();
            }

            return new JsonResult { Data = true };
        }

        #region Add/Remove Sub ACtivity

        public ActionResult _AddNewWeeklyActivity(vmWeeklyActivity model)
        {
            PlayerWeeklyActivityExt m = new PlayerWeeklyActivityExt()
            {
                PlayerID = model.PlayerID,
                ActivityDate = model.ActivityDate
            };
            string Msg = "";
            bool status = true;

            PlayerWeeklyActivityRepository pweeklyRepo = new PlayerWeeklyActivityRepository();
            m.PlayerWeeklyActivityID = pweeklyRepo.CreateOrUpdate(m, ref Msg, ref status, this);

            return PartialView(m);
        }

        public ActionResult RemoveWeeklyActivity(long PlayerWeeklyActivityID)
        {
            PlayerWeeklyActivityRepository pweeklyRepo = new PlayerWeeklyActivityRepository();
            pweeklyRepo.DeleteWeeklyActivity(PlayerWeeklyActivityID, this);

            return new JsonResult { Data = true };
        }

        #endregion Add/Remove Sub ACtivity

        #region AutoComplete Function

        public ActionResult GetWeeklyActivityAutoComplete(string id, long paramPlayerID)
        {
            PlayerWeeklyActivityRepository playerWeeklyActivityRepo = new PlayerWeeklyActivityRepository();
            var data = playerWeeklyActivityRepo.GeWeeklyActivityAutoComplete(paramPlayerID);

            return Json(data, JsonRequestBehavior.AllowGet);
        }

        #endregion AutoComplete Function

        #endregion Weekly Activities

        #region Share Activities

        //public ActionResult GetPlayerLeagueCoach(string id, long PlayerID)
        //{
        //    PlayersRepository playerRepo = new PlayersRepository();
        //    var result = playerRepo.GetPlayerLeagueCoach(PlayerID);

        //    return new JsonResult { Data = new { CoachUserID = result.CoachUserID, CoachFullName = result.CoachFullName, CoachEmail = result.CoachEmail } };
        //}

        //public ActionResult GetPlayerTeamEmails(string id, long PlayerID)
        //{
        //    PlayersRepository playerRepo = new PlayersRepository();
        //    var result = playerRepo.GetPlayerTeamMemberEmailAddresses(PlayerID);

        //    return new JsonResult { Data = new { TeamEmailAddresses = result } };
        //}

        public ActionResult ShareActivityData(string id, ShareActivity model)
        {
            if (model.ActivityTypeId == 1) //Daily Activity
            {
                PlayerDailyActivityRepository modelRepo = new PlayerDailyActivityRepository();
                modelRepo.ShareActivity(model, this);
            }
            else if (model.ActivityTypeId == 2) //Weekly Activity
            {
                PlayerWeeklyActivityRepository modelRepo = new PlayerWeeklyActivityRepository();
                modelRepo.ShareActivity(model, this);
            }

            return new JsonResult { Data = true };
        }

        #endregion Share Activities

        #region Member Image Gallery and Options

        public ActionResult ImageGallery(string id)
        {
            PlayerDashboardRepository modelRepo = new PlayerDashboardRepository();
            var playerDashboard = modelRepo.ReadOne(id);

            string Reason = "";
            if (modelRepo.ValidateLogin(playerDashboard, this, ref Reason))
            {
                PlayerProgressGallery model = new PlayerProgressGallery();

                model.PlayerID = playerDashboard.PlayerID;
                PlayerImagesRepository pImgRepo = new PlayerImagesRepository();
                model.playerImages = pImgRepo.ReadAll(model.PlayerID.Value, false, false);

                ViewBag.PlayerProgressGallery = model;

                //Descrypt password
                return View(playerDashboard);
            }
            else
            {
                //Otherwise Redirect to Login Screen, if cookie didn't match then redirect to Login Page
                System.Web.Routing.RouteValueDictionary rt = new System.Web.Routing.RouteValueDictionary();
                rt.Add("id", id);
                rt.Add("Reason", Reason);
                return RedirectToAction("Login", rt);
            }
        }

        public ActionResult _Read_AllImages([DataSourceRequest]DataSourceRequest request, long ParamPlayerID)
        {
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            DataSourceResult result = modelRepo.ReadAll(ParamPlayerID, false, true).ToDataSourceResult(request);
            return Json(result);
        }

        #region Before After Image Creator

        public ActionResult BeforeAfterImageCreator(string id)
        {
            PlayerDashboardRepository modelRepo = new PlayerDashboardRepository();
            var playerDashboard = modelRepo.ReadOne(id);

            string Reason = "";
            if (modelRepo.ValidateLogin(playerDashboard, this, ref Reason))
            {
                //Descrypt password
                return View(playerDashboard);
            }
            else
            {
                //Otherwise Redirect to Login Screen, if cookie didn't match then redirect to Login Page
                System.Web.Routing.RouteValueDictionary rt = new System.Web.Routing.RouteValueDictionary();
                rt.Add("id", id);
                rt.Add("Reason", Reason);
                return RedirectToAction("Login", rt);
            }
        }

        public ActionResult _Read_FirstImages([DataSourceRequest]DataSourceRequest request, long ParamPlayerID)
        {
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            DataSourceResult result = modelRepo.ReadAll(ParamPlayerID, false, true).ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult _Read_SecondImages([DataSourceRequest]DataSourceRequest request, long ParamPlayerID, long ParamImageID)
        {
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            var data = modelRepo.ReadAll(ParamPlayerID, false, true).Where(m => m.PlayerImageID != ParamImageID).ToList();
            DataSourceResult result = data.ToDataSourceResult(request);
            return Json(result);
        }

        #endregion Before After Image Creator

        #endregion Member Image Gallery and Options

        #region Data Visualization / Progress Charts

        //public ActionResult ProgressCharts(string id)
        //{
        //    PlayerDashboardRepository modelRepo = new PlayerDashboardRepository();
        //    var playerDashboard = modelRepo.ReadOne(id);

        //    string Reason = "";
        //    if (modelRepo.ValidateLogin(playerDashboard, this, ref Reason))
        //    {
        //        return View(playerDashboard);
        //    }
        //    else
        //    {
        //        //Otherwise Redirect to Login Screen, if cookie didn't match then redirect to Login Page
        //        System.Web.Routing.RouteValueDictionary rt = new System.Web.Routing.RouteValueDictionary();
        //        rt.Add("id", id);
        //        rt.Add("Reason", Reason);
        //        return RedirectToAction("Login", "Member", rt);
        //    }
        //}

        public ActionResult GetProgressCharts(string id, long PlayerID)
        {
            ProgressChartsRepository paRepo = new ProgressChartsRepository();
            var playerProgressChart = paRepo.ReadAll_Dashboard(PlayerID);

            playerProgressChart.DashboardURL = id;

            return PartialView("_GetProgressCharts", playerProgressChart);
        }

        public ActionResult _ReadChartData([DataSourceRequest]DataSourceRequest request, string id, long paramPlayerID)
        {
            ProgressChartsRepository pcRepo = new ProgressChartsRepository();

            return Json(pcRepo.Read_PlayerWeigtPerWeekChart(paramPlayerID), JsonRequestBehavior.AllowGet);
        }

        #endregion Data Visualization / Progress Charts

        #region Tools & Benefits

        #region Tools

        public ActionResult Tools(string id)
        {
            PlayerDashboardRepository modelRepo = new PlayerDashboardRepository();
            var playerDashboard = modelRepo.ReadOne(id);

            string Reason = "";
            if (modelRepo.ValidateLogin(playerDashboard, this, ref Reason))
            {
                return View(playerDashboard);
            }
            else
            {
                //Otherwise Redirect to Login Screen, if cookie didn't match then redirect to Login Page
                System.Web.Routing.RouteValueDictionary rt = new System.Web.Routing.RouteValueDictionary();
                rt.Add("id", id);
                rt.Add("Reason", Reason);
                return RedirectToAction("Login", "Member", rt);
            }
        }

       

        #endregion Tools

        #region Benefits

        public ActionResult Benefits(string id)
        {
            PlayerDashboardRepository modelRepo = new PlayerDashboardRepository();
            var playerDashboard = modelRepo.ReadOne(id);

            string Reason = "";
            if (modelRepo.ValidateLogin(playerDashboard, this, ref Reason))
            {
                return View(playerDashboard);
            }
            else
            {
                //Otherwise Redirect to Login Screen, if cookie didn't match then redirect to Login Page
                System.Web.Routing.RouteValueDictionary rt = new System.Web.Routing.RouteValueDictionary();
                rt.Add("id", id);
                rt.Add("Reason", Reason);
                return RedirectToAction("Login", "Member", rt);
            }
        }

      

        #endregion Benefits

        #endregion Tools & Benefits

        #region Community

        public ActionResult Community(string id)
        {
            PlayerDashboardRepository modelRepo = new PlayerDashboardRepository();
            var playerDashboard = modelRepo.ReadOne(id);

            string Reason = "";
            if (modelRepo.ValidateLogin(playerDashboard, this, ref Reason))
            {
                return View(playerDashboard);
            }
            else
            {
                //Otherwise Redirect to Login Screen, if cookie didn't match then redirect to Login Page
                System.Web.Routing.RouteValueDictionary rt = new System.Web.Routing.RouteValueDictionary();
                rt.Add("id", id);
                rt.Add("Reason", Reason);
                return RedirectToAction("Login", "Member", rt);
            }
        }

        public ActionResult GetPosts(string id, long PlayerID)
        {
            var posts = RssFeeder.GetProgresDashboardFeeds(PlayerID);
            return PartialView("_GenerateCommunityPosts", posts);
        }

        #endregion Community

        #region Achievements

        public ActionResult Achievements(string id)
        {
            PlayerDashboardRepository modelRepo = new PlayerDashboardRepository();
            var playerDashboard = modelRepo.ReadOne(id);

            string Reason = "";
            if (modelRepo.ValidateLogin(playerDashboard, this, ref Reason))
            {
                return View(playerDashboard);
            }
            else
            {
                //Otherwise Redirect to Login Screen, if cookie didn't match then redirect to Login Page
                System.Web.Routing.RouteValueDictionary rt = new System.Web.Routing.RouteValueDictionary();
                rt.Add("id", id);
                rt.Add("Reason", Reason);
                return RedirectToAction("Login", "Member", rt);
            }
        }

        public ActionResult GetAchievements_PercentCategory(string id, long PlayerID)
        {
            PlayerAchievementsRepository paRepo = new PlayerAchievementsRepository();
            var playerAchievements = paRepo.Read_PecentAchievementCategoryCompleted(PlayerID);

            return PartialView("_GetAchievement_PercentCategory", playerAchievements);
        }

        public ActionResult GetAchievement_Locked_UnLocked(string id, long PlayerID, long AchievementCategoryID)
        {
            PlayerAchievementsRepository paRepo = new PlayerAchievementsRepository();
            var playerAchievements = paRepo.Read_PecentAchievementCategoryCompleted(PlayerID, AchievementCategoryID);

            return PartialView("_GetAchievement_Locked_UnLocked", playerAchievements);
        }

        #endregion Achievements

        #region Notifications

        public ActionResult GetPlayerNotifications(string id, long PlayerID)
        {
            PlayerDashboardNotificationsRepository paRepo = new PlayerDashboardNotificationsRepository();
            var playerNotifications = paRepo.ReadAll_NonDismissed(PlayerID);

            return PartialView("_GetPlayerNotifications", playerNotifications);
        }

        public ActionResult DismissNotification(string id, long PlayerID, long DashboardNotificationID)
        {
            PlayerDashboardNotificationsRepository paRepo = new PlayerDashboardNotificationsRepository();
            paRepo.DismissNotification(DashboardNotificationID, PlayerID);

            return new JsonResult { Data = true };
        }

        #endregion Notifications

        #region Dashboard Settings

        //id = DashboardURLId
        [HttpGet]
        public ActionResult Settings(string id)
        {
            PlayerDashboardRepository modelRepo = new PlayerDashboardRepository();
            var playerDashboard = modelRepo.ReadOne(id);

            string Reason = "";
            if (modelRepo.ValidateLogin(playerDashboard, this, ref Reason))
            {
                //Descrypt password
                return View(playerDashboard);
            }
            else
            {
                //Otherwise Redirect to Login Screen, if cookie didn't match then redirect to Login Page
                System.Web.Routing.RouteValueDictionary rt = new System.Web.Routing.RouteValueDictionary();
                rt.Add("id", id);
                rt.Add("Reason", Reason);
                return RedirectToAction("Login", rt);
            }
        }

        [HttpPost]
        public ActionResult UpdateSettings(string id, PlayerDashboardExt model)
        {
            string _Msg = "";
            bool _status = true;
            //TODO: Validate Information
            PlayerDashboardRepository modelRepo = new PlayerDashboardRepository();
            _status = modelRepo.CreateOrUpdate(model, ref _Msg, this);

            return new JsonResult { Data = new { status = _status, Msg = _Msg } };
        }

        public ActionResult ValidateDashboardURL(string id, string DashboardURL)
        {
            string _Msg = "";
            bool _status = true;

            PlayerDashboardRepository modelRepo = new PlayerDashboardRepository();
            modelRepo.ValidateDashboardURL(id, DashboardURL, ref _Msg, ref _status);

            return new JsonResult { Data = new { status = _status, Msg = _Msg } };
        }

        #endregion Dashboard Settings

        #region Dashboard Daily Reminders


        #endregion  

        #region Member Login, LogOff and Forgotten Password

        #region Login

        [HttpGet]
        //id = PlayerDashboard.Dashboard URL to get PlayerID
        public ActionResult Login(string id)
        {
            PlayerDashboardLogin model = new PlayerDashboardLogin();
            model.DashboardURLId = id;

            if (Request.QueryString["Reason"] != null)
            {
                model.Reason = Convert.ToString(Request.QueryString["Reason"]);
            }

            ViewBag.ModelIsLogin = true;

            return View(model);
        }

        [HttpPost]
        //id = PlayerDashboard.Dashboard URL to get PlayerID
        public ActionResult Login(PlayerDashboardLogin model)
        {
            string Reason = "";
            PlayerDashboardRepository modelRepo = new PlayerDashboardRepository();
            //Check if Player is Allowed to Access Premium Dashboard
            var player = modelRepo.ValidatePlayerDashboard(model.DashboardURLId, model.Password, ref Reason);

            if (player != null)
            {
                string LoginSesstionID = SecurityUtils.GenerateUniqueGUID(UniqueGUIDTable.ProgressDashboard);
                CookiesRespository cookieRepo = new CookiesRespository();
                var httpcookie = cookieRepo.CreateDashboardLoginCookie(LoginSesstionID);

                Response.SetCookie(httpcookie);

                modelRepo.UpdateLoginSessionID(player.PlayerID, LoginSesstionID);

                System.Web.Routing.RouteValueDictionary rt = new System.Web.Routing.RouteValueDictionary();
                rt.Add("id", model.DashboardURLId);

                SecurityUtils.AddAuditLog("Premium Dashboard Member", "Log In - Dashboard URL Id = " + model.DashboardURLId, this);

                return RedirectToAction("Index", rt);
            }

            if (!string.IsNullOrEmpty(Reason))
            {
                model.Reason = Reason;
            }
            else
            {
                model.Reason = "Please correct the Password and try again, Or Click on forgotten password";
                //ModelState.AddModelError("", );
            }

            ViewBag.ModelIsLogin = true;

            return View(model);
        }

        #endregion Login

        //
        // GET: /Account/LogOff

        #region LogOff

        public ActionResult LogOff(string id)
        {
            PlayerDashboardRepository modelRepo = new PlayerDashboardRepository();
            var playerDashboard = modelRepo.ReadOne(id);

            modelRepo.UpdateLoginSessionID(playerDashboard.PlayerID, null);

            SecurityUtils.AddAuditLog("Premium Dashboard Member", "Log Off", this);

            Session.RemoveAll();

            CookiesRespository cookieRepo = new CookiesRespository();
            cookieRepo.DeleteDashboardLoginCookie(this);

            return RedirectToAction("Index", "Home");
        }

        #endregion LogOff

        #region Forgotten & Reset Password

        //id = DashboardURLId
        public ActionResult ForgottenPassword(string id)
        {
            ViewBag.status = "";
            ForgottenPasswordModel model = new ForgottenPasswordModel()
            {
                DashboardUrl = id
            };

            ViewBag.ModelIsLogin = true;

            return View(model);
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
                    var playerDashboard = db.PlayerDashboard.FirstOrDefault(u => u.DashboardURL == model.DashboardUrl);

                    PlayersRepository playerRepo = new PlayersRepository();
                    var player = playerRepo.ReadOne_ByEmailAddress(model.Email);

                    if (playerDashboard != null && player != null)
                    {
                        //Now Send an Email to User for Username and Password!
                        //  if (sys.EmailsEnabled)
                        {
                            try
                            {
                                //Random rndm = new Random();
                                //var RandomNum = rndm.Next(10001, int.MaxValue);
                                Guid guid = Guid.NewGuid();
                                string EncryptedRandomNum = guid.ToString();
                                SecurityUtils.CheckforInvalidFileNameChar(ref EncryptedRandomNum); //it will remove any unsupported characters
                                playerDashboard.PasswordResetCode = EncryptedRandomNum;
                                playerDashboard.ResetCodeExpiry = DateTime.Now.AddHours(24);
                                db.SaveChanges();

                                status = SendEmail(player, EncryptedRandomNum, sys.CurrentDomain);
                            }
                            catch (Exception ex)
                            {
                                ViewBag.status = "Failed to send an Email. Please contact your Site Administrator" + ex.Message;
                                exMsg = ex.Message;
                            }

                            if (status)
                            {
                                //Add To Log
                                SecurityUtils.AddAuditLog("Requested for Forgotten Password", "User \"" + player.FullName + "\" requested for Forgotten Password , Email Sent to: \"" + player.EmailAddress + "\"", this);
                                ViewBag.status = "Your Login Details has been sent to above email address. <a style='color:black' href='/Member/Login/" + model.DashboardUrl + "'> Log In </a>";
                            }
                            else
                            {
                                ViewBag.status = "Failed to send an Email. Please contact your Site Administrator." + exMsg + " - " + EmailsRepository.EmailErrorMsg;
                            }
                        }
                    }
                    else
                    {
                        ViewBag.status = "Your provided email address is not valid. Please contact us at football@manvfat.com";
                    }
                }
            }

            ViewBag.ModelIsLogin = true;

            return View();
        }

        public bool SendEmail(PlayersExt player, string EncryptedRandomNum, string CurrentDomain)
        {
            string ResetPasswordLink = CurrentDomain + "/Member/ResetPassword/" + EncryptedRandomNum;

            MandrillRepository emailRepo = new MandrillRepository();
            bool status = true;

            status = emailRepo.SendForgottenPassword_Email(player, ResetPasswordLink);

            return status;
        }

        [AllowAnonymous]
        [HttpGet]
        public ActionResult ResetPassword(string id)
        {
            string errorMsg = "";
            bool status = true;
            long PlayerID = 0;
            if (id != null)
            {
                using (DBEntities db = new DBEntities())
                {
                    var playerDashboard = db.PlayerDashboard.FirstOrDefault(u => u.PasswordResetCode == id);
                    if (playerDashboard != null)
                    {
                        if (DateTime.Now < playerDashboard.ResetCodeExpiry) //when Reset Code time didn't expired
                        {
                            PlayerID = playerDashboard.PlayerID;
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
                UserID = PlayerID,
                status = status,
                Message = errorMsg
            };

            ViewBag.ModelIsLogin = true;

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
                    var playerDashboard = db.PlayerDashboard.FirstOrDefault(u => u.PlayerID == model.UserID);
                    if (playerDashboard != null)
                    {
                        playerDashboard.PasswordResetCode = null;
                        playerDashboard.ResetCodeExpiry = null;
                        playerDashboard.DashboardPassword = SecurityUtils.EncryptText(model.ConfirmPassword);
                        db.SaveChanges();

                        System.Web.Routing.RouteValueDictionary rt = new System.Web.Routing.RouteValueDictionary();
                        rt.Add("id", playerDashboard.DashboardURL);

                        return RedirectToAction("ResetPasswordSuccess", rt);
                    }
                    else
                    {
                        ModelState.AddModelError("", "Password Reset Code is not valid.");
                    }
                }
            }

            ViewBag.ModelIsLogin = true;

            return View(model);
        }

        public ActionResult ResetPasswordSuccess(string id)
        {
            ViewBag.DashboardURL = id;

            ViewBag.ModelIsLogin = true;

            return View();
        }

        #endregion Forgotten & Reset Password

        #endregion Member Login, LogOff and Forgotten Password

        #region Member Error Pages

        //Error Controller and will be used to redirect the user when he is not a Premium Member
        public ActionResult NoPremiumMembership(string id)
        {
            return View();
        }

        #endregion Member Error Pages
    }
}