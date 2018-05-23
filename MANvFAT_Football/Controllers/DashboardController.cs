using Kendo.Mvc.UI;
using Kendo.Mvc.Extensions;
using MANvFAT_Football.Models.Repositories;
using System.Web.Mvc;
using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Enumerations;

namespace MANvFAT_Football.Controllers
{
    [Authorization(Permissions.AllUsers)]
    public class DashboardController : BaseController
    {
        // GET: Dashboard
        public ActionResult Index()
        {
            return View();
        }

        public ActionResult _ReadAdvertisementPercent([DataSourceRequest]DataSourceRequest request)
        {
            AdvertisementsRepository modelRepo = new AdvertisementsRepository();
            DataSourceResult result = modelRepo.Read_Dashboard().ToDataSourceResult(request);
            return Json(result);
        }

        public ActionResult OtherDetails()
        {
            AdvertisementsRepository modelRepo = new AdvertisementsRepository();
            return View(modelRepo.ReadAll_OtherDetailsList());
        }
    }
}