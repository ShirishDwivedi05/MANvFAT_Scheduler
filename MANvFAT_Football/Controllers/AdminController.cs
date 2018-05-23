using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Enumerations;
using System.Web.Mvc;

namespace MANvFAT_Football.Controllers
{
    [Authorization(Permissions.AllUsers)]
    public class AdminController : BaseController
    {
        // GET: Admin
        public ActionResult Index()
        {
            return View();
        }
    }
}