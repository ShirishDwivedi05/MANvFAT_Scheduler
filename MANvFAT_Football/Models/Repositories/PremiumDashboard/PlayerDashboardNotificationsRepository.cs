using MANvFAT_Football.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace MANvFAT_Football.Models.Repositories
{
    public class PlayerDashboardNotificationsRepository : BaseRepository
    {
        public List<PlayerDashboardNotificationExt> ReadAll_NonDismissed(long PlayerID)
        {
            return db.PlayerDashboardNotifications.Include("DashboardNotifications").Where(m => m.PlayerID == PlayerID && m.Dismissed == false).OrderByDescending(o => o.DashboardNotifications.NotificationDateTime).ToList().Select(m => Map(m)).ToList();
        }

        public void DismissNotification(long DashboardNotificationID, long PlayerID)
        {
            var dbmodel = db.PlayerDashboardNotifications.FirstOrDefault(m => m.PlayerID == PlayerID && m.DashboardNotificationID == DashboardNotificationID);

            if (dbmodel != null)
            {
                dbmodel.Dismissed = true;
                dbmodel.DismissedDateTime = DateTime.Now;

                db.SaveChanges();

                var ForAuditLog = Map(db.PlayerDashboardNotifications.Include("DashboardNotifications").FirstOrDefault(m => m.PlayerID == PlayerID && m.DashboardNotificationID == DashboardNotificationID));


                SecurityUtils.AddAuditLog("Player Dismiss Notification ", "Title = " + ForAuditLog.Title + " PlayerID = " + ForAuditLog.PlayerID);
            }
        }

        public PlayerDashboardNotificationExt Map(PlayerDashboardNotifications m)
        {
            PlayerDashboardNotificationExt model = new PlayerDashboardNotificationExt()
            {
                PlayerDashboardNotificationID = m.PlayerDashboardNotificationID,
                DashboardNotificationID = m.DashboardNotificationID,
                PlayerID = m.PlayerID,
                Title = m.DashboardNotifications.Title,
                Summary = m.DashboardNotifications.Summary,
                Link = m.DashboardNotifications.Link
            };

            return model;
        }
    }

    public class PlayerDashboardNotificationExt : DashboardNotificationExt
    {
        public long PlayerDashboardNotificationID { get; set; }
        public long PlayerID { get; set; }
        public bool Dismissed { get; set; }
        public System.DateTime DismissedDateTime { get; set; }
    }
}