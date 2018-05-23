using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;

namespace MANvFAT_Football.Models.Repositories
{
    public class PlayerWeeklyActivityRepository : BaseRepository
    {
        private Thread EmailThread;

        public List<vmWeeklyActivity> GetWeeklyActivityData(long PlayerID, DateTime ActivityDate, Controller ctrl)
        {
            var StartDateOfWeek = DateTimeExtensions.StartOfWeek(ActivityDate, DayOfWeek.Monday);
            var EndDateOfWeek = StartDateOfWeek.AddDays(6);

            List<vmWeeklyActivity> ListOfViewModel = new List<vmWeeklyActivity>();

            for (DateTime loopDate = StartDateOfWeek; loopDate <= EndDateOfWeek; loopDate = loopDate.AddDays(1))
            {
                var data = db.PlayerWeeklyActivities.Where(m => m.PlayerID == PlayerID && m.ActivityDate == loopDate).OrderBy(o => o.PlayerWeeklyActivityID);
                if (data.Count() == 0)
                {
                    //Add it into DB and Load in the GUI after getting PlayerWeeklyActivityID

                    PlayerWeeklyActivityExt m = new PlayerWeeklyActivityExt()
                    {
                        PlayerID = PlayerID,
                        ActivityDate = loopDate
                    };
                    string Msg = "";
                    bool status = true;

                    m.PlayerWeeklyActivityID = CreateOrUpdate(m, ref Msg, ref status, ctrl);

                    vmWeeklyActivity vmWeeklyAct = new vmWeeklyActivity()
                    {
                        PlayerWeeklyActivityID = m.PlayerWeeklyActivityID,
                        PlayerID = m.PlayerID,
                        ActivityDate = m.ActivityDate
                    };

                    PlayerWeeklyActivityExt mAct = new PlayerWeeklyActivityExt()
                    {
                        PlayerWeeklyActivityID = m.PlayerWeeklyActivityID,
                        ActivityDate = m.ActivityDate,
                        Activity = "",
                        ActivityTime = 0,
                        Completed = false
                    };

                    vmWeeklyAct.ListOfWeeklyActivities.Add(mAct);

                    ListOfViewModel.Add(vmWeeklyAct);
                }
                else
                {
                    foreach (var item in data)
                    {
                        if (ListOfViewModel.Any(m => m.ActivityDate == item.ActivityDate))
                        {
                            var model = ListOfViewModel.FirstOrDefault(m => m.ActivityDate == item.ActivityDate);

                            PlayerWeeklyActivityExt mAct = new PlayerWeeklyActivityExt()
                            {
                                PlayerWeeklyActivityID = item.PlayerWeeklyActivityID,
                                ActivityDate = item.ActivityDate,
                                Activity = item.Activity,
                                ActivityTime = item.ActivityTime,
                                Completed = item.Completed
                            };

                            model.ListOfWeeklyActivities.Add(mAct);
                        }
                        else
                        {
                            vmWeeklyActivity m = new vmWeeklyActivity()
                            {
                                PlayerWeeklyActivityID = item.PlayerWeeklyActivityID,
                                PlayerID = item.PlayerID,
                                ActivityDate = item.ActivityDate
                            };

                            PlayerWeeklyActivityExt mAct = new PlayerWeeklyActivityExt()
                            {
                                PlayerWeeklyActivityID = item.PlayerWeeklyActivityID,
                                ActivityDate = item.ActivityDate,
                                Activity = item.Activity,
                                ActivityTime = item.ActivityTime,
                                Completed = item.Completed
                            };

                            m.ListOfWeeklyActivities.Add(mAct);

                            ListOfViewModel.Add(m);
                        }
                    }
                }
            }

            return ListOfViewModel;
        }

        public List<PlayerWeeklyActivityExt> ReadAll(long PlayerID)
        {
            var result = db.PlayerWeeklyActivities.Where(m => m.PlayerID == PlayerID).ToList().Select(m => Map(m)).ToList();

            return result;
        }

        #region AutoComplete

        public List<string> GeWeeklyActivityAutoComplete(long PlayerID)
        {
            List<string> ListOfData = new List<string>();

            ListOfData = db.PlayerWeeklyActivities.Where(m => m.PlayerID == PlayerID).Select(m => m.Activity).Distinct().ToList();

            return ListOfData;
        }

        #endregion AutoComplete

        #region Share Activity

        public List<WeeklyActivityCSV> CreateListOfActivities(ShareActivity model, Controller ctrl)
        {
            List<WeeklyActivityCSV> ListOfActivitiesCSV = new List<WeeklyActivityCSV>();

            if (model.ShareFrequencyID == 1) //Today
            {
                DateTime TodayDate = DateTime.Now.Date;

                var StartDateOfWeek = DateTimeExtensions.StartOfWeek(TodayDate, DayOfWeek.Monday);
                var EndDateOfWeek = StartDateOfWeek.AddDays(6);

                var Activities = db.PlayerWeeklyActivities.Where(m => m.PlayerID == model.PlayerID && (m.ActivityDate >= StartDateOfWeek && m.ActivityDate <= EndDateOfWeek)).OrderBy(o => o.ActivityDate).ToList().Select(m => MAP_To_CSV(m)).ToList();

                for (DateTime loopDate = StartDateOfWeek; loopDate <= EndDateOfWeek; loopDate = loopDate.AddDays(1))
                {
                    if (Activities.Any(m => IsDateMatched(m.ActivityDate, loopDate)) == false)
                    {
                        WeeklyActivityCSV m = new WeeklyActivityCSV()
                        {
                            ActivityDate = loopDate.ToString("dd/MM/yyyy"),
                            Activity = "",
                            ActivityTime = "",
                            Completed = ""
                        };

                        Activities.Add(m);
                    }
                }

                ListOfActivitiesCSV.AddRange(Activities);
            }
            else if (model.ShareFrequencyID == 2) //Last 7 Days
            {
                var StartDateOfWeek = DateTimeExtensions.StartOfWeek(DateTime.Now.Date.AddDays(-7), DayOfWeek.Monday);
                var EndDateOfWeek = StartDateOfWeek.AddDays(6);

                var Activities = db.PlayerWeeklyActivities.Where(m => m.PlayerID == model.PlayerID && (m.ActivityDate >= StartDateOfWeek && m.ActivityDate <= EndDateOfWeek)).OrderBy(o => o.ActivityDate).ToList().Select(m => MAP_To_CSV(m)).ToList();

                for (DateTime loopDate = StartDateOfWeek; loopDate <= EndDateOfWeek; loopDate = loopDate.AddDays(1))
                {
                    if (Activities.Any(m => IsDateMatched(m.ActivityDate, loopDate)) == false)
                    {
                        WeeklyActivityCSV m = new WeeklyActivityCSV()
                        {
                            ActivityDate = loopDate.ToString("dd/MM/yyyy"),
                            Activity = "",
                            ActivityTime = "",
                            Completed = ""
                        };

                        Activities.Add(m);
                    }
                }

                ListOfActivitiesCSV.AddRange(Activities);
            }
            else if (model.ShareFrequencyID == 3) //Last Month
            {
                var StartDateOfWeek = DateTimeExtensions.StartOfWeek(DateTime.Now.Date.AddDays(-30), DayOfWeek.Monday);
                var EndDateOfWeek = DateTime.Now.Date;

                var Activities = db.PlayerWeeklyActivities.Where(m => m.PlayerID == model.PlayerID && (m.ActivityDate >= StartDateOfWeek && m.ActivityDate <= EndDateOfWeek)).OrderBy(o => o.ActivityDate).ToList().Select(m => MAP_To_CSV(m)).ToList();

                for (DateTime loopDate = StartDateOfWeek; loopDate <= EndDateOfWeek; loopDate = loopDate.AddDays(1))
                {
                    if (Activities.Any(m => IsDateMatched(m.ActivityDate, loopDate)) == false)
                    {
                        WeeklyActivityCSV m = new WeeklyActivityCSV()
                        {
                            ActivityDate = loopDate.ToString("dd/MM/yyyy"),
                            Activity = "",
                            ActivityTime = "",
                            Completed = ""
                        };

                        Activities.Add(m);
                    }
                }

                ListOfActivitiesCSV.AddRange(Activities);
            }
            else if (model.ShareFrequencyID == 4) //Date Range
            {
                var StartDate = model.Activity_ShareDateFrom.Value;
                var EndDate = model.Activity_ShareDateTo.Value;

                var Activities = db.PlayerWeeklyActivities.Where(m => m.PlayerID == model.PlayerID && (m.ActivityDate >= StartDate && m.ActivityDate <= EndDate)).OrderBy(o => o.ActivityDate).ToList().Select(m => MAP_To_CSV(m)).ToList();

                for (DateTime loopDate = StartDate; loopDate <= EndDate; loopDate = loopDate.AddDays(1))
                {
                    if (Activities.Any(m => IsDateMatched(m.ActivityDate, loopDate)) == false)
                    {
                        WeeklyActivityCSV m = new WeeklyActivityCSV()
                        {
                            ActivityDate = loopDate.ToString("dd/MM/yyyy"),
                            Activity = "",
                            ActivityTime = "",
                            Completed = ""
                        };

                        Activities.Add(m);
                    }
                }

                ListOfActivitiesCSV.AddRange(Activities);
            }

            return ListOfActivitiesCSV;
        }

        public void ShareActivity(ShareActivity model, Controller ctrl)
        {
            List<WeeklyActivityCSV> ListOfActivitiesCSV = CreateListOfActivities(model, ctrl);

            //Get Player Details
            PlayersRepository playerRepo = new PlayersRepository();
            var player = playerRepo.ReadOne(model.PlayerID);

            var leagueName = (string.IsNullOrEmpty(player.SelectedLeagueName) == false ? player.SelectedLeagueName : "No League Selected");

            string FileName = "WeeklyActivity_" + player.FullName.Replace(" ", string.Empty) + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".csv";

            //Now Create CSV
            string fullFilePath = CreateCSV(ListOfActivitiesCSV, FileName, player.FullName, player.EmailAddress, leagueName, ctrl);

            //Now Send Email

            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();

            PlayerDashboardRepository pdRepo = new PlayerDashboardRepository();
            var playerDashboard = pdRepo.ReadOne(model.PlayerID);

            EmailsRepository emailRepo = new EmailsRepository();

            string Body = "Hi," +
                "<br/><br/> I would like to share my Weekly Activity Data with you " +
                "<br/><br/> Personal Message: " + model.MsgBody;
            Guid guid = Guid.NewGuid();
            EmailThread = new Thread(() =>
               emailRepo.SendEmailWithAttachment(playerDashboard.PlayerEmailAddress, model.EmailAddress, "Weekly Activity Data - " + playerDashboard.PlayerFullName, Body, fullFilePath, EmailThread, ctrl, sys.SupportEmailAddress, null, false));
            EmailThread.Name = "WeeklyActivityData_" + guid.ToString();
            EmailThread.Start();
        }

        public bool IsDateMatched(string ActivityDate, DateTime LoopDate)
        {
            bool status = false;

            if (!string.IsNullOrEmpty(ActivityDate))
            {
                var _ActivityDate = Convert.ToDateTime(ActivityDate);

                if (_ActivityDate == LoopDate)
                {
                    status = true;
                }
            }

            return status;
        }

        public void AddBlankEntries()
        {
        }

        public string CreateCSV(List<WeeklyActivityCSV> ListOfActivities, string FileName, string PlayerName, string PlayerEmailAddress, string LeagueName, Controller ctrl)
        {
            string CSVFilePath = "";
            WeeklyActivityCSV dataCSV = new WeeklyActivityCSV();

            var output = dataCSV.ToCsvHeader();
            output += Environment.NewLine;

            ListOfActivities.ForEach(data =>
            {
                output += data.ToCsvRow();
                output += Environment.NewLine;
            });

            output += "-------------------------------------";
            output += Environment.NewLine;

            output += "Player Name: " + PlayerName;
            output += Environment.NewLine;

            output += "Email Address: " + PlayerEmailAddress;
            output += Environment.NewLine;

            output += "League Name: " + LeagueName;
            output += Environment.NewLine;

            try
            {
                var path = ctrl.Server.MapPath(SecurityUtils.ActivityData_Path);

                //if Directory is not exists then create one
                if (!System.IO.Directory.Exists(path))
                {
                    System.IO.Directory.CreateDirectory(path);
                }

                CSVFilePath = System.IO.Path.Combine(path, FileName);

                System.IO.File.WriteAllText(CSVFilePath, output);
            }
            catch (Exception)
            {
                throw;
            }
            finally
            {
                //if(System.IO.File.Exists(path))
                //{
                //    System.IO.File.Delete(path);
                //}
            }

            return CSVFilePath;
        }

        public WeeklyActivityCSV MAP_To_CSV(PlayerWeeklyActivities model)
        {
            WeeklyActivityCSV m = new WeeklyActivityCSV()
            {
                ActivityDate = model.ActivityDate.ToString("dd/MM/yyyy"),
                Activity = model.Activity,
                ActivityTime = ActivityMinutesToString(model.ActivityTime),
                Completed = model.Completed ? "Yes" : "No"
            };

            return m;
        }

        public string ActivityMinutesToString(int? ActivityTime)
        {
            string ActivityTimeStr = "";
            if (ActivityTime.HasValue)
            {
                if (ActivityTime.Value < 60)
                {
                    ActivityTimeStr = ActivityTime.Value.ToString() + " minutes";
                }
                else if (ActivityTime.Value == 60)
                {
                    ActivityTimeStr = "1 Hour";
                }
                else if (ActivityTime.Value > 60 && ActivityTime.Value < 120)
                {
                    ActivityTimeStr = "1 Hour";
                    int minutes = ActivityTime.Value - 60;
                    ActivityTimeStr += " " + minutes.ToString() + " minutes";
                }
                else if (ActivityTime.Value == 120)
                {
                    ActivityTimeStr = "2 Hours";
                }
                else if (ActivityTime.Value > 120 && ActivityTime.Value < 180)
                {
                    ActivityTimeStr = "2 Hours";
                    int minutes = ActivityTime.Value - 120;
                    ActivityTimeStr += " " + minutes.ToString() + " minutes";
                }
                else if (ActivityTime.Value == 180)
                {
                    ActivityTimeStr = "3 Hours";
                }
                else
                {
                    ActivityTimeStr = "";
                }
            }

            return ActivityTimeStr;
        }

        #endregion Share Activity

        #region Maintenance

        public long CreateOrUpdate(PlayerWeeklyActivityExt model, ref string Msg, ref bool status, Controller ctrl)
        {
            long PlayerWeeklyActivityID = 0;

            //Remove the last commna
            model.Activity = string.IsNullOrEmpty(model.Activity) == false ? model.Activity.TrimEnd().TrimEnd(',') : null;

            if (model.PlayerWeeklyActivityID == 0)
            {
                try
                {
                    //TODO: Map to DB Object
                    var dbmodel = Map(model);
                    //TODO: Save DB Changes and Set the Return Primary Key ID
                    db.PlayerWeeklyActivities.Add(dbmodel);
                    db.SaveChanges();
                    //TOD: Add to Audit Log
                    AuditLog(ctrl, AuditAction.Create, model, null);

                    PlayerWeeklyActivityID = dbmodel.PlayerWeeklyActivityID;
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
                    var dbmodel = db.PlayerWeeklyActivities.FirstOrDefault(p => p.PlayerWeeklyActivityID == model.PlayerWeeklyActivityID);
                    var ForAuditLog = Map(dbmodel);
                    //TODO: Map to DB Object
                    MapUpdate(ref dbmodel, model);
                    //TODO: Update DB Changes
                    db.SaveChanges();

                    //TOD: Add to Audit Log
                    AuditLog(ctrl, AuditAction.Update, ForAuditLog, model);

                    PlayerWeeklyActivityID = dbmodel.PlayerWeeklyActivityID;
                }
                catch (Exception ex)
                {
                    Msg = ErrorHandling.HandleException(ex);
                    status = false;
                }
            }

            return PlayerWeeklyActivityID;
        }

        public void DeleteWeeklyActivity(long PlayerWeeklyActivityID, Controller ctrl)
        {
            var ItemToDelete = db.PlayerWeeklyActivities.FirstOrDefault(m => m.PlayerWeeklyActivityID == PlayerWeeklyActivityID);

            if (ItemToDelete != null)
            {
                var ForAudiLog = Map(ItemToDelete);

                db.PlayerWeeklyActivities.Remove(ItemToDelete);
                db.SaveChanges();

                AuditLog(ctrl, AuditAction.Delete, null, ForAudiLog);
            }
        }

        public PlayerWeeklyActivities Map(PlayerWeeklyActivityExt model)
        {
            PlayerWeeklyActivities tblModel = new PlayerWeeklyActivities()
            {
                PlayerWeeklyActivityID = model.PlayerWeeklyActivityID,
                PlayerID = model.PlayerID,
                ActivityDate = model.ActivityDate,
                Activity = model.Activity,
                ActivityTime = model.ActivityTime,
                Completed = model.Completed
            };

            return tblModel;
        }

        public PlayerWeeklyActivityExt Map(PlayerWeeklyActivities model)
        {
            PlayerWeeklyActivityExt tblModel = new PlayerWeeklyActivityExt()
            {
                PlayerWeeklyActivityID = model.PlayerWeeklyActivityID,
                PlayerID = model.PlayerID,
                ActivityDate = model.ActivityDate,
                Activity = model.Activity,
                ActivityTime = model.ActivityTime,
                Completed = model.Completed
            };

            return tblModel;
        }

        public void MapUpdate(ref PlayerWeeklyActivities dbmodel, PlayerWeeklyActivityExt model)
        {
            dbmodel.PlayerWeeklyActivityID = model.PlayerWeeklyActivityID;
            //dbmodel.PlayerID = model.PlayerID;
            //dbmodel.ActivityDate = model.ActivityDate;

            dbmodel.Activity = model.Activity;
            dbmodel.ActivityTime = model.ActivityTime;
            dbmodel.Completed = model.Completed;
        }

        /// <summary>
        /// Add the Action to Audit Log
        /// </summary>
        /// <param name="model">The Object for which this Auditlog took place</param>
        /// <param name="Action">"Added New User OR Updated User Details OR Deleted User"</param>

        private void AuditLog(Controller ctrl, AuditAction auditAction, PlayerWeeklyActivityExt dbmodel = null, PlayerWeeklyActivityExt model = null)
        {
            string AuditLogShortDesc = "", AuditLogLongDesc = "";

            StringBuilder sb = new StringBuilder();

            if (auditAction == AuditAction.Create) //Creating new Record
            {
                AuditLogShortDesc = AuditLogLongDesc = "New Player Weekly Activity has been Added PlayerID = " + dbmodel.PlayerID + " Activity Date = " + dbmodel.ActivityDate.ToString("dd/MM/yyyy");
            }
            else if (auditAction == AuditAction.Update)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Player Weekly Activity has been Updated PlayerID = " + model.PlayerID + " DashboardURL = " + model.ActivityDate.ToString("dd/MM/yyyy");
            }
            else if (auditAction == AuditAction.Delete)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Player Weekly Activity has been Deleted PlayerID = " + model.PlayerID + " DashboardURL = " + model.ActivityDate.ToString("dd/MM/yyyy");
            }

            SecurityUtils.AddAuditLog(AuditLogShortDesc, AuditLogLongDesc, ctrl);
        }

        #endregion Maintenance
    }

    public static class DateTimeExtensions
    {
        public static DateTime StartOfWeek(this DateTime dt, DayOfWeek startOfWeek)
        {
            int diff = dt.DayOfWeek - startOfWeek;
            if (diff < 0)
            {
                diff += 7;
            }
            return dt.AddDays(-1 * diff).Date;
        }
    }

    public class vmWeeklyActivity
    {
        public long PlayerWeeklyActivityID { get; set; }
        public long PlayerID { get; set; }
        public DateTime ActivityDate { get; set; }

        public List<PlayerWeeklyActivityExt> ListOfWeeklyActivities { get; set; }

        public vmWeeklyActivity()
        {
            ListOfWeeklyActivities = new List<PlayerWeeklyActivityExt>();
        }
    }

    public class PlayerWeeklyActivityExt
    {
        public long PlayerWeeklyActivityID { get; set; }
        public long PlayerID { get; set; }
        public System.DateTime ActivityDate { get; set; }
        public string Activity { get; set; }
        public int? ActivityTime { get; set; }
        public bool Completed { get; set; }
    }

    public class WeeklyActivityExt
    {
        public long PlayerWeeklyActivityID { get; set; }
        public long PlayerID { get; set; }
        public System.DateTime ActivityDate { get; set; }
        public string Activity { get; set; }
        public int? ActivityTime { get; set; }
        public bool Completed { get; set; }
    }

    public class WeeklyActivityCSV
    {
        public string ActivityDate { get; set; }
        public string Activity { get; set; }
        public string ActivityTime { get; set; }
        public string Completed { get; set; }
    }
}