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
    public class DashboardNotificationsRepository : BaseRepository
    {
        public List<DashboardNotificationExt> ReadAll()
        {
            return db.DashboardNotifications.Where(m=>m.IsAchievementNotification==false).OrderByDescending(o => o.NotificationDateTime).ToList().Select(m => Map(m)).ToList();
        }

        public DashboardNotificationExt ReadOne(long DashboardNotificationID)
        {
            var model = db.DashboardNotifications.Where(m => m.DashboardNotificationID == DashboardNotificationID).FirstOrDefault();
            return Map(model);
        }

        public void DeleteDismissedNotifications()
        {
            try
            {
                db.DeleteDismissedDashboardNotifications();
                SecurityUtils.AddAuditLog("DeleteDismissedDashboardNotifications", "Delete Dismissed Dashboard Notifications Successfully");
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
            }
        }

        public void CreateRecurringNotifications(Controller ctrl)
        {
            try
            {

           
            var recurringNotifications = db.DashboardNotifications.Where(m => m.IsRecurring).ToList();

            foreach (var item in recurringNotifications)
            {
                if(item.RecurringFrequency==(int)enumShareFrequency.Daily)
                {
                    AddPlayerDashboardNotifications(item.DashboardNotificationID, item.LeagueID);
                }
                else if(item.RecurringFrequency==(int)enumShareFrequency.Weekly)
                {
                    //if recurring Frequency is Weekly and selected Day of Week is Today then Repeat the Notification
                    if(item.DayOfWeek==(int)DateTime.Now.DayOfWeek)
                    {
                        AddPlayerDashboardNotifications(item.DashboardNotificationID, item.LeagueID);
                    }
                }
                else if (item.RecurringFrequency == (int)enumShareFrequency.Monthly)
                {
                    //if recurring Frequency is Monthly and Today is 1st of the Month then Repeat the Notification
                    if (DateTime.Now.Date.Day==1)
                    {
                        AddPlayerDashboardNotifications(item.DashboardNotificationID, item.LeagueID);
                    }
                }
            }
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
            }

        }

        public bool CreateOrUpdate(DashboardNotificationExt model, ref string Msg, Controller ctrl)
        {
            bool status = true;

            if (model.DashboardNotificationID == 0)
            {
                try
                {
                    //TODO: Map to DB Object
                    var dbmodel = Map(model);
                    //TODO: Save DB Changes and Set the Return Primary Key ID
                    db.DashboardNotifications.Add(dbmodel);
                    db.SaveChanges();

                    //Now Add Same notification for Players Dashboard Notifications
                    AddPlayerDashboardNotifications(dbmodel.DashboardNotificationID, dbmodel.LeagueID);

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
                    var dbmodel = db.DashboardNotifications.FirstOrDefault(p => p.DashboardNotificationID == model.DashboardNotificationID);
                    var ForAuditLog = Map(dbmodel);
                    //TODO: Map to DB Object
                    MapUpdate(ref dbmodel, model);
                    //TODO: Update DB Changes
                    db.SaveChanges();

                    //Now Remove and Then Add Same notification for Players Dashboard Notifications
                    AddPlayerDashboardNotifications(dbmodel.DashboardNotificationID, dbmodel.LeagueID);

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

        public void AddPlayerDashboardNotifications(long DashboardNotificationID, long? LeagueID)
        {
            List<long?> LeaguePlayerIDs = new List<long?>();
            string LeaguePlayersStr = "";

            //if (LeagueID.HasValue)
            //{
            //    PlayerWeightWeeksRepository pwwRepo = new PlayerWeightWeeksRepository();
            //    LeaguePlayerIDs = pwwRepo.ReadAll_PlayerIDs_ByLeague(LeagueID.Value);

            //    LeaguePlayersStr = string.Join(",", LeaguePlayerIDs);
            //}

            //if(string.IsNullOrEmpty(LeaguePlayersStr))
            //{
                db.InsertDashboardNotificationforPlayers(DashboardNotificationID, null);
            //}
            //else
            //{
            //    db.InsertDashboardNotificationforPlayers(DashboardNotificationID, LeaguePlayersStr);
            //}

        }

        public bool Delete(DashboardNotificationExt model, ref string Msg, Controller ctrl)
        {
            bool status = true;

            //TODO: Get Current Object from DB
            var ItemToDelete = db.DashboardNotifications.FirstOrDefault(m => m.DashboardNotificationID == model.DashboardNotificationID);
            try
            {
                if (ItemToDelete != null)
                {

                    var ForAudiLog = Map(ItemToDelete);
                    //TODO: Check if it is not null, then Remove from DB
                    db.DeleteDashboardNotification(ItemToDelete.DashboardNotificationID);

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



        public DashboardNotifications Map(DashboardNotificationExt model)
        {
            DashboardNotifications tblModel = new DashboardNotifications()
            {
                DashboardNotificationID = model.DashboardNotificationID,
                Title = model.Title,
                Summary = model.Summary,
                Link = model.Link,
                LeagueID = model.LeagueID,
                NotificationDateTime = model.NotificationDateTime,
                IsRecurring = model.IsRecurring,
                RecurringFrequency = model.RecurringFrequency,
                DayOfWeek = model.DayOfWeek,
                IsAchievementNotification = model.IsAchievementNotification

            };

            return tblModel;
        }

        public DashboardNotificationExt Map(DashboardNotifications model)
        {

            DashboardNotificationExt tblModel = new DashboardNotificationExt()
            {
                DashboardNotificationID = model.DashboardNotificationID,
                Title = model.Title,
                Summary = model.Summary,
                Link = model.Link,
                LeagueID = model.LeagueID,

                NotificationDateTime = model.NotificationDateTime,
                IsRecurring = model.IsRecurring,
                RecurringFrequency = model.RecurringFrequency,
                DayOfWeek = model.DayOfWeek,
                IsAchievementNotification = model.IsAchievementNotification
            };

            if(tblModel.RecurringFrequency.HasValue)
            {
                switch (tblModel.RecurringFrequency.Value)
                {
                    case 1:
                        tblModel.RecurringFrequency_Str = "Daily";
                        break;
                    case 2:
                        tblModel.RecurringFrequency_Str = "Weekly";
                        break;
                    case 3:
                        tblModel.RecurringFrequency_Str = "Monthly";
                        break;
                    default:
                        tblModel.RecurringFrequency_Str = "";
                        break;
                }
                
            }
            else
            {
                tblModel.RecurringFrequency_Str = "";
            }

            if (tblModel.DayOfWeek.HasValue)
            {
                switch (tblModel.DayOfWeek.Value)
                {
                    case 0:
                        tblModel.DayOfWeek_Str = "Sunday";
                        break;
                    case 1:
                        tblModel.DayOfWeek_Str = "Monday";
                        break;
                    case 2:
                        tblModel.DayOfWeek_Str = "Tuesday";
                        break;
                    case 3:
                        tblModel.DayOfWeek_Str = "Wednesday";
                        break;
                    case 4:
                        tblModel.DayOfWeek_Str = "Thursday";
                        break;
                    case 5:
                        tblModel.DayOfWeek_Str = "Friday";
                        break;
                    case 6:
                        tblModel.DayOfWeek_Str = "Saturday";
                        break;
                    default:
                        tblModel.DayOfWeek_Str = "";
                        break;
                }

            }
            else
            {
                tblModel.DayOfWeek_Str = "";
            }

            return tblModel;
        }

        public void MapUpdate(ref DashboardNotifications dbmodel, DashboardNotificationExt model)
        {
            dbmodel.DashboardNotificationID = model.DashboardNotificationID;
            dbmodel.Title = model.Title;
            dbmodel.Summary = model.Summary;
            dbmodel.Link = model.Link;
            dbmodel.LeagueID = model.LeagueID;
            dbmodel.NotificationDateTime = model.NotificationDateTime;
            dbmodel.IsRecurring = model.IsRecurring;
            dbmodel.RecurringFrequency = model.RecurringFrequency;
            dbmodel.DayOfWeek = model.DayOfWeek;
        }


        /// <summary>
        /// Add the Action to Audit Log
        /// </summary>
        /// <param name="model">The Object for which this Auditlog took place</param>
        /// <param name="Action">"Added New User OR Updated User Details OR Deleted User"</param>

        private void AuditLog(Controller ctrl, AuditAction auditAction, DashboardNotificationExt dbmodel = null, DashboardNotificationExt model = null, string AdditionalMsg = "")
        {
            string AuditLogShortDesc = "", AuditLogLongDesc = "";

            StringBuilder sb = new StringBuilder();

            if (auditAction == AuditAction.Create) //Creating new Record
            {
                AuditLogShortDesc = AuditLogLongDesc = "New Dashboard Notification has been Added " + dbmodel.Title + " Additional Msg: " + AdditionalMsg;
            }
            else if (auditAction == AuditAction.Update)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Dashboard Notification has been Updated " + model.Title + " Additional Msg: " + AdditionalMsg;
            }
            else if (auditAction == AuditAction.Delete)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Dashboard Notification has been Deleted " + model.Title + " Additional Msg: " + AdditionalMsg;
            }

            SecurityUtils.AddAuditLog(AuditLogShortDesc, AuditLogLongDesc, ctrl);
        }
    }

    public class DashboardNotificationExt
    {
        public long DashboardNotificationID { get; set; }
        [Required(ErrorMessage = "Title Required")]
        public string Title { get; set; }
        [Required(ErrorMessage = "Summary Required")]
        public string Summary { get; set; }
        public string Link { get; set; }
        public System.DateTime NotificationDateTime { get; set; }
        public Nullable<long> LeagueID { get; set; }
        public string LeagueName { get; set; }
        public bool IsRecurring { get; set; }
        public Nullable<int> RecurringFrequency { get; set; }
        public string RecurringFrequency_Str { get; set; }
        public Nullable<int> DayOfWeek { get; set; }
        public string DayOfWeek_Str { get; set; }
        public bool IsAchievementNotification { get; set; }
    }
}