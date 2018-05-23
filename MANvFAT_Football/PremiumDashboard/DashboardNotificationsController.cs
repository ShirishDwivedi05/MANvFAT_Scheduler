using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Enumerations;
using MANvFAT_Football.Models.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace MANvFAT_Football.Controllers
{
    [Authorization(Permissions.Administrator)]
    public class DashboardNotificationsController : Controller
    {
        // GET: DashboardNotifications
        public ActionResult Index()
        {
            return View();
        }

        #region Grid Create, Read, Update, Delete Functions

        public ActionResult _Read([DataSourceRequest]DataSourceRequest request)
        {
            DashboardNotificationsRepository modelRepo = new DashboardNotificationsRepository();
            DataSourceResult result = modelRepo.ReadAll().ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult _Create([DataSourceRequest]DataSourceRequest request, DashboardNotificationExt model)
        {
            if (ModelState.IsValid)
            {
                string Msg = "";

                DashboardNotificationsRepository modelRepo = new DashboardNotificationsRepository();
                if (modelRepo.CreateOrUpdate(model, ref Msg, this) == false)
                {
                    return this.Json(new DataSourceResult { Errors = Msg });
                }
            }
            else
            {
                string error = ErrorHandling.HandleModelStateErrors(ModelState);
                return this.Json(new DataSourceResult { Errors = error });
            }

            return Json(new[] { model }.ToDataSourceResult(request, ModelState));
        }

        public ActionResult _Destroy([DataSourceRequest]DataSourceRequest request, DashboardNotificationExt model)
        {
            string Msg = "";


            DashboardNotificationsRepository modelRepo = new DashboardNotificationsRepository();
            if (modelRepo.Delete(model, ref Msg, this) == false)
            {
                return this.Json(new DataSourceResult { Errors = Msg });
            }

            return Json(request);
        }

        public ActionResult _Update([DataSourceRequest]DataSourceRequest request, DashboardNotificationExt model)
        {
            if (ModelState.IsValid)
            {
                string Msg = "";

                DashboardNotificationsRepository modelRepo = new DashboardNotificationsRepository();
                if (modelRepo.CreateOrUpdate(model, ref Msg, this) == false)
                {
                    return this.Json(new DataSourceResult { Errors = Msg });
                }
            }
            else
            {
                string error = ErrorHandling.HandleModelStateErrors(ModelState);
                return this.Json(new DataSourceResult { Errors = error });
            }
            return Json(request);
        }

        #endregion Grid Create, Read, Update, Delete Functions
    }
}