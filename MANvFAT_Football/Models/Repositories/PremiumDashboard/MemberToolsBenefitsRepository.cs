using MANvFAT_Football.Helpers;
using System.Collections.Generic;
using System;
using System.ComponentModel.DataAnnotations;
using System.Web.Mvc;
using System.Linq;
using System.Text;
using MANvFAT_Football.Models.Enumerations;
using System.ComponentModel;
using System.IO;

namespace MANvFAT_Football.Models.Repositories
{
    public class MemberToolsBenefitsRepository : BaseRepository
    {
        public List<MemberToolsBenefitsExt> ReadAll()
        {
            return db.MemberToolsBenefits.OrderBy(o => o.SortOrder).ToList().Select(m => Map(m)).ToList();
        }

        public MemberToolsBenefitsExt ReadOne(long MemberToolsBenefitID)
        {
            var model = db.MemberToolsBenefits.Where(m => m.MemberToolsBenefitID == MemberToolsBenefitID).FirstOrDefault();
            return Map(model);
        }

        public List<MemberToolsBenefitsExt> ReadAll_PremiumDashboard(long PlayerID, bool GetTools, bool GetBenefits)
        {
            List<MemberToolsBenefitsExt> result = new List<MemberToolsBenefitsExt>();

       
                result = db.MemberToolsBenefits
                            .Where(m => m.IsTool == GetTools && m.IsBenefit == GetBenefits)
                            .OrderBy(o => o.SortOrder).ToList().Select(m => Map(m)).ToList();
            
            return result;
        }

        public bool CreateOrUpdate(MemberToolsBenefitsExt model, ref string Msg, Controller ctrl)
        {
            bool status = true;

            if (model.MemberToolsBenefitID == 0)
            {
                try
                {
                    //TODO: Map to DB Object
                    var dbmodel = Map(model);
                    //TODO: Save DB Changes and Set the Return Primary Key ID
                    db.MemberToolsBenefits.Add(dbmodel);
                    db.SaveChanges();
                    //TOD: Add to Audit Log
                    AuditLog(ctrl, AuditAction.Create, model, null);
                }
                catch (Exception ex)
                {
                    Msg = ErrorHandling.HandleException(ex);
                    status = false;
                }
            }
            else
            {
                try
                {
                    var dbmodel = db.MemberToolsBenefits.FirstOrDefault(p => p.MemberToolsBenefitID == model.MemberToolsBenefitID);
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
            }

            return status;
        }

        public bool AddToolBenefitImage(long MemberToolsBenefitID, string PictureName, ref string Msg, Controller ctrl)
        {
            bool status = true;

            var model = db.MemberToolsBenefits.FirstOrDefault(m => m.MemberToolsBenefitID == MemberToolsBenefitID);
            try
            {
                if (model != null)
                {

                    model.PictureName = PictureName;
                    db.SaveChanges();

                    //Add To Log
                    SecurityUtils.AddAuditLog("Member Tool Benefits", "New Image Added MemberToolsBenefitID = " + MemberToolsBenefitID + " PictureName = " + PictureName, ctrl);
                }
            }
            catch (Exception ex)
            {
                Msg = ErrorHandling.HandleException(ex);
                status = false;
            }
            return status;
        }


        public bool Delete(MemberToolsBenefitsExt model, ref string Msg, string ToolBenefit_ImageFolder, Controller ctrl)
        {
            bool status = true;

            //TODO: Get Current Object from DB
            var ItemToDelete = db.MemberToolsBenefits.FirstOrDefault(m => m.MemberToolsBenefitID == model.MemberToolsBenefitID);
            try
            {
                if (ItemToDelete != null)
                {

                    var ForAudiLog = Map(ItemToDelete);
                    //TODO: Check if it is not null, then Remove from DB
                    db.MemberToolsBenefits.Remove(ItemToDelete);
                    db.SaveChanges();


                    if (!string.IsNullOrEmpty(ForAudiLog.PictureName))
                    {
                        string PrevImgPath = Path.Combine(ToolBenefit_ImageFolder, ForAudiLog.PictureName);

                        if (System.IO.File.Exists(PrevImgPath))
                        {
                            System.IO.File.Delete(PrevImgPath);
                        }
                    }

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



        public MemberToolsBenefits Map(MemberToolsBenefitsExt model)
        {
            MemberToolsBenefits tblModel = new MemberToolsBenefits()
            {
                MemberToolsBenefitID = model.MemberToolsBenefitID,
                Title = model.Title,
                Summary = model.Summary,
                PictureName = model.PictureName,
                Link = model.Link,
                IsTool = model.IsTool,
                IsBenefit = model.IsBenefit,
                SortOrder = model.SortOrder

            };

            return tblModel;
        }

        public MemberToolsBenefitsExt Map(MemberToolsBenefits model)
        {

            MemberToolsBenefitsExt tblModel = new MemberToolsBenefitsExt()
            {
                MemberToolsBenefitID = model.MemberToolsBenefitID,
                Title = model.Title,
                Summary = model.Summary,
                PictureName = model.PictureName,
                PictureURL = string.IsNullOrEmpty(model.PictureName) == false ? SecurityUtils.ToolBenefit_ImagePath + "/" + model.PictureName : "",
                Link = model.Link,
                IsTool = model.IsTool,
                IsBenefit = model.IsBenefit,
                SortOrder = model.SortOrder
            };

            return tblModel;
        }

        public void MapUpdate(ref MemberToolsBenefits dbmodel, MemberToolsBenefitsExt model)
        {
            dbmodel.MemberToolsBenefitID = model.MemberToolsBenefitID;
            dbmodel.Title = model.Title;
            dbmodel.Summary = model.Summary;
            dbmodel.Link = model.Link;
            dbmodel.IsTool = model.IsTool;
            dbmodel.IsBenefit = model.IsBenefit;
            dbmodel.SortOrder = model.SortOrder;
        }


        /// <summary>
        /// Add the Action to Audit Log
        /// </summary>
        /// <param name="model">The Object for which this Auditlog took place</param>
        /// <param name="Action">"Added New User OR Updated User Details OR Deleted User"</param>

        private void AuditLog(Controller ctrl, AuditAction auditAction, MemberToolsBenefitsExt dbmodel = null, MemberToolsBenefitsExt model = null, string AdditionalMsg = "")
        {
            string AuditLogShortDesc = "", AuditLogLongDesc = "";

            StringBuilder sb = new StringBuilder();

            if (auditAction == AuditAction.Create) //Creating new Record
            {
                AuditLogShortDesc = AuditLogLongDesc = "New Tool_Benefit has been Added " + dbmodel.Title + " Additional Msg: " + AdditionalMsg;
            }
            else if (auditAction == AuditAction.Update)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Tool_Benefit has been Updated " + model.Title + " Additional Msg: " + AdditionalMsg;
            }
            else if (auditAction == AuditAction.Delete)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Tool_Benefit has been Deleted " + model.Title + " Additional Msg: " + AdditionalMsg;
            }

            SecurityUtils.AddAuditLog(AuditLogShortDesc, AuditLogLongDesc, ctrl);
        }

    }

    public class MemberToolsBenefitsExt
    {
        public long MemberToolsBenefitID { get; set; }
        [Required(ErrorMessage = "Title Required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Summary Required")]
        public string Summary { get; set; }
        public string PictureName { get; set; }
        public string PictureURL { get; set; }
        public string Link { get; set; }
        [DisplayName("Sort Order")]
        public int SortOrder { get; set; }
        [DisplayName("Is Tool?")]
        public bool IsTool { get; set; }
        [DisplayName("Is Benefit?")]
        public bool IsBenefit { get; set; }
    }
}