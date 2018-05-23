using MANvFAT_Football.Helpers;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace MANvFAT_Football.Models.Repositories
{
    public class SystemSettingsRepository : BaseRepository
    {
        #region System Settings

        public SystemSettingsExt GetSystemSettings()
        {
            var mSysSettings = db.SystemSettings.FirstOrDefault();
            var merchantExt = Map(mSysSettings);
            return merchantExt;
        }

        public bool Update(SystemSettingsExt model, ref string Msg, ref string MsgCss, Controller ctrl)
        {
            bool status = true;

            try
            {
                //TODO: Map to DB Object
                var dbmodel = db.SystemSettings.FirstOrDefault(m => m.SystemSettingID == model.SystemSettingID);
                //TODO: Update DB Changes
                MapForUpdate(ref dbmodel, model);

                SaveDBChanges();
                //TOD: Add to Audit Log
                AuditLog("Updated System Settings", ctrl);
                //To get here, everything must be OK, so commit the transaction
                Msg = "System Settings updated Successfully";
                MsgCss = "alert alert-success";
            }
            catch (System.Exception ex)
            {
                Msg = "Unexpected Error occurred: Error = " + ex.Message;
                MsgCss = "alert alert-danger";
            }

            return status;
        }

        private SystemSettingsExt Map(SystemSettings model)
        {
            SystemSettingsExt m = new SystemSettingsExt()
            {
                SystemSettingID = model.SystemSettingID,

                EmailsEnabled = model.EmailsEnabled,
                SendTempEmail = model.SendTempEmail,
                SupportEmailAddress = model.SupportEmailAddress,
                TempEmailAddress = model.TempEmailAddress,
                CurrentDomain = model.CurrentDomain,
                AdminEmailAddress = model.AdminEmailAddress,

                GoCardless_Mode = model.GoCardless_Mode,
                GoCardless_APIUrlLive = model.GoCardless_APIUrlLive,
                GoCardless_TokenUrlLive = model.GoCardless_TokenUrlLive,
                GoCardless_ClientIDLive = model.GoCardless_ClientIDLive,
                GoCardless_ClientSecretLive = model.GoCardless_ClientSecretLive,
                GoCardless_TokenLive = model.GoCardless_TokenLive,

                GoCardless_APIUrlSandbox = model.GoCardless_APIUrlSandbox,
                GoCardless_TokenUrlSandbox = model.GoCardless_TokenUrlSandbox,
                GoCardless_ClientIDSandbox = model.GoCardless_ClientIDSandbox,
                GoCardless_ClientSecretSandbox = model.GoCardless_ClientSecretSandbox,
                GoCardless_TokenSandbox = model.GoCardless_TokenSandbox,
                GoCardless_WebhookIDSandbox = model.GoCardless_WebhookIDSandbox,
                GoCardless_WebhookIDLive = model.GoCardless_WebhookIDLive,

                RegFeeWithBook = model.RegFeeWithBook,
                RegFeeWithoutBook = model.RegFeeWithoutBook,
                PayPal_Mode = model.PayPal_Mode,
                PayPalServerURL_Live = model.PayPalServerURL_Live,
                BusinessEmail_Live = model.BusinessEmail_Live,
                NotifyURL_IPN_Live = model.NotifyURL_IPN_Live,
                ReturnURL_Live = model.ReturnURL_Live,
                PayPalServerURL_SandBox = model.PayPalServerURL_SandBox,
                BusinessEmail_SandBox = model.BusinessEmail_SandBox,
                NotifyURL_IPN_SandBox = model.NotifyURL_IPN_SandBox,
                ReturnURL_SandBox = model.ReturnURL_SandBox,
                ResetLeagueWeightLoss = model.ResetLeagueWeightLoss,
                DownForMaintenance = model.DownForMaintenance,
                DownForMaintenance_Timer = model.DownForMaintenance_Timer,
                ReferralCandy_AccessID = model.ReferralCandy_AccessID,
                ReferralCandy_SecreteKey = model.ReferralCandy_SecreteKey,
                PremiumDashboardFee = model.PremiumDashboardFee,
                Enable_MobileVerification = model.Enable_MobileVerification
            };

            return m;
        }

        private void MapForUpdate(ref SystemSettings dbmodel, SystemSettingsExt model)
        {
            dbmodel.SystemSettingID = model.SystemSettingID;

            dbmodel.EmailsEnabled = model.EmailsEnabled;
            dbmodel.SendTempEmail = model.SendTempEmail;
            dbmodel.SupportEmailAddress = model.SupportEmailAddress;
            dbmodel.TempEmailAddress = model.TempEmailAddress;
            dbmodel.CurrentDomain = model.CurrentDomain;
            dbmodel.AdminEmailAddress = model.AdminEmailAddress;

            dbmodel.GoCardless_Mode = model.GoCardless_Mode;
            dbmodel.GoCardless_APIUrlLive = model.GoCardless_APIUrlLive;
            dbmodel.GoCardless_TokenUrlLive = model.GoCardless_TokenUrlLive;
            dbmodel.GoCardless_ClientIDLive = model.GoCardless_ClientIDLive;
            dbmodel.GoCardless_ClientSecretLive = model.GoCardless_ClientSecretLive;
            dbmodel.GoCardless_APIUrlSandbox = model.GoCardless_APIUrlSandbox;
            dbmodel.GoCardless_TokenUrlSandbox = model.GoCardless_TokenUrlSandbox;
            dbmodel.GoCardless_ClientIDSandbox = model.GoCardless_ClientIDSandbox;
            dbmodel.GoCardless_ClientSecretSandbox = model.GoCardless_ClientSecretSandbox;
            dbmodel.GoCardless_TokenLive = model.GoCardless_TokenLive;
            dbmodel.GoCardless_TokenSandbox = model.GoCardless_TokenSandbox;
            dbmodel.GoCardless_WebhookIDSandbox = model.GoCardless_WebhookIDSandbox;
            dbmodel.GoCardless_WebhookIDLive = model.GoCardless_WebhookIDLive;

            dbmodel.RegFeeWithBook = model.RegFeeWithBook;
            dbmodel.RegFeeWithoutBook = model.RegFeeWithoutBook;
            dbmodel.PremiumDashboardFee = model.PremiumDashboardFee;

            dbmodel.PayPal_Mode = model.PayPal_Mode;
            dbmodel.PayPalServerURL_Live = model.PayPalServerURL_Live;
            dbmodel.BusinessEmail_Live = model.BusinessEmail_Live;
            dbmodel.NotifyURL_IPN_Live = model.NotifyURL_IPN_Live;
            dbmodel.ReturnURL_Live = model.ReturnURL_Live;
            dbmodel.PayPalServerURL_SandBox = model.PayPalServerURL_SandBox;
            dbmodel.BusinessEmail_SandBox = model.BusinessEmail_SandBox;
            dbmodel.NotifyURL_IPN_SandBox = model.NotifyURL_IPN_SandBox;
            dbmodel.ReturnURL_SandBox = model.ReturnURL_SandBox;

            dbmodel.ResetLeagueWeightLoss = model.ResetLeagueWeightLoss;

            dbmodel.DownForMaintenance = model.DownForMaintenance;
            dbmodel.DownForMaintenance_Timer = model.DownForMaintenance_Timer;

            dbmodel.ReferralCandy_AccessID = model.ReferralCandy_AccessID;
            dbmodel.ReferralCandy_SecreteKey = model.ReferralCandy_SecreteKey;
        }

        private void AuditLog(string Action, Controller ctrl)
        {
            SecurityUtils.AddAuditLog(Action, "", ctrl);
        }

        #endregion System Settings

        #region System Message

        public SystemMessages ReadSystemMessage()
        {
            var model = db.SystemSettings.FirstOrDefault();

            SystemMessages sysMsg = new SystemMessages()
            {
                SystemMessage = model.SystemMessage,
                SystemMessageSubject = model.SystemMessageSubject
            };

            return sysMsg;
        }

        public List<HistoricalSystemMessagesExt> ReadHistoricalSystemMessage()
        {
            var sysMsg = db.SystemMessages.Include("Users").ToList().Select(m=>Map(m));
            return sysMsg.OrderByDescending(o=>o.MessageDateTime).ToList();
        }


        public void UpdateSystemMessage(string SysMsg, string SysMsgSubject, ref string Msg, ref string MsgCss, Controller ctrl)
        {
            try
            {
                var dbmodel = db.SystemSettings.FirstOrDefault();
                dbmodel.SystemMessageSubject = SysMsgSubject;
                dbmodel.SystemMessage = SysMsg;

                db.SaveChanges();


                var LoggedinUser = SecurityUtils.GetUserDetails();
                //When System Message Added/Updated add it to System Message and assign to each coach in the system

                //SystemMessagesRepository systemMessagesRepo = new SystemMessagesRepository();
                //systemMessagesRepo.Create(SysMsg, SysMsgSubject, LoggedinUser.UserID);

                AuditLog("System Message Updated", ctrl);
                Msg = "System Message updated Successfully";
                MsgCss = "alert alert-success";
            }
            catch (System.Exception ex)
            {
                ErrorHandling.HandleException(ex);
                Msg = "Unexpected Error occurred: Error = " + ex.Message;
                MsgCss = "alert alert-danger";
            }
        }

        public HistoricalSystemMessagesExt Map(Models.SystemMessages m)
        {
            HistoricalSystemMessagesExt model = new HistoricalSystemMessagesExt()
            {
                SystemMessageID = m.SystemMessageID,
                SystemMessage = m.SystemMessage,
                SystemMessageSubject = m.SystemMessageSubject,
                MessageDateTime = m.MessageDateTime,
                CreatedByUserID = m.CreatedByUserID,
                CreatedByUserFullName = m.Users.FullName
            };

            return model;
        }

        #endregion System Message
    }

    public class SystemSettingsExt
    {
        public long SystemSettingID { get; set; }

        [DisplayName("Send Emails?")]
        public bool EmailsEnabled { get; set; }

        [DisplayName("Send Temp Emails?")]
        public bool SendTempEmail { get; set; }

        [DisplayName("Support Email Address")]
        [Required(ErrorMessage = "Please enter Support Email Address")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter valid email address.")]
        public string SupportEmailAddress { get; set; }

        [DisplayName("Temp Email Address")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter valid email address.")]
        [Required(ErrorMessage = "Please enter Temp Email Address")]
        public string TempEmailAddress { get; set; }

        [DisplayName("Current Domain")]
        [DataType(DataType.Url)]
        [Required(ErrorMessage = "Please enter CurrentDomain")]
        public string CurrentDomain { get; set; }

        [DisplayName("Admin Email Address")]
        [DataType(DataType.EmailAddress)]
        [EmailAddress(ErrorMessage = "Please enter valid email address.")]
        [Required(ErrorMessage = "Please enter Admin Email Address")]
        public string AdminEmailAddress { get; set; }

        [DisplayName("Mode")]
        public string GoCardless_Mode { get; set; }

        [DisplayName("API URL")]
        public string GoCardless_APIUrlLive { get; set; }

        [DisplayName("Token URL")]
        public string GoCardless_TokenUrlLive { get; set; }

        [DisplayName("Client SystemSettingID")]
        public string GoCardless_ClientIDLive { get; set; }

        [DisplayName("Client Secret")]
        public string GoCardless_ClientSecretLive { get; set; }

        [DisplayName("API URL")]
        public string GoCardless_APIUrlSandbox { get; set; }

        [DisplayName("Token URL")]
        public string GoCardless_TokenUrlSandbox { get; set; }

        [DisplayName("Client SystemSettingID")]
        public string GoCardless_ClientIDSandbox { get; set; }

        [DisplayName("Client Secret")]
        public string GoCardless_ClientSecretSandbox { get; set; }

        [DisplayName("Token")]
        public string GoCardless_TokenLive { get; set; }

        [DisplayName("Token")]
        public string GoCardless_TokenSandbox { get; set; }

        [Required(ErrorMessage = "Please enter Fee With Book")]
        [DisplayName("Reg. Fee With Book")]
        public decimal RegFeeWithBook { get; set; }

        [Required(ErrorMessage = "Please enter Fee Without Book")]
        [DisplayName("Reg. Fee Without Book")]
        public decimal RegFeeWithoutBook { get; set; }

        public SelectList PaymentModes { get; set; }

        [DisplayName("PayPal Mode")]
        public string PayPal_Mode { get; set; }

        [DisplayName("PayPal Server URL")]
        public string PayPalServerURL_Live { get; set; }

        [DisplayName("PayPal Email")]
        public string BusinessEmail_Live { get; set; }

        [DisplayName("Notify URL IPN")]
        public string NotifyURL_IPN_Live { get; set; }

        [DisplayName("Return URL")]
        public string ReturnURL_Live { get; set; }

        [DisplayName("PayPal Server URL")]
        public string PayPalServerURL_SandBox { get; set; }

        [DisplayName("PayPal Email")]
        public string BusinessEmail_SandBox { get; set; }

        [DisplayName("Notify URL IPN")]
        public string NotifyURL_IPN_SandBox { get; set; }

        [DisplayName("Return URL")]
        public string ReturnURL_SandBox { get; set; }

        public decimal ResetLeagueWeightLoss { get; set; }

        [DisplayName("Webhook ID")]
        public string GoCardless_WebhookIDSandbox { get; set; }

        [DisplayName("Webhook ID")]
        public string GoCardless_WebhookIDLive { get; set; }

        [DisplayName("Down For Maintenance?")]
        public bool DownForMaintenance { get; set; }

        [DisplayName("Down For Maintenance Timer")]
        public string DownForMaintenance_Timer { get; set; }

        [Required(ErrorMessage = "Please enter Access ID")]
        [DisplayName("Access ID")]
        public string ReferralCandy_AccessID { get; set; }

        [Required(ErrorMessage = "Please enter Secrete Key")]
        [DisplayName("Secrete Key")]
        public string ReferralCandy_SecreteKey { get; set; }
        [Required(ErrorMessage = "Please enter Premium Dashboard Fee")]
        [DisplayName("Premium Dashboard Fee")]
        public decimal PremiumDashboardFee { get; set; }
        public bool Enable_MobileVerification { get; set; }
    }

    public class SystemMessages
    {
        [Required(ErrorMessage ="Please enter Message")]
        [AllowHtml]
        public string SystemMessage { get; set; }
        [Required(ErrorMessage = "Please enter Subject")]
        public string SystemMessageSubject { get; set; }
    }

    public class HistoricalSystemMessagesExt
    {
        public long SystemMessageID { get; set; }
        public DateTime MessageDateTime { get; set; }
        public string SystemMessage { get; set; }
        public string SystemMessageSubject { get; set; }
        public long CreatedByUserID { get; set; }
        public string CreatedByUserFullName { get; set; }
    }
    public class RegFees
    {
        public string RegFeeWithBook { get; set; }
        public string RegFeeWithoutBook { get; set; }
    }
}