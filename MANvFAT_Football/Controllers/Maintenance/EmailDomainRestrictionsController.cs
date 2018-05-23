﻿using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Repositories;
using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using System.Web.Mvc;
using MANvFAT_Football.Models.Enumerations;

namespace MANvFAT_Football.Controllers.Maintenance
{
    [Authorization(Permissions.Administrator)]
    public class EmailDomainRestrictionsController : BaseController
    {
        // GET: EmailDomainRestrictions
        public ActionResult Index()
        {
            return View();
        }

        #region Grid Create, Read, Update, Delete Functions

        public ActionResult _Read([DataSourceRequest]DataSourceRequest request)
        {
            EmailDomainRestrictionsRepository modelRepo = new EmailDomainRestrictionsRepository();
            DataSourceResult result = modelRepo.ReadAll().ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult _Create([DataSourceRequest]DataSourceRequest request, EmailDomainRestrictionsExt model)
        {
            if (ModelState.IsValid)
            {
                string Msg = "";

                EmailDomainRestrictionsRepository modelRepo = new EmailDomainRestrictionsRepository();
                if (modelRepo.Create(model, ref Msg, this) == false)
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

        public ActionResult _Destroy([DataSourceRequest]DataSourceRequest request, EmailDomainRestrictionsExt model)
        {
            string Msg = "";

            EmailDomainRestrictionsRepository modelRepo = new EmailDomainRestrictionsRepository();
            if (modelRepo.Delete(model, ref Msg, this) == false)
            {
                return this.Json(new DataSourceResult { Errors = Msg });
            }

            return Json(request);
        }

        public ActionResult _Update([DataSourceRequest]DataSourceRequest request, EmailDomainRestrictionsExt model)
        {
            if (ModelState.IsValid)
            {
                string Msg = "";

                EmailDomainRestrictionsRepository modelRepo = new EmailDomainRestrictionsRepository();
                if (modelRepo.Update(model, ref Msg, this) == false)
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