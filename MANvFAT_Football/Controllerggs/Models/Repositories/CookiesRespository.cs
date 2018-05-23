using MANvFAT_Football.Helpers;
using System;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MANvFAT_Football.Models.Repositories
{
    public class CookiesRespository
    {
        public static string Coach_SysMsg_CookieName = "sysMsg";
        public static string PremiumDashboard_CookieName = "pmem";
        public static string MobileNumVerification_CookieName = "accmobver";

        #region Premium Dashboard Login Cookies

        public HttpCookie CreateDashboardLoginCookie(string LoginSessionID)
        {
            string CookieName = PremiumDashboard_CookieName;

            HttpCookie cookie = new HttpCookie(CookieName);

            cookie.Values["LoginSessionID"] = LoginSessionID;

            cookie.Expires = DateTime.Now.AddDays(7);

            return cookie;
        }

        public CookieExt GetDashboardLoginCookie(Controller ctrl)
        {
            CookieExt Logincookie = new CookieExt();
            try
            {
                string CookieName = PremiumDashboard_CookieName;
                HttpCookie cookie = null;
                if (ctrl.Request.Cookies.AllKeys.Contains(CookieName))
                {
                    cookie = ctrl.Request.Cookies[CookieName];
                }

                if (cookie != null)
                {
                    Logincookie = new CookieExt()
                    {
                        LoginSessionID = ctrl.Request.Cookies[CookieName].Values["LoginSessionID"].ToString(),
                    };
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
            }
            return Logincookie;
        }

        public void DeleteDashboardLoginCookie(Controller ctrl)
        {
            string CookieName = PremiumDashboard_CookieName;
            if (ctrl.Request.Cookies.AllKeys.Contains(CookieName))
            {
                HttpCookie cookie = ctrl.Request.Cookies[CookieName];
                cookie.Expires = DateTime.Now.AddDays(-1);
                ctrl.Response.SetCookie(cookie);
                //ctrl.Request.Cookies.Set(cookie);
            }
        }

        #endregion

        #region Coach System Message Cookies

        public HttpCookie CreateCoach_SysMsg_Cookie()
        {
            string CookieName = Coach_SysMsg_CookieName;

            HttpCookie cookie = new HttpCookie(CookieName);

            cookie.Values["MsgAccepted"] = "1";

            cookie.Expires = new DateTime(DateTime.Now.Year, DateTime.Now.Month, DateTime.Now.Day, 23, 59, 59);

            return cookie;
        }

        public CookieExt GetCoach_SysMsg_Cookie(Controller ctrl)
        {
            CookieExt SysMsgCookie = new CookieExt();
            try
            {
                string CookieName = Coach_SysMsg_CookieName;
                HttpCookie cookie = null;
                if (ctrl.Request.Cookies.AllKeys.Contains(CookieName))
                {
                    cookie = ctrl.Request.Cookies[CookieName];
                }

                if (cookie != null)
                {
                    SysMsgCookie = new CookieExt()
                    {
                        IsSysMsgAccepted = ctrl.Request.Cookies[CookieName].Values["MsgAccepted"].ToString(),
                    };
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
            }
            return SysMsgCookie;
        }

        public void DeleteCoach_SysMsg_Cookie(Controller ctrl)
        {
            string CookieName = Coach_SysMsg_CookieName;
            if (ctrl.Request.Cookies.AllKeys.Contains(CookieName))
            {
                HttpCookie cookie = ctrl.Request.Cookies[CookieName];
                cookie.Expires = DateTime.Now.AddDays(-1);
                ctrl.Response.SetCookie(cookie);
                //ctrl.Request.Cookies.Set(cookie);
            }
        }

        #endregion

        #region MobileNumVerrification Cookies

        public HttpCookie CreateMobileNumVerrification_Cookie(string EmailAddress)
        {
            string CookieName = MobileNumVerification_CookieName;

            HttpCookie cookie = new HttpCookie(CookieName);

            cookie.Values["MobVer"] = SecurityUtils.EncryptText(EmailAddress);

            cookie.Expires = DateTime.Now.AddDays(7); //Expire after a Week

            return cookie;
        }

        public CookieExt GetMobileNumVerification_Cookie(Controller ctrl)
        {
            CookieExt modelCookie = new CookieExt();
            try
            {
                string CookieName = MobileNumVerification_CookieName;
                HttpCookie cookie = null;
                if (ctrl.Request.Cookies.AllKeys.Contains(CookieName))
                {
                    cookie = ctrl.Request.Cookies[CookieName];
                }

                if (cookie != null)
                {
                    //The Cookie value with Mobile Verification Number is stored in Cookie as Encrypted value
                    var Encrypted_EmailAddress = ctrl.Request.Cookies[CookieName].Values["MobVer"].ToString();
                    try
                    {
                        //it always be a number. So, check if it's an Integer value by Decrypting the Cookie value
                        string _EmailAddress;
                        _EmailAddress = SecurityUtils.DecryptCypher(Encrypted_EmailAddress);

                        modelCookie = new CookieExt()
                        {
                            EmailAddress_MobVerification = _EmailAddress
                        };
                    }
                    catch (Exception ex) 
                    {
                        ErrorHandling.HandleException(ex);
                    }
                    
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
            }
            //if Cookie found and had valid value then return the value, otherwise it will be Zero i.e. modelCookie.MobileVerficationCode = 0
            return modelCookie;
        }

        public void DeleteMobileNumVerrification_Cookie(Controller ctrl)
        {
            string CookieName = MobileNumVerification_CookieName;
            if (ctrl.Request.Cookies.AllKeys.Contains(CookieName))
            {
                HttpCookie cookie = ctrl.Request.Cookies[CookieName];
                cookie.Expires = DateTime.Now.AddDays(-1);
                ctrl.Response.SetCookie(cookie);
                //ctrl.Request.Cookies.Set(cookie);
            }
        }

        #endregion
    }

    public class CookieExt
    {
        //For Premium Dashboard
        public string LoginSessionID { get; set; }

        //For Coach Accepted Message
        public string IsSysMsgAccepted { get; set; }

        //For Mobile Verification
        public string EmailAddress_MobVerification { get; set; }
    }


}