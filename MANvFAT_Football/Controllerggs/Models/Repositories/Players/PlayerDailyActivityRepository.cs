using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Web.Mvc;

namespace MANvFAT_Football.Models.Repositories
{
    public class PlayerDailyActivityRepository : BaseRepository
    {
        private Thread EmailThread;

        public List<PlayerDailyActivityExt> ReadAll(long PlayerID)
        {
            var result = db.PlayerDailyActivities.Where(m => m.PlayerID == PlayerID).ToList().Select(m => Map(m)).ToList();

            return result;
        }
        public PlayerDailyActivityExt ReadOne(long PlayerID, DateTime ActivityDate, Controller ctrl)
        {

           // ActivityDate = new DateTime(2018, 1, 24);
            var model = db.PlayerDailyActivities.Where(m => m.PlayerID == PlayerID && m.ActivityDate==ActivityDate).FirstOrDefault();
            if (model != null)
                return Map(model);
            else
            {
                //if model is null then Create an ACtivity Record in db and return to View
                PlayerDailyActivityExt m = new PlayerDailyActivityExt()
                {
                    PlayerID = PlayerID,
                    ActivityDate = ActivityDate
                };
                string Msg = "";
                bool status = true;

              m.PlayerDailyActivityID =  CreateOrUpdate(m, ref Msg,ref status, ctrl);

                return m;
            }
        }

        #region AutoComplete

        public List<string> GetDailyActivityAutoComplete(long PlayerID, enumDailyActivityTypes ActivityType)
        {
            List<string> ListOfData = new List<string>();

            if (ActivityType == enumDailyActivityTypes.Breakfast)
            {
                ListOfData = db.PlayerDailyActivities.Where(m => m.PlayerID == PlayerID).Select(m => m.Breakfast).Distinct().ToList();
            }
            else if (ActivityType == enumDailyActivityTypes.Lunch)
            {
                ListOfData = db.PlayerDailyActivities.Where(m => m.PlayerID == PlayerID).Select(m => m.Lunch).Distinct().ToList();
            }
            else if (ActivityType == enumDailyActivityTypes.Dinner)
            {
                ListOfData = db.PlayerDailyActivities.Where(m => m.PlayerID == PlayerID).Select(m => m.Dinner).Distinct().ToList();
            }
            else if (ActivityType == enumDailyActivityTypes.Snacks)
            {
                ListOfData = db.PlayerDailyActivities.Where(m => m.PlayerID == PlayerID).Select(m => m.Snacks).Distinct().ToList();
            }

            return ListOfData;
        }

        #endregion

        #region Share Activity

        public List<PlayerDailyActivityCSV> CreateListOfActivities(ShareActivity model, Controller ctrl)
        {
            List<PlayerDailyActivityCSV> ListOfActivitiesCSV = new List<PlayerDailyActivityCSV>();

            if (model.ShareFrequencyID == 1) //Today
            {
                List<PlayerDailyActivityExt> ListOfActivities = new List<PlayerDailyActivityExt>();

                var Activity = ReadOne(model.PlayerID, DateTime.Now.Date, ctrl);

                ListOfActivities.Add(Activity);

                ListOfActivitiesCSV.AddRange(ListOfActivities.Select(m => MAP_To_CSV(m)).ToList());
            }

            else if (model.ShareFrequencyID == 2) //Last 7 Days
            {
                List<PlayerDailyActivityExt> ListOfActivities = new List<PlayerDailyActivityExt>();

                var StartDate = DateTime.Now.Date.AddDays(-6);
                var EndDate = DateTime.Now.Date;


                for (DateTime loopDate = StartDate; loopDate <= EndDate; loopDate = loopDate.AddDays(1))
                {
                    var Activity = ReadOne(model.PlayerID, loopDate, ctrl);

                    ListOfActivities.Add(Activity);
                }

                ListOfActivitiesCSV.AddRange(ListOfActivities.Select(m => MAP_To_CSV(m)).ToList());
            }

            else if (model.ShareFrequencyID == 3) //Last Month
            {
                List<PlayerDailyActivityExt> ListOfActivities = new List<PlayerDailyActivityExt>();

                var StartDate = DateTime.Now.Date.AddDays(-30);
                var EndDate = DateTime.Now.Date;

                for (DateTime loopDate = StartDate; loopDate <= EndDate; loopDate = loopDate.AddDays(1))
                {
                    var Activity = ReadOne(model.PlayerID, loopDate, ctrl);

                    ListOfActivities.Add(Activity);
                }

                ListOfActivitiesCSV.AddRange(ListOfActivities.Select(m => MAP_To_CSV(m)).ToList());
            }

            else if (model.ShareFrequencyID == 4) //Date Range
            {
                List<PlayerDailyActivityExt> ListOfActivities = new List<PlayerDailyActivityExt>();

                var StartDate = model.Activity_ShareDateFrom.Value;
                var EndDate = model.Activity_ShareDateTo.Value;

                for (DateTime loopDate = StartDate; loopDate <= EndDate; loopDate = loopDate.AddDays(1))
                {
                    var Activity = ReadOne(model.PlayerID, loopDate, ctrl);

                    ListOfActivities.Add(Activity);
                }

                ListOfActivitiesCSV.AddRange(ListOfActivities.Select(m => MAP_To_CSV(m)).ToList());
            }

            return ListOfActivitiesCSV;
        }

        public void ShareActivity(ShareActivity model, Controller ctrl)
        {
           
            List<PlayerDailyActivityCSV> ListOfActivitiesCSV = CreateListOfActivities(model,ctrl);
            
            //Get Player Details
            PlayersRepository playerRepo = new PlayersRepository();
            var player = playerRepo.ReadOne(model.PlayerID);

            var leagueName = (string.IsNullOrEmpty(player.SelectedLeagueName) == false ? player.SelectedLeagueName : "No League Selected");

            string FileName = "DailyActivity_" + player.FullName.Replace(" ", string.Empty) + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".csv";

            //Now Create CSV
            string fullFilePath = CreateCSV(ListOfActivitiesCSV, FileName, player.FullName, player.EmailAddress, leagueName, ctrl);

            //Now Send Email

            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();

            PlayerDashboardRepository pdRepo = new PlayerDashboardRepository();
            var playerDashboard = pdRepo.ReadOne(model.PlayerID);

            EmailsRepository emailRepo = new EmailsRepository();

            string Body = "Hi," +
                "<br/><br/> I would like to share my Daily Activity Data with you " +
                "<br/><br/> Personal Message: "+model.MsgBody;
            Guid guid = Guid.NewGuid();
            EmailThread = new Thread(() =>
               emailRepo.SendEmailWithAttachment(playerDashboard.PlayerEmailAddress, model.EmailAddress, "Daily Activity Data - " + playerDashboard.PlayerFullName, Body, fullFilePath, EmailThread, ctrl, sys.SupportEmailAddress, null, false));
            EmailThread.Name = "DailyActivityData_" + guid.ToString();
            EmailThread.Start();

        }

        public string CreateCSV(List<PlayerDailyActivityCSV> ListOfActivities,string FileName, string PlayerName, string PlayerEmailAddress, string LeagueName, Controller ctrl)
        {
            string CSVFilePath = "";
            PlayerDailyActivityCSV dataCSV = new PlayerDailyActivityCSV();
            string output = "";

          

            output = dataCSV.ToCsvHeader();
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

        public PlayerDailyActivityCSV MAP_To_CSV(PlayerDailyActivityExt model)
        {
            PlayerDailyActivityCSV m = new PlayerDailyActivityCSV()
            {
                ActivityDate = model.ActivityDate.ToString("dd/MM/yyyy"),
                Breakfast = model.Breakfast,
                Lunch = model.Lunch,
                Dinner = model.Dinner,
                Snacks = model.Snacks,
                Drink = model.Drink,
                HowHealthy = model.HowHealthy
            };

            return m;
        }


        #endregion

        #region Maintenance
        public long CreateOrUpdate(PlayerDailyActivityExt model, ref string Msg, ref bool status, Controller ctrl)
        {
            long PlayerDailyActivityID = 0;

            //Remove the last commna
            model.Breakfast = string.IsNullOrEmpty(model.Breakfast) == false ? model.Breakfast.TrimEnd().TrimEnd(',') : null;
            model.Lunch = string.IsNullOrEmpty(model.Lunch)==false ? model.Lunch.TrimEnd().TrimEnd(',') : null;
            model.Dinner = string.IsNullOrEmpty(model.Dinner) == false ? model.Dinner.TrimEnd().TrimEnd(',') : null;
            model.Snacks = string.IsNullOrEmpty(model.Snacks) == false ? model.Snacks.TrimEnd().TrimEnd(',') : null;
            model.Drink = string.IsNullOrEmpty(model.Drink) == false ? model.Drink.TrimEnd().TrimEnd(',') : null;

            if (model.PlayerDailyActivityID == 0)
            {
                try
                {
                    //TODO: Map to DB Object
                    var dbmodel = Map(model);
                    //TODO: Save DB Changes and Set the Return Primary Key ID
                    db.PlayerDailyActivities.Add(dbmodel);
                    db.SaveChanges();
                    //TOD: Add to Audit Log
                    AuditLog(ctrl, AuditAction.Create, model, null);

                    PlayerDailyActivityID = dbmodel.PlayerDailyActivityID;
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
                    var dbmodel = db.PlayerDailyActivities.FirstOrDefault(p => p.PlayerDailyActivityID == model.PlayerDailyActivityID);
                    var ForAuditLog = Map(dbmodel);
                    //TODO: Map to DB Object
                    MapUpdate(ref dbmodel, model);
                    //TODO: Update DB Changes
                    db.SaveChanges();

                    //TOD: Add to Audit Log
                    AuditLog(ctrl, AuditAction.Update, ForAuditLog, model);

                    PlayerDailyActivityID = dbmodel.PlayerDailyActivityID;
                }
                catch (Exception ex)
                {
                    Msg = ErrorHandling.HandleException(ex);
                    status = false;
                }
            }

            return PlayerDailyActivityID;
        }

       
        public PlayerDailyActivities Map(PlayerDailyActivityExt model)
        {
            PlayerDailyActivities tblModel = new PlayerDailyActivities()
            {
                PlayerDailyActivityID = model.PlayerDailyActivityID,
                PlayerID = model.PlayerID,
                ActivityDate = model.ActivityDate,
                Breakfast = model.Breakfast,
                Lunch = model.Lunch,
                Dinner = model.Dinner,
                Snacks = model.Snacks,
                Drink = model.Drink,
                HowHealthy = model.HowHealthy
            };

            return tblModel;
        }

        public PlayerDailyActivityExt Map(PlayerDailyActivities model)
        {
            PlayerDailyActivityExt tblModel = new PlayerDailyActivityExt()
            {
                PlayerDailyActivityID = model.PlayerDailyActivityID,
                PlayerID = model.PlayerID,
                ActivityDate = model.ActivityDate,
                Breakfast = model.Breakfast,
                Lunch = model.Lunch,
                Dinner = model.Dinner,
                Snacks = model.Snacks,
                Drink = model.Drink,
                HowHealthy = model.HowHealthy
            };

            return tblModel;
        }

        public void MapUpdate(ref PlayerDailyActivities dbmodel, PlayerDailyActivityExt model)
        {
            dbmodel.PlayerDailyActivityID = model.PlayerDailyActivityID;
            //dbmodel.PlayerID = model.PlayerID;
            //dbmodel.ActivityDate = model.ActivityDate;

            dbmodel.Breakfast = model.Breakfast;
            dbmodel.Lunch = model.Lunch;
            dbmodel.Dinner = model.Dinner;
            dbmodel.Snacks = model.Snacks;
            dbmodel.Drink = model.Drink;
            dbmodel.HowHealthy = model.HowHealthy;
        }

        /// <summary>
        /// Add the Action to Audit Log
        /// </summary>
        /// <param name="model">The Object for which this Auditlog took place</param>
        /// <param name="Action">"Added New User OR Updated User Details OR Deleted User"</param>

        private void AuditLog(Controller ctrl, AuditAction auditAction, PlayerDailyActivityExt dbmodel = null, PlayerDailyActivityExt model = null)
        {
            string AuditLogShortDesc = "", AuditLogLongDesc = "";

            StringBuilder sb = new StringBuilder();

            if (auditAction == AuditAction.Create) //Creating new Record
            {
                AuditLogShortDesc = AuditLogLongDesc = "New Player Daily Activity has been Added PlayerID = " + dbmodel.PlayerID + " Activity Date = " + dbmodel.ActivityDate.ToString("dd/MM/yyyy");
            }
            else if (auditAction == AuditAction.Update)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Player Daily Activity has been Updated PlayerID = " + model.PlayerID + " DashboardURL = " + model.ActivityDate.ToString("dd/MM/yyyy");
            }
            else if (auditAction == AuditAction.Delete)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Player Daily Activity has been Deleted PlayerID = " + model.PlayerID + " DashboardURL = " + model.ActivityDate.ToString("dd/MM/yyyy");
            }

            SecurityUtils.AddAuditLog(AuditLogShortDesc, AuditLogLongDesc, ctrl);
        }

        #endregion


    }


    public partial class PlayerDailyActivityExt
    {
        public long PlayerDailyActivityID { get; set; }
        public long PlayerID { get; set; }
        public System.DateTime ActivityDate { get; set; }
        public string Breakfast { get; set; }
        public string Lunch { get; set; }
        public string Dinner { get; set; }
        public string Snacks { get; set; }
        public string Drink { get; set; }
        [DisplayName("How healthy were your food and drink choices today?")]
        public int? HowHealthy { get; set; }
    }

    public partial class PlayerDailyActivityCSV
    {
        public string ActivityDate { get; set; }
        public string Breakfast { get; set; }
        public string Lunch { get; set; }
        public string Dinner { get; set; }
        public string Snacks { get; set; }
        public string Drink { get; set; }
        public int? HowHealthy { get; set; }
    }

}