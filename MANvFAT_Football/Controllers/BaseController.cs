using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Enumerations;
using MANvFAT_Football.Models.Repositories;
using System;
using System.Linq;
using System.Web.Mvc;

namespace MANvFAT_Football.Controllers
{
    public class BaseController : Controller
    {
        // GET: Base
        protected override void OnActionExecuted(ActionExecutedContext context)
        {
            if(context.Exception!=null)
            {
                ErrorHandling.HandleException(context.Exception);
            }

            var result = context.Result as ViewResultBase;
            if (result == null)
            {
                // The controller action didn't return a view result
                // => no need to continue any further
                return;
            }

            bool _IsAdmin = false, _IsLeagueViewer = false, _IsCoach= false, _DownForMaintenance=false;
            bool _IsTechUser = false; //hello@manvaft.com and tech@manvfat.com are the Tech users, and only allowed to edit System Settings.
            decimal _TotalLostByAllLeagues = 0.00M;
            string _DownForMaintenance_Time = "";
            string[] TechUserEmails = { "hello@manvfat.com", "tech@manvfat.com", "micky.begra@gmail.com" };

            try
            {
                if (User.Identity.IsAuthenticated)
                {
                   

                    var LoggedInUser = SecurityUtils.GetUserDetails();
                    if (LoggedInUser != null)
                    {
                        _IsAdmin = (LoggedInUser.RoleID == (long)Permissions.Administrator);
                        _IsLeagueViewer = (LoggedInUser.RoleID == (long)Permissions.LeagueViewer);
                        _IsCoach = (LoggedInUser.RoleID == (long)Permissions.Coaches);
                        _IsTechUser = (TechUserEmails.Any(m => m.ToLower().Equals(LoggedInUser.EmailAddress.ToLower())));
                    }

                    SystemSettingsRepository sysRepo = new SystemSettingsRepository();
                    var sys = sysRepo.GetSystemSettings();
                    _DownForMaintenance = sys.DownForMaintenance;
                    _DownForMaintenance_Time = sys.DownForMaintenance_Timer;
                }

             
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
            }

            ViewBag.IsAdmin = _IsAdmin;
            ViewBag.IsLeagueViewer = _IsLeagueViewer;
            ViewBag.IsCoach = _IsCoach;
            ViewBag.IsTechUser = _IsTechUser;
            ViewBag.TotalLostByAllLeagues = _TotalLostByAllLeagues.ToString("N2");
            ViewBag.DownForMaintenance = _DownForMaintenance;
            ViewBag.DownForMaintenance_Time = _DownForMaintenance_Time;

            //var model = result.Model as BaseVm;
            //if (model == null)
            //{
            //    // there's no model or the model was not of the expected type
            //    // => no need to continue any further
            //    return;
            //}

            //var userName = UserPropertiesGetter.GetUserName(context.HttpContext);
            //var userPropertiesGetter = new UserPropertiesGetter();
            ////Get the Roles and Permissions for this user
            //var dataRolesPermissions = userPropertiesGetter.GetDataRolesPermissions(context.HttpContext, userName);

            ////Get the users permissions for each controller action in the application
            //var permissionsHelper = new PermissionsHelper();
            //permissionsHelper.GetAppPermissions(dataRolesPermissions);

            //// Put the user data into the viewmodel
            //model.UserDataRolesPermissions = dataRolesPermissions;
        }
    }
}