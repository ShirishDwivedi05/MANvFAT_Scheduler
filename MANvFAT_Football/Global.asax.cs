using Elmah;
using MANvFAT_Football.Helpers;
using System;
using System.Web;
using System.Web.Mvc;
using System.Web.Optimization;
using System.Web.Routing;

namespace MANvFAT_Football
{
    public class MvcApplication : System.Web.HttpApplication
    {
        protected void Application_Start()
        {
            if (SecurityUtils.UseHTTPS)
            {
                GlobalFilters.Filters.Add(new RequireHttpsProdAttribute());
            }


            AreaRegistration.RegisterAllAreas();
            FilterConfig.RegisterGlobalFilters(GlobalFilters.Filters);
            RouteConfig.RegisterRoutes(RouteTable.Routes);
            BundleConfig.RegisterBundles(BundleTable.Bundles);
        }

        protected void Application_Error(object sender, EventArgs e)
        {
            try
            {
                var context = HttpContext.Current;
                var exception = context.Server.GetLastError();
                if (exception is HttpException)
                {
                    //context.Server.ClearError();    // Here is the new line.
                    //Response.Clear();
                    //Response.StatusCode = 200;
                    //Response.Redirect("/Error/UnhandledException");
                    ////Response.Write(@"<html><head></head><body>hello</body></html>");
                    //Response.End();
                    //return;
                }
            }
            catch
            {

            }
        }
        //protected  void Application_BeginRequest()
        //{
        //    System.Threading.Thread.CurrentThread.CurrentCulture = System.Globalization.CultureInfo.CreateSpecificCulture("en-GB");
        //}

        protected void ErrorLog_Filtering(object sender, ExceptionFilterEventArgs e)
        {
            if (SecurityUtils.ErrorMsgstoIgnore(e.Exception.Message))
            {
                e.Dismiss();
            }
        }
    }
}