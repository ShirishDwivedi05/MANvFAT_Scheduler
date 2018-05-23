using MANvFAT_Football.Helpers;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using System.Text;
using MANvFAT_Football.Models.Enumerations;
using System.ComponentModel;

namespace MANvFAT_Football.Models.Repositories
{
    public class EmailDomainRestrictionsRepository : BaseRepository
    {
        public List<EmailDomainRestrictionsExt> ReadAll()
        {
            return db.EmailDomainRestrictions.ToList().Select(m => Map(m)).ToList();
        }

        public EmailDomainRestrictionsExt ReadOne(long EmailDomainID)
        {
            var task = db.EmailDomainRestrictions.Where(m => m.EmailDomainID == EmailDomainID).FirstOrDefault();
            return Map(task);
        }

        public bool Create(EmailDomainRestrictionsExt model, ref string Msg, Controller ctrl)
        {
            bool status = true;
            try
            {
                //TODO: Map to DB Object
                var dbmodel = Map(model);
                //TODO: Save DB Changes and Set the Return Primary Key ID
                db.EmailDomainRestrictions.Add(dbmodel);
                db.SaveChanges();
                //TOD: Add to Audit Log
                AuditLog(ctrl, AuditAction.Create, model, null);
            }
            catch (Exception ex)
            {
                Msg = ErrorHandling.HandleException(ex);
                status = false;
            }

            return status;
        }

        public bool Update(EmailDomainRestrictionsExt model, ref string Msg, Controller ctrl)
        {
            //Wrap it all in a transaction
            bool status = true;
            try
            {
                var dbmodel = db.EmailDomainRestrictions.FirstOrDefault(p => p.EmailDomainID == model.EmailDomainID);
                var ForAuditLog = Map(dbmodel);
                //TODO: Map to DB Object
                MapUpdate(ref dbmodel, model);
                //TODO: Update DB Changes
                db.SaveChanges();

                //TOD: Add to Audit Log
                AuditLog(ctrl, AuditAction.Update, ForAuditLog, model);
            }
            catch (Exception ex)
            {
                Msg = ErrorHandling.HandleException(ex);
                status = false;
            }

            return status;
        }

        public bool Delete(EmailDomainRestrictionsExt model, ref string Msg, Controller ctrl)
        {
            bool status = true;

            //TODO: Get Current Object from DB
            var ItemToDelete = db.EmailDomainRestrictions.FirstOrDefault(m => m.EmailDomainID == model.EmailDomainID);
            try
            {
                if (ItemToDelete != null)
                {
                    var ForAudiLog = Map(ItemToDelete);
                    //TODO: Check if it is not null, then Remove from DB
                    db.EmailDomainRestrictions.Remove(ItemToDelete);
                    db.SaveChanges();

                    //Add To Log
                    AuditLog(ctrl, AuditAction.Delete, null, ForAudiLog);
                }
            }
            catch (Exception ex)
            {
                Msg = ErrorHandling.HandleException(ex);
                status = false;
            }
            return status;
        }

        

        public EmailDomainRestrictions Map(EmailDomainRestrictionsExt model)
        {
            EmailDomainRestrictions tblModel = new EmailDomainRestrictions()
            {
                EmailDomainID = model.EmailDomainID,
                EmailDomain = model.EmailDomain.Replace(" ", string.Empty).Trim()
            };

            return tblModel;
        }

        public EmailDomainRestrictionsExt Map(EmailDomainRestrictions model)
        {
            EmailDomainRestrictionsExt tblModel = new EmailDomainRestrictionsExt()
            {
                EmailDomainID = model.EmailDomainID,
                EmailDomain = model.EmailDomain
            };

            return tblModel;
        }

        public void MapUpdate(ref EmailDomainRestrictions dbmodel, EmailDomainRestrictionsExt model)
        {
            dbmodel.EmailDomainID = model.EmailDomainID;
            dbmodel.EmailDomain = model.EmailDomain.Replace(" ", string.Empty).Trim();
        }

        /// <summary>
        /// Add the Action to Audit Log
        /// </summary>
        /// <param name="model">The Object for which this Auditlog took place</param>
        /// <param name="Action">"Added New User OR Updated User Details OR Deleted User"</param>

        private void AuditLog(Controller ctrl, AuditAction auditAction, EmailDomainRestrictionsExt dbmodel = null, EmailDomainRestrictionsExt model = null)
        {
            string AuditLogShortDesc = "", AuditLogLongDesc = "";

            StringBuilder sb = new StringBuilder();

            if (auditAction == AuditAction.Create) //Creating new Record
            {
                AuditLogShortDesc = AuditLogLongDesc = "New Email Domain Restriction has been Added " + dbmodel.EmailDomain;
            }
            else if (auditAction == AuditAction.Update)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Email Domain Restriction has been Updated " + model.EmailDomain;
            }
            else if (auditAction == AuditAction.Delete)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Email Domain Restriction has been Deleted " + model.EmailDomain;
            }

            SecurityUtils.AddAuditLog(AuditLogShortDesc, AuditLogLongDesc, ctrl);
        }
    }

    public class EmailDomainRestrictionsExt
    {
        public long EmailDomainID { get; set; }
        [DisplayName("Email Domain")]
        [Required(ErrorMessage = "Please enter Email Domain")]
        public string EmailDomain { get; set; }
    }
}