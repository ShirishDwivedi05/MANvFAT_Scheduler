using MANvFAT_Football.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;


namespace MANvFAT_Football.Models.Repositories
{
    public class AuditLogRepository : BaseRepository
    {
        public AuditLogRepository()
            : base()
        {
        }

        //TODO: Uncomment the Following Code To Enable AuditLog Repository

        public IList<AuditLogsExt> searchResult(string _SdateFrom, string _SdateTo, string _Action, string _UserID)
        {
            DateTime dateFrom = DateTime.Now;
            DateTime dateTo = DateTime.Now;
            long? UserID = null;
            SecurityUtils.ConvertToDateTime(_SdateFrom, ref dateFrom);
            SecurityUtils.ConvertToDateTime(_SdateTo, ref dateTo);
            UserID = SecurityUtils.ConvertToLongOrNull(_UserID);

            var logs =
                        (from product in db.AuditLogs

                         join u in db.Users on product.UserID equals u.UserID into subMembers
                         from uMembers in subMembers.DefaultIfEmpty()

                         where

                         ((_SdateFrom == "" && _SdateFrom != null) || product.LogDate >= dateFrom)
                         && ((_SdateTo == "" && _SdateTo != null) || product.LogDate <= dateTo)
                         && ((_Action == "" && _Action != null) || product.AuditLogShortDesc.Contains(_Action))
                         && (UserID == null || product.UserID == UserID)
                         orderby product.LogDate descending
                         select new AuditLogsExt
                         {
                             AuditLogID = product.AuditLogID,
                             LogDate = product.LogDate,
                             UserID = product.UserID,
                             Username = (uMembers == null ? string.Empty : uMembers.FullName),
                             AuditLogShortDesc = product.AuditLogShortDesc,
                             AuditLogLongDesc = product.AuditLogLongDesc
                         });

            return logs.ToList<AuditLogsExt>();
        }

        public AuditLogsExt ReadOne(long AuditLogID)
        {
            var auditLogExt = (from m in db.AuditLogs
                               where m.AuditLogID == AuditLogID
                               select new AuditLogsExt
                               {
                                   AuditLogID = m.AuditLogID,
                                   LogDate = m.LogDate,
                                   AuditLogShortDesc = m.AuditLogShortDesc,
                                   AuditLogLongDesc = m.AuditLogLongDesc,
                                   UserID = m.UserID,
                                   Username = m.UserID.HasValue ? db.Users.FirstOrDefault(u => u.UserID == m.UserID).FullName : ""

                               }).FirstOrDefault();

            return auditLogExt;
        }

        public AuditLogRepository(DBEntities dbContext)
            : base(dbContext)
        {
        }

        public void AddAuditLog(AuditLogs log)
        {
            db.AuditLogs.Add(log);
            db.SaveChanges();
        }

        public void AddAuditLog_ToHistory(AuditLogs log)
        {
            db.Database.CommandTimeout = 300;
            db.InsertAuditLogToHistory(log.LogDate, log.AuditLogShortDesc, log.AuditLogLongDesc);
        }

    }

    public class AuditLogsExt
    {
        public long AuditLogID { get; set; }
        public System.DateTime LogDate { get; set; }
        public string AuditLogShortDesc { get; set; }
        public string AuditLogLongDesc { get; set; }
        public long? UserID { get; set; }
        public string Username { get; set; }
    }

    public class AuditLogSearch
    {
        public string SUserID { get; set; }

        public string StxtDateFrom { get; set; }
        public string StxtDateTo { get; set; }
        public string StxtAction { get; set; }

        public AuditLogSearch()
        {
            SUserID = "";
            StxtDateFrom = "";
            StxtDateTo = "";
            StxtAction = "";
        }
    }
}