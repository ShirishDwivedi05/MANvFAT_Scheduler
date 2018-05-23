using EasyHttp.Http;
using log4net;
using MANvFAT_Football.Helpers;
using System.Collections.Generic;
using System.Dynamic;
using System.Net;
using System.Web.Mvc;

namespace MANvFAT_Football.Models.Repositories
{
    internal enum MandrillError
    {
        OK,
        WebException,
        HttpNotOk,
        Invalid,
        Rejected,
        Unknown
    }

    public class MandrillRepository
    {
        private static string MandrillBaseUrl = "https://mandrillapp.com/api/1.0/";

        #region Testing Template

        /*
        public bool SendActivationEMail(out string errorMsg)
        {
            string activationLink = "http://usmanchohan.co.uk";
            //HttpContext.Current.Request.Url.GetLeftPart(UriPartial.Authority) + "/Register/Activation.aspx?id=" + ts.Id;

            //send-template(string key, string template_name, array template_content, struct message)
            dynamic sendParams = new ExpandoObject();
            sendParams.key = "Z46-URL8pJppuh8Hv5Ff3A";
            sendParams.template_name = "Secret Project Trial Activation";

            sendParams.template_content = new List<dynamic>();

            sendParams.message = new ExpandoObject();
            sendParams.message.subject = "Here's your Secret Project activation email";
            sendParams.message.from_email = "info@SecretProject.com";
            sendParams.message.from_name = "Secret Project";

            sendParams.message.to = new List<dynamic>();
            sendParams.message.to.Add(new ExpandoObject());
            sendParams.message.to[0].email = "usmanakram_86@yaho.com";
            sendParams.message.to[0].name = "Usman Akram";

            sendParams.message.track_opens = true;
            //sendParams.message.track_clicks = true;

            sendParams.message.global_merge_vars = new List<dynamic>();
            sendParams.message.global_merge_vars.Add(new ExpandoObject());
            sendParams.message.global_merge_vars[0].name = "NAME";
            sendParams.message.global_merge_vars[0].content = "Chohan";

            sendParams.message.global_merge_vars.Add(new ExpandoObject());
            sendParams.message.global_merge_vars[1].name = "LINK";
            sendParams.message.global_merge_vars[1].content = activationLink;

            errorMsg = string.Empty;

            MandrillError merr = SendMessage(sendParams);

            switch (merr)
            {
                case MandrillError.OK:
                    return true;

                case MandrillError.WebException:
                case MandrillError.HttpNotOk:
                    errorMsg = "There was an issue sending your activation e-mail. Please try again later or call us directly.";
                    break;

                case MandrillError.Invalid:
                    errorMsg = "Your email address appears to be invalid. Please try again with a valid address, or call us directly.";
                    break;

                case MandrillError.Rejected:
                    errorMsg = "Your activation email was rejected. Please try again with a valid address, or call us directly.";
                    break;

                case MandrillError.Unknown:
                    errorMsg = "There was an unknown problem sending your activation email. Please try again, or call us directly.";
                    break;
            }
            return false;
        }
        */

        #endregion Testing Template

        public bool SendRegistrationEmail(PlayersExt model,  Controller ctrl)
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();
            //Add this Postfix if it is a TESTING Site.
            string TestSitePostfix = (sys.CurrentDomain.Contains("test") || sys.CurrentDomain.Contains("localhost")) ? "_TestSite" : "";

            dynamic sendParams = new ExpandoObject();
           
                sendParams.template_name = "tmpRegistration_PlannedLeagueCOPY" + TestSitePostfix;
            

           // string LeagueHomePage = sys.CurrentDomain + "/" + league.NewsTag;
            string ReferralPageLink = sys.CurrentDomain + "/Refer/" + model.ReferralCode;
            string ReferralLink = sys.CurrentDomain + "/refercode/" + model.ReferralCode;

            sendParams.template_content = new List<dynamic>();

            sendParams.message = new ExpandoObject();
            sendParams.message.subject = "Welcome to MAN v FAT Football";
            sendParams.message.to = new List<dynamic>();
            sendParams.message.to.Add(new ExpandoObject());
            sendParams.message.to[0].email = model.EmailAddress;
            sendParams.message.to[0].name = model.FullName;

            //sendParams.message.track_opens = true;
            //sendParams.message.track_clicks = true;

            sendParams.message.global_merge_vars = new List<dynamic>();

            sendParams.message.global_merge_vars.Add(new ExpandoObject());
            sendParams.message.global_merge_vars[0].name = "BMI";
            sendParams.message.global_merge_vars[0].content = model.BMI.HasValue ? model.BMI.Value.ToString("N2") : "";

            sendParams.message.global_merge_vars.Add(new ExpandoObject());
            sendParams.message.global_merge_vars[1].name = "BODYFAT";
            sendParams.message.global_merge_vars[1].content = model.BodyFat.HasValue ? model.BodyFat.Value.ToString("N2") : "";

            sendParams.message.global_merge_vars.Add(new ExpandoObject());
            sendParams.message.global_merge_vars[2].name = "LEAGUEHOMEPAGE";
            sendParams.message.global_merge_vars[2].content = //LeagueHomePage;

            sendParams.message.global_merge_vars.Add(new ExpandoObject());
            sendParams.message.global_merge_vars[3].name = "WHATSAPPGROUP";
            sendParams.message.global_merge_vars[3].content = //WhatsAppGroupLink;

            sendParams.message.global_merge_vars.Add(new ExpandoObject());
            sendParams.message.global_merge_vars[4].name = "MSGNOEMERGENCYDETAILS";
            sendParams.message.global_merge_vars[4].content = GenerateHtml_EmergencyContactDetails(sys, model);

            sendParams.message.global_merge_vars.Add(new ExpandoObject());
            sendParams.message.global_merge_vars[5].name = "REFERRALPAGELINK";
            sendParams.message.global_merge_vars[5].content = ReferralPageLink;

            sendParams.message.global_merge_vars.Add(new ExpandoObject());
            sendParams.message.global_merge_vars[6].name = "REFERRALLINK";
            sendParams.message.global_merge_vars[6].content = ReferralLink;

            sendParams.message.global_merge_vars.Add(new ExpandoObject());
            sendParams.message.global_merge_vars[7].name = "REFERRALCODE";
            sendParams.message.global_merge_vars[7].content = model.ReferralCode;

            ////Get Premium Dashboard Payment Link
            //PlayerPaymentsRepository playerPaymentsRepo = new PlayerPaymentsRepository();
            //var payment = playerPaymentsRepo.GetPremiumDashboardPayment(model.PlayerID, ctrl);

            //sendParams.message.global_merge_vars.Add(new ExpandoObject());
            //sendParams.message.global_merge_vars[5].name = "PAYLINKDASHBOARD";
            //sendParams.message.global_merge_vars[5].content = payment != null ? SecurityUtils.GeneratePayLinkURL(payment.PaylinkID) : "";

            MandrillError merr = SendMessage(sendParams, sys);
            string errorMsg = "";
            switch (merr)
            {
                case MandrillError.OK:
                    SecurityUtils.AddAuditLog("Mandrill Email Success", "\"Registration Email\" Email Sent To PlayerID: " + model.PlayerID + " Email = " + model.EmailAddress);
                    return true;

                case MandrillError.WebException:
                case MandrillError.HttpNotOk:
                    errorMsg = "There was an issue sending your activation e-mail. Please try again later or call us directly.";
                    SecurityUtils.AddAuditLog("Mandrill Email Error", "\"Registration Email\" Attempt to Send Email To = PlayerID: " + model.PlayerID + " Email = " + model.EmailAddress + " - HttpNotOK: " + errorMsg);
                    break;

                case MandrillError.Invalid:
                    errorMsg = "Your email address appears to be invalid. Please try again with a valid address, or call us directly.";
                    SecurityUtils.AddAuditLog("Mandrill Email Error", "\"Registration Email\" Invalid: " + errorMsg);
                    break;

                case MandrillError.Rejected:
                    errorMsg = "Your activation email was rejected. Please try again with a valid address, or call us directly.";
                    SecurityUtils.AddAuditLog("Mandrill Email Error", "\"Registration Email\" Rejected: " + errorMsg);
                    break;

                case MandrillError.Unknown:
                    errorMsg = "There was an unknown problem sending your activation email. Please try again, or call us directly.";
                    SecurityUtils.AddAuditLog("Mandrill Email Error", "\"Registration Email\" Unknown: " + errorMsg);
                    break;
            }
            return false;
        }

      
        public bool SendPaymentSuccessForDashboard_Email(PlayerDashboardExt model)
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();
            //Add this Postfix if it is a TESTING Site.
            string TestSitePostfix = (sys.CurrentDomain.Contains("test") || sys.CurrentDomain.Contains("localhost")) ? "_TestSite" : "";

            dynamic sendParams = new ExpandoObject();
            sendParams.template_name = "tmpPaymentSuccessForDashboard" + TestSitePostfix;

            sendParams.template_content = new List<dynamic>();

            sendParams.message = new ExpandoObject();
            sendParams.message.subject = "MANvFAT Football Progress Dashboard Payment Received";
            sendParams.message.to = new List<dynamic>();
            sendParams.message.to.Add(new ExpandoObject());
            sendParams.message.to[0].email = model.PlayerEmailAddress;
            sendParams.message.to[0].name = model.PlayerFullName;

            //sendParams.message.track_opens = true;
            //sendParams.message.track_clicks = true;


            sendParams.message.global_merge_vars = new List<dynamic>();

            sendParams.message.global_merge_vars.Add(new ExpandoObject());
            sendParams.message.global_merge_vars[0].name = "DASHBOARDURL";
            sendParams.message.global_merge_vars[0].content = SecurityUtils.GenerateDashboardURL(model.DashboardURL);

            sendParams.message.global_merge_vars.Add(new ExpandoObject());
            sendParams.message.global_merge_vars[1].name = "DASHBOARDPASSWORD";
            sendParams.message.global_merge_vars[1].content = model.DashboardPassword;

            MandrillError merr = SendMessage(sendParams, sys);
            string errorMsg = "";
            switch (merr)
            {
                case MandrillError.OK:
                    SecurityUtils.AddAuditLog("Mandrill Email Success", "\"Payment Success Email\" Email Sent To PlayerID: " + model.PlayerID + " Email = " + model.PlayerEmailAddress);
                    return true;

                case MandrillError.WebException:
                case MandrillError.HttpNotOk:
                    errorMsg = "There was an issue sending your activation e-mail. Please try again later or call us directly.";
                    SecurityUtils.AddAuditLog("Mandrill Email Error", " \"Payment Success Email\" Attempt to Send Email To = PlayerID: " + model.PlayerID + " Email = " + model.PlayerEmailAddress + " - HttpNotOK: " + errorMsg);
                    break;

                case MandrillError.Invalid:
                    errorMsg = "Your email address appears to be invalid. Please try again with a valid address, or call us directly.";
                    SecurityUtils.AddAuditLog("Mandrill Email Error", "\"Payment Success Email\" Invalid: " + errorMsg);
                    break;

                case MandrillError.Rejected:
                    errorMsg = "Your activation email was rejected. Please try again with a valid address, or call us directly.";
                    SecurityUtils.AddAuditLog("Mandrill Email Error", "\"Payment Success Email\" Rejected: " + errorMsg);
                    break;

                case MandrillError.Unknown:
                    errorMsg = "There was an unknown problem sending your activation email. Please try again, or call us directly.";
                    SecurityUtils.AddAuditLog("Mandrill Email Error", "\"Payment Success Email\" Unknown: " + errorMsg);
                    break;
            }
            return false;
        }

     
        public bool ProgressDashboard_Standalone(PlayerDashboardExt model)
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();

            var DashboardURL = sys.CurrentDomain + "/Member/" + model.DashboardURL;

            //Add this Postfix if it is a TESTING Site.
            string TestSitePostfix = (sys.CurrentDomain.Contains("test") || sys.CurrentDomain.Contains("localhost")) ? "_TestSite" : "";

            dynamic sendParams = new ExpandoObject();

            sendParams.template_name = "ProgressDashboard_Standalone" + TestSitePostfix;

            sendParams.template_content = new List<dynamic>();

            sendParams.message = new ExpandoObject();
            sendParams.message.subject = "Your Progress Dashboard at MAN v FAT Football";
            sendParams.message.to = new List<dynamic>();

            sendParams.message.to.Add(new ExpandoObject());
            sendParams.message.to[0].email = model.PlayerEmailAddress;
            sendParams.message.to[0].name = model.PlayerFullName;


            //sendParams.message.track_opens = true;
            //sendParams.message.track_clicks = true;

            sendParams.message.global_merge_vars = new List<dynamic>();

            sendParams.message.global_merge_vars.Add(new ExpandoObject());
            sendParams.message.global_merge_vars[0].name = "PROGRESSDASHLINK";
            sendParams.message.global_merge_vars[0].content = DashboardURL;

            sendParams.message.global_merge_vars.Add(new ExpandoObject());
            sendParams.message.global_merge_vars[1].name = "PROGRESSDASHPASS";
            sendParams.message.global_merge_vars[1].content = SecurityUtils.DecryptCypher(model.DashboardPassword);

            MandrillError merr = SendMessage(sendParams, sys);
            string errorMsg = "";
            switch (merr)
            {
                case MandrillError.OK:
                    SecurityUtils.AddAuditLog("Mandrill Email Success", "\"ProgressDashboard_Standalone\" Email Sent To PlayerID: " + model.PlayerID + " Email = " + model.PlayerEmailAddress);
                    return true;

                case MandrillError.WebException:
                case MandrillError.HttpNotOk:
                    errorMsg = "There was an issue sending your activation e-mail. Please try again later or call us directly.";
                    SecurityUtils.AddAuditLog("Mandrill Email Error", "\"ProgressDashboard_Standalone\" Attempt to Send Email To = PlayerID: " + model.PlayerID + " Email = " + model.PlayerEmailAddress + " - HttpNotOK: " + errorMsg);
                    break;

                case MandrillError.Invalid:
                    errorMsg = "Your email address appears to be invalid. Please try again with a valid address, or call us directly.";
                    SecurityUtils.AddAuditLog("Mandrill Email Error", "\"ProgressDashboard_Standalone\" Invalid: " + errorMsg);
                    break;

                case MandrillError.Rejected:
                    errorMsg = "Your activation email was rejected. Please try again with a valid address, or call us directly.";
                    SecurityUtils.AddAuditLog("Mandrill Email Error", "\"ProgressDashboard_Standalone\" Rejected: " + errorMsg);
                    break;

                case MandrillError.Unknown:
                    errorMsg = "There was an unknown problem sending your activation email. Please try again, or call us directly.";
                    SecurityUtils.AddAuditLog("Mandrill Email Error", "\"ProgressDashboard_Standalone\" Unknown: " + errorMsg);
                    break;
            }
            return false;
        }
       
        public bool SendForgottenPassword_Email(PlayersExt model, string ResetPasswordLink)
        {
            //
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();
            //Add this Postfix if it is a TESTING Site.
            string TestSitePostfix = (sys.CurrentDomain.Contains("test") || sys.CurrentDomain.Contains("localhost")) ? "_TestSite" : "";

            dynamic sendParams = new ExpandoObject();
            sendParams.template_name = "ForgottenPassword"+TestSitePostfix;

            sendParams.template_content = new List<dynamic>();

            sendParams.message = new ExpandoObject();
            sendParams.message.subject = "MANvFAT Football Forgotten Password";
            sendParams.message.to = new List<dynamic>();
            sendParams.message.to.Add(new ExpandoObject());
            sendParams.message.to[0].email = model.EmailAddress;
            sendParams.message.to[0].name = model.FullName;

            //sendParams.message.track_opens = true;
            //sendParams.message.track_clicks = true;

            sendParams.message.global_merge_vars = new List<dynamic>();

            sendParams.message.global_merge_vars.Add(new ExpandoObject());
            sendParams.message.global_merge_vars[0].name = "RESETPASSWORDLINK";
            sendParams.message.global_merge_vars[0].content = ResetPasswordLink;

            MandrillError merr = SendMessage(sendParams, sys);
            string errorMsg = "";
            switch (merr)
            {
                case MandrillError.OK:
                    SecurityUtils.AddAuditLog("Mandrill Email Success", "\"Forgotten Password Email\" Email Sent To PlayerID: " + model.PlayerID + " Email = " + model.EmailAddress);
                    return true;

                case MandrillError.WebException:
                case MandrillError.HttpNotOk:
                    errorMsg = "There was an issue sending your activation e-mail. Please try again later or call us directly.";
                    SecurityUtils.AddAuditLog("Mandrill Email Error", " \"Forgotten Password Email\" Attempt to Send Email To = PlayerID: " + model.PlayerID + " Email = " + model.EmailAddress + " - HttpNotOK: " + errorMsg);
                    break;

                case MandrillError.Invalid:
                    errorMsg = "Your email address appears to be invalid. Please try again with a valid address, or call us directly.";
                    SecurityUtils.AddAuditLog("Mandrill Email Error", "\"Forgotten Password Email\" Invalid: " + errorMsg);
                    break;

                case MandrillError.Rejected:
                    errorMsg = "Your activation email was rejected. Please try again with a valid address, or call us directly.";
                    SecurityUtils.AddAuditLog("Mandrill Email Error", "\"Forgotten Password Email\" Rejected: " + errorMsg);
                    break;

                case MandrillError.Unknown:
                    errorMsg = "There was an unknown problem sending your activation email. Please try again, or call us directly.";
                    SecurityUtils.AddAuditLog("Mandrill Email Error", "\"Forgotten Password Email\" Unknown: " + errorMsg);
                    break;
            }
            return false;
        }

        //Red/Amber Flag Alert Emails
      

        private MandrillError SendMessage(dynamic sendParams, SystemSettingsExt sys)
        {
            if (sys.EmailsEnabled)
            {
                sendParams.key = "LMPoi1kWRMJJQMWCHFsJIQ";//"Z46-URL8pJppuh8Hv5Ff3A";

                sendParams.message.from_email = sys.AdminEmailAddress;// "info@usmanchohan.co.uk";
                sendParams.message.from_name = "MANvFAT Football";

                if (sys.SendTempEmail)
                {
                    sendParams.message.to[0].email = sys.TempEmailAddress;
                    sendParams.message.to[0].name = "MVFF Test Email";
                }

                ILog _log = log4net.LogManager.GetLogger("Mandrill/SendMessage");

                string url = MandrillBaseUrl + "/messages/send-template.json";

                var http = new HttpClient
                {
                    Request = { Accept = HttpContentTypes.ApplicationJson }
                };

                EasyHttp.Http.HttpResponse response;
                try
                {
                    response = http.Post(url, sendParams, HttpContentTypes.ApplicationJson);
                }
                catch (WebException ex)
                {
                    ErrorHandling.HandleException(ex);
                    _log.ErrorFormat("Error: WebException - {0}", ex.Message);
                    return MandrillError.WebException;
                }

                if (response.StatusCode != HttpStatusCode.OK)
                {
                    _log.InfoFormat("Response = {0} - {1}", response.StatusCode, response.StatusDescription);
                    _log.Info(response.RawText);
                    SecurityUtils.AddAuditLog("Mandrill Email Error", "Rejected: " + response.RawText);
                    return MandrillError.HttpNotOk;
                }

                dynamic rv = response.DynamicBody;
                string msgResponse = string.Format("email: {0}, status: {1}", rv[0].email, rv[0].status);
                _log.InfoFormat(msgResponse);

                SecurityUtils.AddAuditLog("Mandrill Email Info", "Info: " + msgResponse);

                string send_status = rv[0].status;
                if (send_status == "sent" || send_status == "queued")
                {
                    //SecurityUtils.AddAuditLog("Mandrill Email Success", "send_status = " + send_status);
                    return MandrillError.OK;
                }
                // otherwise, it should be "rejected" or "invalid"
                if (send_status == "invalid")
                {
                    return MandrillError.Invalid;
                }
                if (send_status == "rejected")
                {
                    return MandrillError.Rejected;
                }

                // unexpected...
                return MandrillError.Unknown;
            }
            else
            {
                SecurityUtils.AddAuditLog("Mandrill Email Failed", "Emails are Disabled from System Settings");
                return MandrillError.Unknown;
            }
        }

        public string GenerateHtml_EmergencyContactDetails(SystemSettingsExt sys, PlayersExt player)
        {
            string html = "";

            if (string.IsNullOrEmpty(player.Emergency_ContactName) || string.IsNullOrEmpty(player.Emergency_ContactPhone))
            {
                string EncryptedEmail = SecurityUtils.EncryptText(player.EmailAddress);
                string _EmergencyContactPageLink = sys.CurrentDomain + "/Home/EmergencyContact?p=" + EncryptedEmail;

                html = "<h2 style='color:red;'>Emergency Contact Details:</h2>" +
                        "<p> As agreed in your terms and conditions all players are required to provide a contact in case of illness or injury. Fill in the form at" +
                        "<a target = '_blank' href = '" + _EmergencyContactPageLink + "'> this link </a> and hit Submit to enable us to finalise your registration. Please ignore this if you've already submitted your Emergency Contact Details.</p>";
            }

            return html;
        }
    }
}