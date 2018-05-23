using MANvFAT_Football.Helpers;
using System.Web.Mvc;

namespace MANvFAT_Football.Controllers
{
    [HandleError]
    public class ErrorController : BaseController
    {
        public static ExceptionContext filterContext;

        public ActionResult getErrorCode(string id)
        {
            string errorCodeNumber = ErrorHandling.ErrorCode;
            ErrorHandling.ErrorCode = null;
            return new JsonResult { Data = new { errorCode = errorCodeNumber } };
        }

        public ActionResult Index()
        {
            return View();
        }

        public ActionResult UnAuthorizedAccess()
        {
            if (Request.QueryString["Locked"] != null)
            {
                ViewBag.Locked = "Your cannot log on the System, because your status is Locked. Please contact your Administrator.";
            }
            return View();
        }

        public ActionResult RestrictedEmailDomain()
        {
            return View();
        }

        #region Page not Found Exception Handling Page

        public ActionResult PageNotFound()
        {
            return View();
        }

        #endregion Page not Found Exception Handling Page

        #region Unhandled Exception view

        public ActionResult UnhandledException()
        {
            return View();
        }

        #endregion Unhandled Exception view

        public ActionResult UnexpectedError()
        {
            return View();
        }

        public ActionResult CookiesAreNotEnabled()
        {
            return View();
        }
    }
}