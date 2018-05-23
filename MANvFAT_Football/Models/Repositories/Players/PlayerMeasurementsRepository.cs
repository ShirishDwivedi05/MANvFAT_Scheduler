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
    public class PlayerMeasurementsRepository : BaseRepository
    {
        private Thread EmailThread;

        public List<PlayerMeasurementExt> ReadAll(long PlayerID)
        {
            var result = db.PlayerMeasurements.Where(m => m.PlayerID == PlayerID).ToList().Select(m => Map(m)).ToList();

            return result;
        }
        public PlayerMeasurementExt ReadOne(long PlayerID, DateTime ActivityDate, Controller ctrl)
        {
            var model = db.PlayerMeasurements.Where(m => m.PlayerID == PlayerID && m.ActivityDate == ActivityDate).FirstOrDefault();
            if (model != null)
                return Map(model);
            else
            {
                //if model is null then Create an ACtivity Record in db and return to View
                PlayerMeasurementExt m = new PlayerMeasurementExt()
                {
                    PlayerID = PlayerID,
                    ActivityDate = ActivityDate
                };
                string Msg = "";
                bool status = true;

                m.PlayerMeasurementID = CreateOrUpdate(m, ref Msg, ref status, ctrl);

                return m;
            }
        }

        #region AutoComplete

   

        #endregion

        #region Share Activity

        public List<PlayerMeasurementCSV> CreateListOfMeasurements(ShareActivity model, Controller ctrl)
        {
            List<PlayerMeasurementCSV> ListOfMeasurementsCSV = new List<PlayerMeasurementCSV>();

            if (model.ShareFrequencyID == 1) //Today
            {
                List<PlayerMeasurementExt> ListOfMeasurements = new List<PlayerMeasurementExt>();

                var Activity = ReadOne(model.PlayerID, DateTime.Now.Date, ctrl);

                ListOfMeasurements.Add(Activity);

                ListOfMeasurementsCSV.AddRange(ListOfMeasurements.Select(m => MAP_To_CSV(m)).ToList());
            }

            else if (model.ShareFrequencyID == 2) //Last 7 Days
            {
                List<PlayerMeasurementExt> ListOfMeasurements = new List<PlayerMeasurementExt>();

                var StartDate = DateTime.Now.Date.AddDays(-6);
                var EndDate = DateTime.Now.Date;


                for (DateTime loopDate = StartDate; loopDate <= EndDate; loopDate = loopDate.AddDays(1))
                {
                    var Activity = ReadOne(model.PlayerID, loopDate, ctrl);

                    ListOfMeasurements.Add(Activity);
                }

                ListOfMeasurementsCSV.AddRange(ListOfMeasurements.Select(m => MAP_To_CSV(m)).ToList());
            }

            else if (model.ShareFrequencyID == 3) //Last Month
            {
                List<PlayerMeasurementExt> ListOfMeasurements = new List<PlayerMeasurementExt>();

                var StartDate = DateTime.Now.Date.AddDays(-30);
                var EndDate = DateTime.Now.Date;

                for (DateTime loopDate = StartDate; loopDate <= EndDate; loopDate = loopDate.AddDays(1))
                {
                    var Activity = ReadOne(model.PlayerID, loopDate, ctrl);

                    ListOfMeasurements.Add(Activity);
                }

                ListOfMeasurementsCSV.AddRange(ListOfMeasurements.Select(m => MAP_To_CSV(m)).ToList());
            }

            else if (model.ShareFrequencyID == 4) //Date Range
            {
                List<PlayerMeasurementExt> ListOfMeasurements = new List<PlayerMeasurementExt>();

                var StartDate = model.Activity_ShareDateFrom.Value;
                var EndDate = model.Activity_ShareDateTo.Value;

                for (DateTime loopDate = StartDate; loopDate <= EndDate; loopDate = loopDate.AddDays(1))
                {
                    var Activity = ReadOne(model.PlayerID, loopDate, ctrl);

                    ListOfMeasurements.Add(Activity);
                }

                ListOfMeasurementsCSV.AddRange(ListOfMeasurements.Select(m => MAP_To_CSV(m)).ToList());
            }

            return ListOfMeasurementsCSV;
        }

        public void ShareActivity(ShareActivity model, Controller ctrl)
        {

            List<PlayerMeasurementCSV> ListOfMeasurementsCSV = CreateListOfMeasurements(model, ctrl);

            //Get Player Details
            PlayersRepository playerRepo = new PlayersRepository();
            var player = playerRepo.ReadOne(model.PlayerID);

            string FileName = "Measurements_" + player.FullName.Replace(" ", string.Empty) + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".csv";

            //Now Create CSV
            string fullFilePath = CreateCSV(ListOfMeasurementsCSV, FileName, player.FullName, player.EmailAddress, ctrl);

            //Now Send Email

            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();

            PlayerDashboardRepository pdRepo = new PlayerDashboardRepository();
            var playerDashboard = pdRepo.ReadOne(model.PlayerID);

            EmailsRepository emailRepo = new EmailsRepository();

            string Body = "Hi," +
                "<br/><br/> I would like to share my Measurements Data with you " +
                "<br/><br/> Personal Message: " + model.MsgBody;
            Guid guid = Guid.NewGuid();
            EmailThread = new Thread(() =>
               emailRepo.SendEmailWithAttachment(playerDashboard.PlayerEmailAddress, model.EmailAddress, "Measurements Data - " + playerDashboard.PlayerFullName, Body, fullFilePath, EmailThread, ctrl, sys.SupportEmailAddress, null, false));
            EmailThread.Name = "MeasurementData_" + guid.ToString();
            EmailThread.Start();

        }

        public string CreateCSV(List<PlayerMeasurementCSV> ListOfMeasurements, string FileName, string PlayerName, string PlayerEmailAddress, Controller ctrl)
        {
            string CSVFilePath = "";
            PlayerMeasurementCSV dataCSV = new PlayerMeasurementCSV();
            string output = "";



            output = dataCSV.ToCsvHeader();
            output += Environment.NewLine;

            ListOfMeasurements.ForEach(data =>
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

        public PlayerMeasurementCSV MAP_To_CSV(PlayerMeasurementExt model)
        {
            PlayerMeasurementCSV m = new PlayerMeasurementCSV()
            {
                ActivityDate = model.ActivityDate.ToString("dd/MM/yyyy"),
                Neck = model.Neck.HasValue ? model.Neck.Value.ToString("N2") : "",
                RightBicep = model.RightBicep.HasValue ? model.RightBicep.Value.ToString("N2") : "",
                LeftBicep = model.LeftBicep.HasValue ? model.LeftBicep.Value.ToString("N2") : "",
                Waist = model.Waist.HasValue ? model.Waist.Value.ToString("N2") : "",
                Hips = model.Hips.HasValue ? model.Hips.Value.ToString("N2") : "",
                RightThigh = model.RightThigh.HasValue ? model.RightThigh.Value.ToString("N2") : "",
                LeftThigh = model.LeftThigh.HasValue ? model.LeftThigh.Value.ToString("N2") : "",
            };

            return m;
        }


        #endregion

        #region Maintenance
        public long CreateOrUpdate(PlayerMeasurementExt model, ref string Msg, ref bool status, Controller ctrl)
        {
            long PlayerMeasurementID = 0;

            if (model.PlayerMeasurementID == 0)
            {
                try
                {
                    //TODO: Map to DB Object
                    var dbmodel = Map(model);
                    //TODO: Save DB Changes and Set the Return Primary Key ID
                    db.PlayerMeasurements.Add(dbmodel);
                    db.SaveChanges();
                    //TOD: Add to Audit Log
                    AuditLog(ctrl, AuditAction.Create, model, null);

                    PlayerMeasurementID = dbmodel.PlayerMeasurementID;
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
                    var dbmodel = db.PlayerMeasurements.FirstOrDefault(p => p.PlayerMeasurementID == model.PlayerMeasurementID);
                    var ForAuditLog = Map(dbmodel);
                    //TODO: Map to DB Object
                    MapUpdate(ref dbmodel, model);
                    //TODO: Update DB Changes
                    db.SaveChanges();

                    //TOD: Add to Audit Log
                    AuditLog(ctrl, AuditAction.Update, ForAuditLog, model);

                    PlayerMeasurementID = dbmodel.PlayerMeasurementID;
                }
                catch (Exception ex)
                {
                    Msg = ErrorHandling.HandleException(ex);
                    status = false;
                }
            }

            return PlayerMeasurementID;
        }


        public PlayerMeasurements Map(PlayerMeasurementExt model)
        {
            PlayerMeasurements tblModel = new PlayerMeasurements()
            {
                PlayerMeasurementID = model.PlayerMeasurementID,
                PlayerID = model.PlayerID,
                ActivityDate = model.ActivityDate,
                Neck = model.Neck,
                RightBicep = model.RightBicep,
                LeftBicep = model.LeftBicep,
                Waist = model.Waist,
                Hips = model.Hips,
                RightThigh = model.RightThigh,
                LeftThigh = model.LeftThigh
            };

            return tblModel;
        }

        public PlayerMeasurementExt Map(PlayerMeasurements model)
        {
            PlayerMeasurementExt tblModel = new PlayerMeasurementExt()
            {
                PlayerMeasurementID = model.PlayerMeasurementID,
                PlayerID = model.PlayerID,
                ActivityDate = model.ActivityDate,
                Neck = model.Neck,
                RightBicep = model.RightBicep,
                LeftBicep = model.LeftBicep,
                Waist = model.Waist,
                Hips = model.Hips,
                RightThigh = model.RightThigh,
                LeftThigh = model.LeftThigh
            };

            return tblModel;
        }

        public void MapUpdate(ref PlayerMeasurements dbmodel, PlayerMeasurementExt model)
        {
            dbmodel.PlayerMeasurementID = model.PlayerMeasurementID;
            //dbmodel.PlayerID = model.PlayerID;
            //dbmodel.ActivityDate = model.ActivityDate;

            dbmodel.Neck = model.Neck;
            dbmodel.RightBicep = model.RightBicep;
            dbmodel.LeftBicep = model.LeftBicep;
            dbmodel.Waist = model.Waist;
            dbmodel.Hips = model.Hips;
            dbmodel.RightThigh = model.RightThigh;
            dbmodel.LeftThigh = model.LeftThigh;
        }

        /// <summary>
        /// Add the Action to Audit Log
        /// </summary>
        /// <param name="model">The Object for which this Auditlog took place</param>
        /// <param name="Action">"Added New User OR Updated User Details OR Deleted User"</param>

        private void AuditLog(Controller ctrl, AuditAction auditAction, PlayerMeasurementExt dbmodel = null, PlayerMeasurementExt model = null)
        {
            string AuditLogShortDesc = "", AuditLogLongDesc = "";

            StringBuilder sb = new StringBuilder();

            if (auditAction == AuditAction.Create) //Creating new Record
            {
                AuditLogShortDesc = AuditLogLongDesc = "New Player Measurement has been Added PlayerID = " + dbmodel.PlayerID + " Activity Date = " + dbmodel.ActivityDate.ToString("dd/MM/yyyy");
            }
            else if (auditAction == AuditAction.Update)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Player Measurement has been Updated PlayerID = " + model.PlayerID + " DashboardURL = " + model.ActivityDate.ToString("dd/MM/yyyy");
            }
            else if (auditAction == AuditAction.Delete)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Player Measurement has been Deleted PlayerID = " + model.PlayerID + " DashboardURL = " + model.ActivityDate.ToString("dd/MM/yyyy");
            }

            SecurityUtils.AddAuditLog(AuditLogShortDesc, AuditLogLongDesc, ctrl);
        }

        #endregion
    }

    public partial class PlayerMeasurementExt
    {
        public long PlayerMeasurementID { get; set; }
        public long PlayerID { get; set; }
        public System.DateTime ActivityDate { get; set; }
        public Nullable<decimal> Neck { get; set; }
        public Nullable<decimal> RightBicep { get; set; }
        public Nullable<decimal> LeftBicep { get; set; }
        public Nullable<decimal> Waist { get; set; }
        public Nullable<decimal> Hips { get; set; }
        public Nullable<decimal> RightThigh { get; set; }
        public Nullable<decimal> LeftThigh { get; set; }
    }

    public partial class PlayerMeasurementCSV
    {
        public string ActivityDate { get; set; }
        public string Neck { get; set; }
        public string RightBicep { get; set; }
        public string LeftBicep { get; set; }
        public string Waist { get; set; }
        public string Hips { get; set; }
        public string RightThigh { get; set; }
        public string LeftThigh { get; set; }
    }
}