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
    public class PlayerWeightsRepository : BaseRepository
    {
        private Thread EmailThread;

        public List<PlayerWeightExt> ReadAll(long PlayerID)
        {
            var result = db.PlayerWeights.Where(m => m.PlayerID == PlayerID).ToList().Select(m => Map(m)).ToList();

            return result;
        }
        public PlayerWeightExt ReadOne(long PlayerID, DateTime ActivityDate, Controller ctrl)
        {
            var model = db.PlayerWeights.Where(m => m.PlayerID == PlayerID && m.ActivityDate == ActivityDate).FirstOrDefault();
            if (model != null)
                return Map(model);
            else
            {
                //if model is null then Create an ACtivity Record in db and return to View
                PlayerWeightExt m = new PlayerWeightExt()
                {
                    PlayerID = PlayerID,
                    ActivityDate = ActivityDate
                };
                string Msg = "";
                bool status = true;

                m.PlayerWeightID = CreateOrUpdate(m, ref Msg, ref status, ctrl);

                return m;
            }
        }

        #region AutoComplete



        #endregion

        #region Share Activity

        public List<PlayerWeightSCV> CreateListOfWeights(ShareActivity model, Controller ctrl)
        {
            List<PlayerWeightSCV> ListOfWeightsCSV = new List<PlayerWeightSCV>();

            if (model.ShareFrequencyID == 1) //Today
            {
                List<PlayerWeightExt> ListOfWeights = new List<PlayerWeightExt>();

                var Activity = ReadOne(model.PlayerID, DateTime.Now.Date, ctrl);

                ListOfWeights.Add(Activity);

                ListOfWeightsCSV.AddRange(ListOfWeights.Select(m => MAP_To_CSV(m)).ToList());
            }

            else if (model.ShareFrequencyID == 2) //Last 7 Days
            {
                List<PlayerWeightExt> ListOfWeights = new List<PlayerWeightExt>();

                var StartDate = DateTime.Now.Date.AddDays(-6);
                var EndDate = DateTime.Now.Date;


                for (DateTime loopDate = StartDate; loopDate <= EndDate; loopDate = loopDate.AddDays(1))
                {
                    var Activity = ReadOne(model.PlayerID, loopDate, ctrl);

                    ListOfWeights.Add(Activity);
                }

                ListOfWeightsCSV.AddRange(ListOfWeights.Select(m => MAP_To_CSV(m)).ToList());
            }

            else if (model.ShareFrequencyID == 3) //Last Month
            {
                List<PlayerWeightExt> ListOfWeights = new List<PlayerWeightExt>();

                var StartDate = DateTime.Now.Date.AddDays(-30);
                var EndDate = DateTime.Now.Date;

                for (DateTime loopDate = StartDate; loopDate <= EndDate; loopDate = loopDate.AddDays(1))
                {
                    var Activity = ReadOne(model.PlayerID, loopDate, ctrl);

                    ListOfWeights.Add(Activity);
                }

                ListOfWeightsCSV.AddRange(ListOfWeights.Select(m => MAP_To_CSV(m)).ToList());
            }

            else if (model.ShareFrequencyID == 4) //Date Range
            {
                List<PlayerWeightExt> ListOfWeights = new List<PlayerWeightExt>();

                var StartDate = model.Activity_ShareDateFrom.Value;
                var EndDate = model.Activity_ShareDateTo.Value;

                for (DateTime loopDate = StartDate; loopDate <= EndDate; loopDate = loopDate.AddDays(1))
                {
                    var Activity = ReadOne(model.PlayerID, loopDate, ctrl);

                    ListOfWeights.Add(Activity);
                }

                ListOfWeightsCSV.AddRange(ListOfWeights.Select(m => MAP_To_CSV(m)).ToList());
            }

            return ListOfWeightsCSV;
        }

        public void ShareActivity(ShareActivity model, Controller ctrl)
        {

            List<PlayerWeightSCV> ListOfWeightsCSV = CreateListOfWeights(model, ctrl);

            //Get Player Details
            PlayersRepository playerRepo = new PlayersRepository();
            var player = playerRepo.ReadOne(model.PlayerID);

            string FileName = "Weights_" + player.FullName.Replace(" ", string.Empty) + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".csv";

            //Now Create CSV
            string fullFilePath = CreateCSV(ListOfWeightsCSV, FileName, player.FullName, player.EmailAddress, ctrl);

            //Now Send Email

            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();

            PlayerDashboardRepository pdRepo = new PlayerDashboardRepository();
            var playerDashboard = pdRepo.ReadOne(model.PlayerID);

            EmailsRepository emailRepo = new EmailsRepository();

            string Body = "Hi," +
                "<br/><br/> I would like to share my Weights Data with you " +
                "<br/><br/> Personal Message: " + model.MsgBody;
            Guid guid = Guid.NewGuid();
            EmailThread = new Thread(() =>
               emailRepo.SendEmailWithAttachment(playerDashboard.PlayerEmailAddress, model.EmailAddress, "Weights Data - " + playerDashboard.PlayerFullName, Body, fullFilePath, EmailThread, ctrl, sys.SupportEmailAddress, null, false));
            EmailThread.Name = "WeightsData_" + guid.ToString();
            EmailThread.Start();

        }

        public string CreateCSV(List<PlayerWeightSCV> ListOfWeights, string FileName, string PlayerName, string PlayerEmailAddress, Controller ctrl)
        {
            string CSVFilePath = "";
            PlayerWeightSCV dataCSV = new PlayerWeightSCV();
            string output = "";



            output = dataCSV.ToCsvHeader();
            output += Environment.NewLine;

            ListOfWeights.ForEach(data =>
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

        public PlayerWeightSCV MAP_To_CSV(PlayerWeightExt model)
        {
            PlayerWeightSCV m = new PlayerWeightSCV()
            {
                ActivityDate = model.ActivityDate.ToString("dd/MM/yyyy"),
                Weight = model.Weight.ToString("N2"),
            };

            return m;
        }


        #endregion

        #region Maintenance
        public long CreateOrUpdate(PlayerWeightExt model, ref string Msg, ref bool status, Controller ctrl)
        {
            long PlayerWeightID = 0;

            if (model.PlayerWeightID == 0)
            {
                try
                {
                    //TODO: Map to DB Object
                    var dbmodel = Map(model);
                    //TODO: Save DB Changes and Set the Return Primary Key ID
                    db.PlayerWeights.Add(dbmodel);
                    db.SaveChanges();
                    //TOD: Add to Audit Log
                    AuditLog(ctrl, AuditAction.Create, model, null);

                    PlayerWeightID = dbmodel.PlayerWeightID;
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
                    var dbmodel = db.PlayerWeights.FirstOrDefault(p => p.PlayerWeightID == model.PlayerWeightID);
                    var ForAuditLog = Map(dbmodel);
                    //TODO: Map to DB Object
                    MapUpdate(ref dbmodel, model);
                    //TODO: Update DB Changes
                    db.SaveChanges();

                    //TOD: Add to Audit Log
                    AuditLog(ctrl, AuditAction.Update, ForAuditLog, model);

                    PlayerWeightID = dbmodel.PlayerWeightID;
                }
                catch (Exception ex)
                {
                    Msg = ErrorHandling.HandleException(ex);
                    status = false;
                }
            }

            return PlayerWeightID;
        }


        public PlayerWeights Map(PlayerWeightExt model)
        {
            PlayerWeights tblModel = new PlayerWeights()
            {
                PlayerWeightID = model.PlayerWeightID,
                PlayerID = model.PlayerID,
                ActivityDate = model.ActivityDate,
                Weight = model.Weight
               
            };

            return tblModel;
        }

        public PlayerWeightExt Map(PlayerWeights model)
        {
            PlayerWeightExt tblModel = new PlayerWeightExt()
            {
                PlayerWeightID = model.PlayerWeightID,
                PlayerID = model.PlayerID,
                ActivityDate = model.ActivityDate,
                Weight = model.Weight
            };

            return tblModel;
        }

        public void MapUpdate(ref PlayerWeights dbmodel, PlayerWeightExt model)
        {
            dbmodel.PlayerWeightID = model.PlayerWeightID;
            //dbmodel.PlayerID = model.PlayerID;
            //dbmodel.ActivityDate = model.ActivityDate;
            dbmodel.Weight = model.Weight;
        }

        /// <summary>
        /// Add the Action to Audit Log
        /// </summary>
        /// <param name="model">The Object for which this Auditlog took place</param>
        /// <param name="Action">"Added New User OR Updated User Details OR Deleted User"</param>

        private void AuditLog(Controller ctrl, AuditAction auditAction, PlayerWeightExt dbmodel = null, PlayerWeightExt model = null)
        {
            string AuditLogShortDesc = "", AuditLogLongDesc = "";

            StringBuilder sb = new StringBuilder();

            if (auditAction == AuditAction.Create) //Creating new Record
            {
                AuditLogShortDesc = AuditLogLongDesc = "New Player Weight has been Added PlayerID = " + dbmodel.PlayerID + " Activity Date = " + dbmodel.ActivityDate.ToString("dd/MM/yyyy");
            }
            else if (auditAction == AuditAction.Update)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Player Weight has been Updated PlayerID = " + model.PlayerID + " DashboardURL = " + model.ActivityDate.ToString("dd/MM/yyyy");
            }
            else if (auditAction == AuditAction.Delete)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Player Weight has been Deleted PlayerID = " + model.PlayerID + " DashboardURL = " + model.ActivityDate.ToString("dd/MM/yyyy");
            }

            SecurityUtils.AddAuditLog(AuditLogShortDesc, AuditLogLongDesc, ctrl);
        }

        #endregion
    }

    public partial class PlayerWeightExt
    {
        public long PlayerWeightID { get; set; }
        public long PlayerID { get; set; }
        public System.DateTime ActivityDate { get; set; }
        public decimal Weight { get; set; }
    }

    public partial class PlayerWeightSCV
    {
        public long PlayerWeightID { get; set; }
        public long PlayerID { get; set; }
        public string ActivityDate { get; set; }
        public string Weight { get; set; }
    }
}