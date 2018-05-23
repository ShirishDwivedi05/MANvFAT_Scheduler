using MANvFAT_Football.Helpers;
using System;
using System.Net.Mail;
using System.Threading;
using System.Web.Mvc;

namespace MANvFAT_Football.Models.Repositories
{
    public class EmailsRepository : BaseRepository
    {
        public static string EmailErrorMsg = "";

        public void ContactEmail(string Name, string Email, string Msg, Controller ctrl)
        {
            try
            {
                string subject = "MANvFAT Football Enquiry";
                string Body = "Hi Admin,<br/>" +
                        "Received and enquiry from MANvFAT Football website<br/><br/>" +
                        "Name: " + Name + "<br/>" +
                        "Email: " + Email + "<br/>" +
                        "Message: " + Msg + "<br/>";
                SendEmail(SecurityUtils.SiteAdminEmail, "football@manvfat.com", subject, Body);
            }
            catch (Exception ex)
            {
                Msg = Msg + ErrorHandling.HandleException(ex);
            }
        }

        public Boolean SendEmail(string From, string To, string Subject, string Body, string BCC = null)
        {
            //  SecurityUtils.AddAuditLog("Inside SendEmail", "From = " + From + " To = " + To + "Subject =" +Subject+ "  Body = "+ Body);
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();

            //SecurityUtils.AddAuditLog("GetSystemSettings", "sys.EmailsEnabled = " + sys.EmailsEnabled.ToString() + " sys.TempEmailAddress = " + sys.TempEmailAddress);

            string[] EmailAddresses = To.Replace(" ", string.Empty).Split(';');

            bool status = true;

            try
            {
                // using (SmtpClient smtp = new SmtpClient("mail.manvfatfootball.org", 587))
                using (SmtpClient smtp = new SmtpClient())
                {
                    // smtp.Credentials = new NetworkCredential("admin@manvfatfootball.org", "Email@manvfat123");
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.EnableSsl = false;
                    if (sys.EmailsEnabled)
                        foreach (var item in EmailAddresses)
                        {
                            if (item != "")
                            {
                                MailMessage mm;
                                MailAddress from = new MailAddress(From, "MANvFAT Football");
                                //(1) Create the MailMessage instance
                                if (sys.SendTempEmail)
                                {
                                    mm = new MailMessage(From, sys.TempEmailAddress, Subject, Body);
                                    mm.IsBodyHtml = true;
                                }
                                else
                                {
                                    mm = new MailMessage();

                                    mm.IsBodyHtml = true;
                                    mm.From = from;
                                    mm.Subject = Subject;
                                    mm.Body = Body;
                                }

                                mm.To.Add(item);

                                if (!string.IsNullOrEmpty(BCC))
                                {
                                    mm.Bcc.Add(BCC);
                                }

                                smtp.Send(mm);
                            }
                        }
                }
            }
            catch (Exception ex)
            {
                SecurityUtils.AddAuditLog("EmailErrorMsg", ex.Message);
                status = false;
                EmailErrorMsg = ex.Message;// + " - " + ex.InnerException != null ? ex.InnerException.Message : "";
                SecurityUtils.AddAuditLog("Failed to send Email", "Email Send Failed with Error = " + EmailErrorMsg + "<br/> Stack Trace:<br/>" + ex.StackTrace);
            }

            return status;
        }

        public Boolean SendEmail_WithCC(string From, string To, string Subject, string Body, string CC = null, string BCC = null)
        {
            //  SecurityUtils.AddAuditLog("Inside SendEmail", "From = " + From + " To = " + To + "Subject =" +Subject+ "  Body = "+ Body);
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();

            //SecurityUtils.AddAuditLog("GetSystemSettings", "sys.EmailsEnabled = " + sys.EmailsEnabled.ToString() + " sys.TempEmailAddress = " + sys.TempEmailAddress);

            bool status = true;

            try
            {
                // using (SmtpClient smtp = new SmtpClient("mail.manvfatfootball.org", 587))
                using (SmtpClient smtp = new SmtpClient())
                {
                    // smtp.Credentials = new NetworkCredential("admin@manvfatfootball.org", "Email@manvfat123");
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.EnableSsl = false;
                    if (sys.EmailsEnabled)

                        if (!string.IsNullOrEmpty(To))
                        {
                            MailMessage mm;
                            MailAddress from = new MailAddress(From, "MANvFAT Football");
                            //(1) Create the MailMessage instance
                            if (sys.SendTempEmail)
                            {
                                mm = new MailMessage(From, sys.TempEmailAddress, Subject, Body);
                                mm.IsBodyHtml = true;
                            }
                            else
                            {
                                mm = new MailMessage();

                                mm.IsBodyHtml = true;
                                mm.From = from;
                                mm.Subject = Subject;
                                mm.Body = Body;
                            }

                            mm.To.Add(To);

                            if (!string.IsNullOrEmpty(BCC))
                            {
                                string[] EmailAddresses_BCC = BCC.Replace(" ", string.Empty).Split(';');

                                foreach (var item_BCC in EmailAddresses_BCC)
                                {
                                    if (!string.IsNullOrEmpty(item_BCC))
                                    {
                                        mm.Bcc.Add(item_BCC);
                                    }
                                }
                            }

                            if (!string.IsNullOrEmpty(CC))
                            {
                                string[] EmailAddresses_CC = CC.Replace(" ", string.Empty).Split(';');

                                foreach (var item_CC in EmailAddresses_CC)
                                {
                                    if (!string.IsNullOrEmpty(item_CC))
                                        mm.CC.Add(item_CC);
                                }
                            }

                            //Now Send Email
                            smtp.Send(mm);
                        }
                }
            }
            catch (Exception ex)
            {
                SecurityUtils.AddAuditLog("EmailErrorMsg", ex.Message);
                status = false;
                EmailErrorMsg = ex.Message;// + " - " + ex.InnerException != null ? ex.InnerException.Message : "";
                SecurityUtils.AddAuditLog("Failed to send Email", "Email Send Failed with Error = " + EmailErrorMsg + "<br/> Stack Trace:<br/>" + ex.StackTrace);
            }

            return status;
        }

        public Boolean SendEmail(string From, string To, string Subject, string Body, Thread thread)
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();

            string[] EmailAddresses = To.Replace(" ", string.Empty).Split(';');
            bool status = true;

            try
            {
                // using (SmtpClient smtp = new SmtpClient("mail.manvfatfootball.org", 587))
                using (SmtpClient smtp = new SmtpClient())
                {
                    // smtp.Credentials = new NetworkCredential("admin@manvfatfootball.org", "Email@manvfat123");
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.EnableSsl = false;
                    if (sys.EmailsEnabled)
                        foreach (var item in EmailAddresses)
                        {
                            if (item != "")
                            {
                                MailMessage mm;
                                MailAddress from = new MailAddress(From);
                                //(1) Create the MailMessage instance
                                if (sys.SendTempEmail)
                                {
                                    mm = new MailMessage(From, sys.TempEmailAddress, Subject, Body);
                                    mm.IsBodyHtml = true;
                                }
                                else
                                {
                                    mm = new MailMessage();

                                    mm.IsBodyHtml = true;
                                    mm.From = from;
                                    mm.Subject = Subject;
                                    mm.Body = Body;
                                }

                                mm.To.Add(item);

                                smtp.Send(mm);
                            }
                        }

                    thread.Join(0);
                }
            }
            catch (ThreadAbortException e)
            {
                SecurityUtils.AddAuditLog("The Email Thread Aborted Successfully", "Email Subject \"" + Subject + "\" has been Sent to " + To);
            }
            catch (Exception ex)
            {
                status = false;
                EmailErrorMsg = ex.Message + " - " + ex.InnerException != null ? ex.InnerException.Message : "";
                SecurityUtils.AddAuditLog("Failed to send Email", "Email Send Failed with Error = " + EmailErrorMsg + "<br/> Stack Trace:<br/>" + ex.StackTrace);
            }
            finally
            {
                SecurityUtils.AddAuditLog("The Email Thread Completed Successfully", "Email Subject \"" + Subject + "\" has been Sent to " + To);
            }

            return status;
        }

        public Boolean SendEmailWithAttachment(string From, string To, string Subject, string Body, string filePath, Thread thread, Controller ctrl, string BCC = null, string CC = null, bool DeleteAttchedFile = false)
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();

            string[] EmailAddresses = To.Replace(" ", string.Empty).Split(';');

            bool status = true;

            try
            {
                // using (SmtpClient smtp = new SmtpClient("mail.manvfatfootball.org", 587))
                using (SmtpClient smtp = new SmtpClient())
                {
                    // smtp.Credentials = new NetworkCredential("admin@manvfatfootball.org", "Email@manvfat123");
                    smtp.DeliveryMethod = SmtpDeliveryMethod.Network;
                    smtp.EnableSsl = false;
                    if (sys.EmailsEnabled)
                        foreach (var item in EmailAddresses)
                        {
                            if (item != "")
                            {
                                MailMessage mm;
                                MailAddress from = new MailAddress(From);
                                //(1) Create the MailMessage instance
                                if (sys.SendTempEmail)
                                {
                                    mm = new MailMessage(From, sys.TempEmailAddress, Subject, Body);
                                    mm.IsBodyHtml = true;
                                }
                                else
                                {
                                    mm = new MailMessage();

                                    mm.IsBodyHtml = true;
                                    mm.From = from;
                                    mm.Subject = Subject;
                                    mm.Body = Body;
                                }

                                mm.To.Add(item);

                                if (!string.IsNullOrEmpty(BCC))
                                {
                                    string[] EmailAddresses_BCC = BCC.Replace(" ", string.Empty).Split(';');

                                    foreach (var item_BCC in EmailAddresses_BCC)
                                    {
                                        if (!string.IsNullOrEmpty(item_BCC))
                                        {
                                            mm.Bcc.Add(item_BCC);
                                        }
                                    }
                                }

                                if (!string.IsNullOrEmpty(CC))
                                {
                                    string[] EmailAddresses_CC = CC.Replace(" ", string.Empty).Split(';');

                                    foreach (var item_CC in EmailAddresses_CC)
                                    {
                                        if (!string.IsNullOrEmpty(item_CC))
                                            mm.CC.Add(item_CC);
                                    }
                                }

                                System.Net.Mail.Attachment attachment;
                                attachment = new System.Net.Mail.Attachment(filePath);
                                mm.Attachments.Add(attachment);

                                smtp.Send(mm);
                            }
                        }
                    thread.Join(0);
                }
            }
            catch (ThreadAbortException e)
            {
                SecurityUtils.AddAuditLog("The Email Thread Aborted Successfully", "Email Subject \"" + Subject + "\" has been Sent to " + To);
            }
            catch (Exception ex)
            {
                status = false;
                EmailErrorMsg = ex.Message + " - " + ex.InnerException != null ? ex.InnerException.Message : "";
                SecurityUtils.AddAuditLog("Failed to send Email", "Email Send Failed with Error = " + EmailErrorMsg + "<br/> Stack Trace:<br/>" + ex.StackTrace);
            }
            finally
            {
                SecurityUtils.AddAuditLog("The Email Thread Completed Successfully", "Email Subject \"" + Subject + "\" has been Sent to " + To);

                //If Deleted tru then Delete the file after sending Email.
                if(DeleteAttchedFile)
                {
                    if(System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }

            return status;
        }

        public bool IsValidEmail(string email)
        {
            try
            {
                var addr = new System.Net.Mail.MailAddress(email);
                return true;
            }
            catch
            {
                return false;
            }
        }
    }
}