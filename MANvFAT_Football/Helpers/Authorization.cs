using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;
using MANvFAT_Football.Models;
using MANvFAT_Football.Models.Enumerations;
using MANvFAT_Football.Models.Repositories;

namespace MANvFAT_Football.Helpers
{
    public class Authorization : AuthorizeAttribute
    {
        private string[] _permissions { get; set; }

        public Authorization(params object[] permission)
        {
            this._permissions = permission.Select(p => p.ToString()).ToArray();
        }

        protected override bool AuthorizeCore(HttpContextBase httpContext)
        {
            //Default Authentication Method

            bool validate = false;
            bool locked = false;
            if (httpContext.User.Identity.IsAuthenticated)
            {
                UsersExt LoggedInUser = SecurityUtils.GetUserDetails(httpContext);

                if (LoggedInUser != null)
                {
                    if (LoggedInUser.Locked != true)
                    {
                        foreach (var item in this._permissions)
                        {
                            var _enum = Enum.Parse(typeof(Permissions), item);

                            if (Convert.ToInt32(Permissions.AllUsers) == Convert.ToInt32(_enum))
                                validate = true;

                            if (LoggedInUser.RoleID == Convert.ToInt32(_enum))
                                validate = true;
                        }
                    }
                    else
                    {
                        locked = true;
                    }
                }
            }
            else
            {
                validate = false;
            }

            if (validate)
                return true;
            else
            {
                if (httpContext.Request.Url.Segments.Count() <= 1 || httpContext.Request.Url.PathAndQuery.Contains("Home"))
                {
                    httpContext.Response.StatusCode = 200;
                    httpContext.Response.Redirect("/Account/LogOn");
                    ErrorHandling.SetErrorCode("UnauthorizedAccess");
                    //The Following Code will be used to Redirect the User to their own Login Screen Based on their Role

                    //if (httpContext.Session["RoleID"] != null)
                    //{
                    //    if ((Int32)httpContext.Session["RoleID"] == (Int32)Permissions.Admin ||
                    //        (Int32)httpContext.Session["RoleID"] == (Int32)Permissions.HeadOffice ||
                    //        (Int32)httpContext.Session["RoleID"] == (Int32)Permissions.RegionalOffice)
                    //    {
                    //        httpContext.Response.Redirect("/Account/LogOn");
                    //    }
                    //    else if ((Int32)httpContext.Session["RoleID"] == (Int32)Permissions.ReadOnly)
                    //    {
                    //        httpContext.Response.Redirect("/Home/LogOn");
                    //    }
                    //}
                    //else
                    //{ httpContext.Response.Redirect("/Account/LogOn"); }
                }
                else
                {
                    httpContext.Response.StatusCode = 200;
                    if (locked)
                        httpContext.Response.Redirect("/Error/UnAuthorizedAccess?Locked=True");
                    else
                        httpContext.Response.Redirect("/Error/UnAuthorizedAccess");

                    ErrorHandling.SetErrorCode("UnauthorizedAccess");
                }

                return false;
            }
        }
    }



}