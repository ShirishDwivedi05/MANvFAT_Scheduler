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
    public class MemberToolsBenefitsRepository :BaseRepository
    {
   


  

       

      
       

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
        [Required(ErrorMessage ="Title Required")]
        public string Title { get; set; }
        [Required(ErrorMessage ="Summary Required")]
        public string Summary { get; set; }
        public string PictureName { get; set; }
        public string PictureURL { get; set; }
        public string Link { get; set; }
        public Nullable<long> LeagueID { get; set; }
        public string LeagueName { get; set; }
        [DisplayName("Sort Order")]
        public int SortOrder { get; set; }
        [DisplayName("Is Tool?")]
        public bool IsTool { get; set; }
        [DisplayName("Is Benefit?")]
        public bool IsBenefit { get; set; }
    }
}