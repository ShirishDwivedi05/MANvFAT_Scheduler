using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Enumerations;
using System.Web.Mvc;

namespace MANvFAT_Football.Controllers
{
    [Authorization(Permissions.Administrator)]
    public class MaintenanceController : BaseController
    {
        // GET: Maintenance
        public ActionResult Users()
        {
            return View();
        }


        public ActionResult EmailDomainRestrictions()
        {
            return View();
        }
        public ActionResult HeaderText()
        {
            return View();
        }
    }
}