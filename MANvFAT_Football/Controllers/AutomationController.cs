using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Repositories;
using System.Threading;
using System.Web.Mvc;

namespace MANvFAT_Football.Controllers
{
    public class AutomationController : Controller
    {
        
        // GET: Automation

      
        #region 09 - Dashboard Data Share

        public ActionResult DashboardDataShare(string secret)
        {
            if (secret == "M@nvF@t@123")
            {
                PlayerDashboardRepository modelRepo = new PlayerDashboardRepository();
                modelRepo.AutoDataShare(this);

                return Json("DashboardDataShare DONE", JsonRequestBehavior.AllowGet);
            }

            return Json("Wrong Password", JsonRequestBehavior.AllowGet);
        }

        #endregion 09 - Dashboard Data Share

        #region 10 - Dashboard Achievements

        public ActionResult DashboardAchievements(string secret)
        {
            if (secret == "M@nvF@t@123")
            {
                if (SecurityUtils.Enable_PremiumDashboard)
                {
                    AchievementsRepository modelRepo = new AchievementsRepository();
                    modelRepo.AutoTrigger_AchievementPoints();
                }
                return Json("DashboardAchievements DONE", JsonRequestBehavior.AllowGet);
            }

            return Json("Wrong Password", JsonRequestBehavior.AllowGet);
        }

        #endregion 10 - Dashboard Achievements

        #region 11 - Create Recurring Dashboard Notifications

        public ActionResult CreateRecurringNotifications(string secret)
        {
            if (secret == "M@nvF@t@123")
            {
                DashboardNotificationsRepository modelRepo = new DashboardNotificationsRepository();
                modelRepo.CreateRecurringNotifications(this);

                return Json("CreateRecurringNotifications DONE", JsonRequestBehavior.AllowGet);
            }

            return Json("Wrong Password", JsonRequestBehavior.AllowGet);
        }

        #endregion 11 - Create Recurring Dashboard Notifications

        #region 12 - Delete Dismissed Dashboard Notifications

        public ActionResult DeleteDismissedNotifications(string secret)
        {
            if (secret == "M@nvF@t@123")
            {
                DashboardNotificationsRepository modelRepo = new DashboardNotificationsRepository();
                modelRepo.DeleteDismissedNotifications();

                return Json("DeleteDismissedNotifications DONE", JsonRequestBehavior.AllowGet);
            }

            return Json("Wrong Password", JsonRequestBehavior.AllowGet);
        }

        #endregion 12 - Delete Dismissed Dashboard Notifications

    
    }
}