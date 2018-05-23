
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
    public class HeaderTextsRepository : BaseRepository
    {
        public List<HeaderTextsExt> ReadAll()
        {
            return db.HeaderTexts.ToList().Select(m => Map(m)).ToList();
        }

        public HeaderTextsExt ReadOne(long HeaderTextId)
        {
            var task = db.HeaderTexts.Where(m => m.HeaderTextID == HeaderTextId).FirstOrDefault();
            return Map(task);
        }

        public bool Create(HeaderTextsExt model, ref string Msg, Controller ctrl)
        {
            bool status = true;
            try
            {
                //TODO: Map to DB Object
                var dbmodel = Map(model);
                //TODO: Save DB Changes and Set the Return Primary Key ID
                db.HeaderTexts.Add(dbmodel);
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

        public bool Update(HeaderTextsExt model, ref string Msg, Controller ctrl)
        {
            //Wrap it all in a transaction
            bool status = true;
            try
            {
                var dbmodel = db.HeaderTexts.FirstOrDefault(p => p.HeaderTextID == model.HeaderTextID);
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

        public bool Delete(HeaderTextsExt model, ref string Msg, Controller ctrl)
        {
            bool status = true;

            //TODO: Get Current Object from DB
            var ItemToDelete = db.HeaderTexts.FirstOrDefault(m => m.HeaderTextID == model.HeaderTextID);
            try
            {
                if (ItemToDelete != null)
                {
                    var ForAudiLog = Map(ItemToDelete);
                    //TODO: Check if it is not null, then Remove from DB
                    db.HeaderTexts.Remove(ItemToDelete);
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

        public HeaderTextsExt Map(HeaderTexts model)
        {
            HeaderTextsExt tblModel = new HeaderTextsExt()
            {
                HeaderTextID = model.HeaderTextID,
                HeaderText = model.HeaderText,
                Link = model.Link,
                Display = model.Display,
                DisplayDate = model.DisplayDate
            };

            return tblModel;
        }

        //public HeaderTextsExt Map(HeaderTextsExt model)
        //{
        //    HeaderTextsExt tblModel = new HeaderTextsExt()
        //    {
        //        HeaderTextID = model.HeaderTextID,
        //        HeaderText = model.HeaderText.Replace(" ", string.Empty).Trim(),
        //        Link=model.Link,
        //        Display=model.Display,
        //        DisplayDate=model.DisplayDate
        //    };

        //    return tblModel;
        //}
        public HeaderTexts Map(HeaderTextsExt model)
        {
            HeaderTexts tblModel = new HeaderTexts()
            {
                HeaderTextID = model.HeaderTextID,
                HeaderText = model.HeaderText,
                Link = model.Link,
                Display = model.Display,
                DisplayDate = model.DisplayDate
            };

            return tblModel;
        }

        public void MapUpdate(ref HeaderTexts dbmodel, HeaderTextsExt model)
        {
            dbmodel.HeaderTextID = model.HeaderTextID;
            dbmodel.HeaderText = model.HeaderText;
            dbmodel.Link = model.Link;
            dbmodel.Display = model.Display;
            dbmodel.DisplayDate = model.DisplayDate;
        }

        /// <summary>
        /// Add the Action to Audit Log
        /// </summary>
        /// <param name="model">The Object for which this Auditlog took place</param>
        /// <param name="Action">"Added New User OR Updated User Details OR Deleted User"</param>

        private void AuditLog(Controller ctrl, AuditAction auditAction, HeaderTextsExt dbmodel = null, HeaderTextsExt model = null)
        {
            string AuditLogShortDesc = "", AuditLogLongDesc = "";

            StringBuilder sb = new StringBuilder();

            if (auditAction == AuditAction.Create) //Creating new Record
            {
                AuditLogShortDesc = AuditLogLongDesc = "New header text has been Added " + dbmodel.HeaderText;
            }
            else if (auditAction == AuditAction.Update)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Header text has been Updated " + model.HeaderText;
            }
            else if (auditAction == AuditAction.Delete)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Header text has been Deleted " + model.HeaderText;
            }

            SecurityUtils.AddAuditLog(AuditLogShortDesc, AuditLogLongDesc, ctrl);
        }
    }

    public class HeaderTextsExt
    {
        public long HeaderTextID { get; set; }

        [DisplayName("Header Text")]
        [Required(ErrorMessage = "Please enter Header Text ")]
        public string HeaderText { get; set; }
        public string Link { get; set; }
        public DateTime? DisplayDate { get; set; }
        public bool Display { get; set; }

    }
}