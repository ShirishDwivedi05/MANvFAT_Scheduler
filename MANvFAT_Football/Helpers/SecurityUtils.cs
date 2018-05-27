using MANvFAT_Football.Models;
using MANvFAT_Football.Models.Enumerations;
using MANvFAT_Football.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Configuration;
using System.Data;
using System.IO;
using System.Linq;
using System.Security.Cryptography;
using System.Text;
using System.Web;
using System.Web.Mvc;
using System.Web.UI;

namespace MANvFAT_Football.Helpers
{
    public static class SecurityUtils
    {
        #region Static Data Members

        public static string SiteAdminEmail = "admin@manvfat.com";
        public static Boolean UseHTTPS = Convert.ToBoolean(ConfigurationManager.AppSettings["UseHTTPS"]);
        public static Boolean Enable_PremiumDashboard = Convert.ToBoolean(ConfigurationManager.AppSettings["Enable_PremiumDashboard"]);
        // public static DateTime TodayDateTime = GetCurrentDateTime();

        public static string Players_ImagePath = "/Content/Data/PlayerImages/";
        public static string Teams_ImagePath = "/Content/Data/TeamImages/";
        public static string Leagues_ImagePath = "/Content/Data/LeagueImages/";
        public static string Leagues_ExcelImport = "/Content/Data/LeagueImport/";
        public static string Document_Path = "/Content/Data/Documents/";
        public static string ActivityData_Path = "/Content/Data/ActivityData/";
        public static string ToolBenefit_ImagePath = "/Content/Data/ToolBenefitImages/";

        //public static string ApplicationType = ConfigurationManager.AppSettings["ApplicationType"].ToString();
        //public static string ApplicationHeader = GetApplicationHeader();

        public static int Thumb_Height = 150;
        public static int Thumb_Width = 150;

        #endregion Static Data Members

        ////Redirect the User to Error/RestrictedEmailDomain if user enter any Restricted Email Domain while Registration
        public static bool IsRestrictedEmailDomainDetected(string EmailAddress)
        {
            EmailDomainRestrictionsRepository modelRepo = new EmailDomainRestrictionsRepository();
            var RestrictedEmailDomains = modelRepo.ReadAll();

            if (RestrictedEmailDomains.Any(m => EmailAddress.ToLower().EndsWith(m.EmailDomain.ToLower())))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        #region Detect Mobile Device

        public static bool IsMobileDevice(Controller ctrl)
        {
            string strUserAgent = ctrl.Request.UserAgent.ToString().ToLower();
            if (strUserAgent != null)
            {
                if (ctrl.Request.Browser.IsMobileDevice == true || strUserAgent.Contains("iphone") ||
                    strUserAgent.Contains("blackberry") || strUserAgent.Contains("mobile") ||
                    strUserAgent.Contains("windows ce") || strUserAgent.Contains("opera mini") ||
                    strUserAgent.Contains("palm"))
                {
                    return true;
                }
                else
                {
                    return false;
                }
            }
            else
            {
                return false;
            }
        }

        #endregion Detect Mobile Device

        #region Encryption / Decryption Functions

        //OLD=> &#%@,?:*
        //New=> &*%#?@,:
        public static string EncryptText(string strText)
        {
            if (strText != null)
            {
                return Encrypt(strText, "&*%#?@,:");
            }
            else
            {
                return null;
            }
        }

        public static string DecryptCypher(string strText)
        {
            if (strText != null)
            {
                return Decrypt(strText, "&*%#?@,:");
            }
            else
            {
                return null;
            }
        }

        public static string Encrypt(string strText, string strEncrKey)
        {
            //------------------------------------------------------------------------
            //Encryption algorithm code
            //------------------------------------------------------------------------
            byte[] byKey = {
    };
            byte[] IV = {
        0x12,
        0x34,
        0x56,
        0x78,
        0x90,
        0xab,
        0xcd,
        0xef
    };

            try
            {
                byKey = System.Text.Encoding.UTF8.GetBytes(Microsoft.VisualBasic.Strings.Left(strEncrKey, 8));

                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                byte[] inputByteArray = Encoding.UTF8.GetBytes(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateEncryptor(byKey, IV), CryptoStreamMode.Write);
                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                return Convert.ToBase64String(ms.ToArray());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        public static string Decrypt(string strText, string sDecrKey)
        {
            //------------------------------------------------------------------------
            //Decryption algorithm code
            //------------------------------------------------------------------------
            byte[] byKey = {
    };
            byte[] IV = {
        0x12,
        0x34,
        0x56,
        0x78,
        0x90,
        0xab,
        0xcd,
        0xef
    };
            byte[] inputByteArray = new byte[strText.Length + 1];

            strText = Microsoft.VisualBasic.Strings.Replace(strText, " ", "+");

            try
            {
                byKey = System.Text.Encoding.UTF8.GetBytes(Microsoft.VisualBasic.Strings.Left(sDecrKey, 8));
                DESCryptoServiceProvider des = new DESCryptoServiceProvider();
                inputByteArray = Convert.FromBase64String(strText);
                MemoryStream ms = new MemoryStream();
                CryptoStream cs = new CryptoStream(ms, des.CreateDecryptor(byKey, IV), CryptoStreamMode.Write);

                cs.Write(inputByteArray, 0, inputByteArray.Length);
                cs.FlushFinalBlock();
                System.Text.Encoding encoding = System.Text.Encoding.UTF8;

                return encoding.GetString(ms.ToArray());
            }
            catch (Exception ex)
            {
                return ex.Message;
            }
        }

        #endregion Encryption / Decryption Functions

        #region Get User

        //Uncomment Following Code to
        /// <summary>
        /// Get Current Logged in User Details
        /// </summary>
        /// <param name="ctlr"></param>
        /// <returns>User Details from Users Table</returns>

        public static UsersExt GetUserDetails()
        {
            string Username = HttpContext.Current.User.Identity.Name.ToLower();
            UsersRepository user = new UsersRepository();
            return user.ReadOne(Username);
        }

        public static UsersExt GetUserDetails(HttpContextBase httpContext)
        {
            string Username = httpContext.User.Identity.Name.ToLower();
            UsersRepository user = new UsersRepository();
            return user.ReadOne(Username);
        }

        public static UsersExt GetUserDetails(string Username)
        {
            UsersRepository user = new UsersRepository();
            return user.ReadOne(Username);
        }

        #endregion Get User

        #region File name Validation Functions

        public static List<char> GetInvalidFileNameCharacters()
        {
            List<char> invalidPathChars = Path.GetInvalidFileNameChars().ToList();
            invalidPathChars.Add('&');
            invalidPathChars.Add('?');
            invalidPathChars.Add('{');
            invalidPathChars.Add('}');
            invalidPathChars.Add('\'');
            invalidPathChars.Add('/');
            // invalidPathChars.Add('=');
            invalidPathChars.Add('+');
            invalidPathChars.Add('*');
            invalidPathChars.Add('%');
            // invalidPathChars.Add('$');
            //  invalidPathChars.Add('£');
            //  invalidPathChars.Add('!');
            //   invalidPathChars.Add('-');
            //   invalidPathChars.Add('_');
            // invalidPathChars.Add('.');
            //   invalidPathChars.Add('`');
            //   invalidPathChars.Add('¬');
            invalidPathChars.Add('#');
            //   invalidPathChars.Add('~');
            //    invalidPathChars.Add(']');
            //    invalidPathChars.Add('[');
            //    invalidPathChars.Add('@');
            invalidPathChars.Add(':');
            invalidPathChars.Add(';');
            invalidPathChars.Add('>');
            invalidPathChars.Add('<');
            //    invalidPathChars.Add(',');
            //    invalidPathChars.Add('|');

            return invalidPathChars;
        }

        public static void CheckforInvalidFileNameChar(ref string filename)
        {
            List<char> invalidFilenameChar = SecurityUtils.GetInvalidFileNameCharacters();

            foreach (char c in invalidFilenameChar)
            {
                filename = filename.Replace(c.ToString(), "");
            }
        }

        public static bool IsValidExcelExtension(string FileExtension)
        {
            string[] ValidExtensions = { ".xls", ".xlsx" };

            return ValidExtensions.Any(m => m.Equals(FileExtension));
        }

        public static string GenerateUniqueReferralCode()
        {
            string ReferralCode = "";
            bool IsExists = false;

            using (DBEntities db = new DBEntities())
            {
                Random rndm = new Random();
                do
                {
                    ReferralCode = "MVF" + rndm.Next(1111111, 9999999).ToString();

                    IsExists = false;
                }
                while (IsExists == true);
            }

            return ReferralCode;
        }

        #endregion File name Validation Functions

        #region URL Validation Function, to remove NON Url Characters

        public static List<char> GetInvalidURLCharacters()
        {
            List<char> invalidChars = Path.GetInvalidFileNameChars().ToList();
            invalidChars.Add('&');
            invalidChars.Add('?');
            invalidChars.Add('{');
            invalidChars.Add('}');
            invalidChars.Add('\'');
            invalidChars.Add('/');
            invalidChars.Add('=');
            invalidChars.Add('+');
            invalidChars.Add('*');
            invalidChars.Add('%');
            invalidChars.Add('$');
            invalidChars.Add('£');
            invalidChars.Add('!');
            invalidChars.Add('-');
            invalidChars.Add('_');
            invalidChars.Add('.');
            invalidChars.Add('`');
            invalidChars.Add('¬');
            invalidChars.Add('#');
            invalidChars.Add('~');
            invalidChars.Add(']');
            invalidChars.Add('[');
            invalidChars.Add('@');
            invalidChars.Add(':');
            invalidChars.Add(';');
            invalidChars.Add('>');
            invalidChars.Add('<');
            invalidChars.Add(',');
            invalidChars.Add('|');
            invalidChars.Add(' ');

            return invalidChars;
        }

        public static void Check_RemoveInvalidURLChar(ref string URL)
        {
            List<char> invalidURLChar = SecurityUtils.GetInvalidURLCharacters();

            foreach (char c in invalidURLChar)
            {
                URL = URL.Replace(c.ToString(), "");
            }
        }

        #endregion URL Validation Function, to remove NON Url Characters

        #region Conversion Helping Functions

        public static DateTime? ConvertToDateTimeOrNull(string val)
        {
            if (val != "" && val != null)
                return Convert.ToDateTime(val);
            else
                return null;
        }

        public static int? ConvertToIntOrNull(string val)
        {
            if (val != "" && val != null)
                return Convert.ToInt32(val);
            else
                return null;
        }

        public static long? ConvertToLongOrNull(string val)
        {
            if (val != "" && val != null)
                return Convert.ToInt64(val);
            else
                return null;
        }

        public static decimal ConvertToDecimalOrZero(object val)
        {
            if (val != null)
                return Convert.ToDecimal(val);
            else
                return 0.00M;
        }

        public static bool? ConvertToBooleanOrNull(string val)
        {
            if (val != "" && val != null)
                return Convert.ToBoolean(val);
            else
                return null;
        }

        public static decimal? ConvertToDecimalOrNull(string val)
        {
            if (val != "" && val != null)
                return Convert.ToDecimal(val);
            else
                return null;
        }

        public static decimal? ConvertToDecimalOrZero(string val)
        {
            if (val != "" && val != null)
                return Convert.ToDecimal(val);
            else
                return 0;
        }

        public static void ConvertToLong(string val, ref long result)
        {
            if (val != "")
                result = Convert.ToInt64(val);
        }

        public static void ConvertToDecimal(string val, ref Decimal result)
        {
            if (val != "")
                result = Convert.ToDecimal(val);
        }

        public static void ConvertToDateTime(string val, ref DateTime result)
        {
            if (val != "")
                result = Convert.ToDateTime(val);
        }

        public static void ConvertToBoolean(string val, ref Boolean result)
        {
            if (val != "")
                result = Convert.ToBoolean(val);
        }

        public static string TrimText(string str, int n)
        {
            if (str.Length > n) return str.Substring(0, n - 3) + "...";
            return str;
        }

        public static string GetRowStampString(byte[] RowVersion)
        {
            var enumuration = RowVersion.GetEnumerator();
            string _RowVersion = "";
            while (enumuration.MoveNext())
            {
                _RowVersion += enumuration.Current.ToString();
            }

            return _RowVersion;
        }

        public static string GetRowVersionByte(byte[] RowVersion)
        {
            var enumuration = RowVersion.GetEnumerator();
            string _RowVersion = "";
            while (enumuration.MoveNext())
            {
                _RowVersion += enumuration.Current.ToString();
            }

            return _RowVersion;
        }

        public static String EnumToString(this Enum en)
        {
            return Enum.GetName(en.GetType(), en);
        }

        public static T ParseEnum<T>(string value)
        {
            return (T)Enum.Parse(typeof(T), value, true);
        }

        #endregion Conversion Helping Functions

        public static string GeneratePayLinkURL(string PayLinkID)
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();

            string _PayLink = sys.CurrentDomain + "/Home/PayNow/" + PayLinkID;

            return _PayLink;
        }

        public static string GenerateDashboardURL(string DashboardURL)
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();

            string _PayLink = sys.CurrentDomain + "/Member/" + DashboardURL;

            return _PayLink;
        }

        public static string GenerateEmbedURL(string VideoURL, ref string Msg, ref bool status)
        {
            string _VideoURL = "";

            if (VideoURL.ToLower().Contains("embed") || VideoURL.ToLower().Contains("player.vimeo"))
            {
                _VideoURL = VideoURL;
            }
            else
            {
                if (VideoURL.ToLower().StartsWith("http") && VideoURL.ToLower().Contains("youtube"))
                {
                    System.Uri uri = new Uri(VideoURL);
                    string v = System.Web.HttpUtility.ParseQueryString(uri.Query).Get("v");

                    _VideoURL = "https://www.youtube.com/embed/" + v;
                }
                else if (VideoURL.ToLower().StartsWith("http") && VideoURL.ToLower().Contains("vimeo"))
                {
                    System.Uri uri = new Uri(VideoURL);

                    _VideoURL = "https://player.vimeo.com/video/" + uri.Segments[1];
                }
                else
                {
                    Msg = "Video URL is not valid.";
                    status = false;
                }
            }
            return _VideoURL;
        }

        public static string RenderPartialToString(Controller controller, string partialViewName, object model, ViewDataDictionary viewData, TempDataDictionary tempData)
        {
            ViewEngineResult result = ViewEngines.Engines.FindPartialView(controller.ControllerContext, partialViewName);

            if (result.View != null)
            {
                controller.ViewData.Model = model;
                StringBuilder sb = new StringBuilder();
                using (StringWriter sw = new StringWriter(sb))
                {
                    using (HtmlTextWriter output = new HtmlTextWriter(sw))
                    {
                        ViewContext viewContext = new ViewContext(controller.ControllerContext, result.View, viewData, tempData, output);
                        result.View.Render(viewContext, output);
                    }
                }

                return sb.ToString();
            }

            return String.Empty;
        }

        #region Audit Log

        /// <summary>
        /// Add Audit Log Entries
        /// </summary>
        /// <param name="_Action"> Action happened </param>
        /// <param name="ctlr">Object of Current Controller, just use "this" keyword</param>
        public static void AddAuditLog(string AuditLogShortDesc, string AuditLogLongDesc, Controller ctlr = null)
        {
            //long? userID = null;

            //if (ctlr != null && ctlr.User != null)
            //{
            //    if (ctlr.User.Identity.IsAuthenticated)
            //    {
            //        using (DBEntities db = new DBEntities())
            //        {
            //            var user = db.Users.FirstOrDefault(m => m.EmailAddress == ctlr.User.Identity.Name);
            //            userID = user.UserID;
            //        }
            //    }
            //}
            //AuditLogs log = new AuditLogs()
            //{
            //    LogDate = DateTime.Now,
            //    AuditLogShortDesc = AuditLogShortDesc,
            //    AuditLogLongDesc = AuditLogLongDesc,
            //    UserID = userID
            //};

            //if (log.UserID == null)
            //{
            //    //If UserID is null it means this log is generated by Automation so add to AuditLog History Table in MVF_AuditHistory Database
            //    var auditLogRepository = new AuditLogRepository();
            //    auditLogRepository.AddAuditLog_ToHistory(log);
            //}
            //else
            //{
            //    var auditLogRepository = new AuditLogRepository();
            //    auditLogRepository.AddAuditLog(log);
            //}
        }

        public static void AddAuditLog(string AuditLogShortDesc, string AuditLogLongDesc, long UserID)
        {
            //AuditLogs log = new AuditLogs()
            //{
            //    LogDate = DateTime.Now,//DateTime.Now,
            //    AuditLogShortDesc = AuditLogShortDesc,
            //    AuditLogLongDesc = AuditLogLongDesc,
            //    UserID = UserID
            //};

            //var auditLogRepository = new AuditLogRepository();
            //auditLogRepository.AddAuditLog(log);
        }

        /// <summary>
        /// Audit logging for transactions, where we pass in the data context and save the changes from outside
        /// </summary>
        //public static void AddAuditLog(string _Action, long? userID, DBEntities db)
        //{
        //    //Add to Log

        //    var auditLogRepository = new AuditLogRepository(db);
        //    auditLogRepository.AddAuditLog(_Action, userID);
        //}

        #endregion Audit Log

        #region Convert List to DataSet

        public static DataSet ConvertListToDataSet<T>(this IList<T> list)
        {
            Type elementType = typeof(T);
            DataSet ds = new DataSet();
            DataTable t = new DataTable();
            ds.Tables.Add(t);

            //add a column to table for each public property on T
            foreach (var propInfo in elementType.GetProperties())
            {
                Type ColType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

                t.Columns.Add(propInfo.Name, ColType);
            }

            //go through each property on T and add each value to the table
            foreach (T item in list)
            {
                DataRow row = t.NewRow();

                foreach (var propInfo in elementType.GetProperties())
                {
                    row[propInfo.Name] = propInfo.GetValue(item, null) ?? DBNull.Value;
                }

                t.Rows.Add(row);
            }

            return ds;
        }

        #endregion Convert List to DataSet

        #region Error Validations

        public static bool ErrorMsgstoIgnore(string Msg)
        {
            bool Ignore = false;

            string[] ErrorMsgs = {
                                     "A public action method 'Registration' was not found on controller"
                                     ,"The controller for path '/apple-app-site-association' was not found or does not implement IController"
                                     ,"The controller for path '/.well-known/apple-app-site-association' was not found or does not implement IController"
                                     ,"http://www.google.com/bot.html"
                                     ,"Server cannot set status after HTTP headers have been sent"
                                     ,"http://www.veooz.com/veoozbot.html"
                                     ,"A potentially dangerous Request.Path value was detected from the client (:)"
                                     ,"http://help.yahoo.com/help/us/ysearch/slurp"
                                     ,"http://www.gnip.com/"
                                     ,"does not belong to this list"
                                     ,"yandex.com/bots"
                                     ,"mj12bot.com"
                                     ,"guess.scritch.org"
                                 };

            List<string> ListOfErrorMsgs = new List<string>();
            ListOfErrorMsgs.AddRange(ErrorMsgs.ToList());

            if (!string.IsNullOrEmpty(Msg))
            {
                if (ErrorMsgs.Any(a => Msg.ToLower().Contains(a.ToLower())))
                {
                    Ignore = true;
                }
            }

            return Ignore;
        }

        #endregion Error Validations
    }

    public class CalculateTotalFunds
    {
        public Int32 RegionID { get; set; }
        public long? ClientID { get; set; }
        public decimal? Value { get; set; }
    }

    public class PartialView
    {
        public string ViewString { get; set; }

        public PartialView()
        {
            ViewString = "";
        }
    }

    public enum SQLErrorEnums
    {
        //Users
        Username,

        //Clients
        ClientName_AlreadyExists
    }
}

namespace System.Web.Mvc
{
    using MANvFAT_Football.Helpers;
    using System;
    using System.Diagnostics.CodeAnalysis;

    [SuppressMessage("Microsoft.Performance", "CA1813:AvoidUnsealedAttributes",
        Justification = "Unsealed because type contains virtual extensibility points.")]
    [AttributeUsage(AttributeTargets.Class | AttributeTargets.Method, Inherited = true, AllowMultiple = false)]
    public class RequireHttpsProdAttribute : FilterAttribute, IAuthorizationFilter
    {
        public virtual void OnAuthorization(AuthorizationContext filterContext)
        {
            if (filterContext == null)
            {
                throw new ArgumentNullException("filterContext");
            }

            string absURL = filterContext.HttpContext.Request.Url.AbsoluteUri;

            //modified to check for local requests since cant use ssl for dev server.
            //SecurityUtils.AddAuditLog("HTTPS", "absURL = "+ absURL + " & filterContext.HttpContext.Request.IsSecureConnection = " + filterContext.HttpContext.Request.IsSecureConnection + " & filterContext.HttpContext.Request.IsLocal = " + filterContext.HttpContext.Request.IsLocal);
            if (absURL.Contains("GoCardless/Webhook") || absURL.Contains("ICalc/Index") || absURL.Contains("Alerts/GetTotalNumberOfNewAlerts"))
            {
                //SecurityUtils.AddAuditLog("HTTPS", "YES IT IS GoCardless/Webhook");
            }
            else
            {
                //SecurityUtils.AddAuditLog("HTTPS", "NO IT IS Not GoCardless/Webhook");
                if (!filterContext.HttpContext.Request.IsSecureConnection && !filterContext.HttpContext.Request.IsLocal)
                {
                    if (SecurityUtils.UseHTTPS)
                        HandleNonHttpsRequest(filterContext);
                }
            }
        }

        protected virtual void HandleNonHttpsRequest(AuthorizationContext filterContext)
        {
            // only redirect for GET requests, otherwise the browser might not propagate the verb and request
            // body correctly.

            string absURL = filterContext.HttpContext.Request.Url.AbsoluteUri;

            //if (!String.Equals(filterContext.HttpContext.Request.HttpMethod, "GET", StringComparison.OrdinalIgnoreCase))
            //{
            //    throw new InvalidOperationException("Requests to the given url must use SSL");
            //}

            // redirect to HTTPS version of page
            string url = "https://" + filterContext.HttpContext.Request.Url.Host + filterContext.HttpContext.Request.RawUrl;
            filterContext.Result = new RedirectResult(url);
        }
    }
}