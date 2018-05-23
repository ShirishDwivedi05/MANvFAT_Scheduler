using Kendo.Mvc.Extensions;
using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web.Mvc;

namespace MANvFAT_Football.Models.Repositories
{
    public class PlayersRepository : BaseRepository
    {
        public List<PlayersExt> ReadAll()
        {
            return db.Players.Include("Heights").Include("Advertisements").OrderBy(o => o.FullName).ToList().Select(m => Map(m)).ToList();
        }

        public List<Players> ReadAll_WithoutMapping(bool? OnlyActive)
        {
            return db.Players.Where(m => OnlyActive.HasValue == false || m.Active == OnlyActive).ToList();
        }

        public long? GetPlayerID_ByEmail(string EmailAddress)
        {
            var player = db.Players.FirstOrDefault(m => m.EmailAddress.ToLower().Equals(EmailAddress.ToLower()));

            return player?.PlayerID;
        }

        /// <summary>
        /// Read Record by PlayerID
        /// </summary>
        /// <param name="PlayerID"></param>
        /// <param name="OnlyBasicInfo">Only return the Basic info if True, otherwise will include SelectedLeagueID, SelectedLeagueName, PlayerTeamID,  PlayerWeightLogStats in return</param>
        /// <returns></returns>
        public PlayersExt ReadOne(long PlayerID)
        {
            var player = db.Players.Include("Heights").Include("Advertisements").Where(m => m.PlayerID == PlayerID).FirstOrDefault();

            if (player == null)
            { return null; }
            else
            {
                return Map(player);
            }
        }

        public PlayersExt ReadOne_Minimum(long PlayerID)
        {
            var player = db.Players.Include("Heights").Include("Advertisements").Where(m => m.PlayerID == PlayerID).FirstOrDefault();

            if (player == null)
            { return null; }
            else
            {
                return Map_Minimum(player);
            }
        }

        public PlayersExt ReadOne_ByEmailAddress(string EmailAddress, bool MapMinimum = false)
        {
            PlayersExt model = null;

            var player = db.Players.Include("Heights").Include("Advertisements")
                            .Where(m => m.EmailAddress.ToLower() == EmailAddress.ToLower()).FirstOrDefault();

            if (player != null)
            {
                if (MapMinimum)
                {
                    model = Map_Minimum(player);
                }
                else
                {
                    model = Map(player);
                }
            }
            else
            {
                model = null;
            }

            return model;
        }

        #region Player Emergency Details

        public PlayerEmergencyDetails Read_EmergencyDetails(string EmailAddress)
        {
            var player = db.Players.Where(m => m.EmailAddress.ToLower() == EmailAddress.ToLower()).FirstOrDefault();

            if (player != null)
                return MapEmergencyDetails(player);
            else
                return null;
        }

        #endregion Player Emergency Details

        public decimal CalculateBMI(decimal? Weight, long? HeightID)
        {
            decimal BMI = 0.00M;

            var height = db.Heights.FirstOrDefault(m => m.HeightID == HeightID.Value);
            var HeightIn_Feet = height.Height_Value / 100.00M;

            BMI = (Weight.Value / HeightIn_Feet) / HeightIn_Feet;

            return BMI;
        }

        public long CreateOrUpdate(ref PlayersExt model, ref string Msg, ref bool status, Controller ctrl, bool AddTo_NoTeamPlayers = false, long? RegPlayerLeagueID = null)
        {
            long PlayerID = model.PlayerID;
            try
            {
                if (model.PlayerID == 0)
                {
                    //TODO: Map to DB Object
                    model.RegistrationDate = DateTime.Now;
                    var dbmodel = Map(model);

                    //TODO: Save DB Changes and Set the Return Primary Key ID
                    db.Players.Add(dbmodel);
                    db.SaveChanges();

                    //TODO:
                    /*
                    1. Add to PlayerDashboard
                    2. Subscribe to Mailchimp List
                    3. Send Welcome Email with Dashboard URL and Temp Password
                    */

                    //1.Add to PlayerDashboard
                    {
                        string _DashboardURL = SecurityUtils.EncryptText(model.EmailAddress);

                        SecurityUtils.Check_RemoveInvalidURLChar(ref _DashboardURL);

                        PlayerDashboardExt playerDashboardExt = new PlayerDashboardExt()
                        {
                            PlayerID = dbmodel.PlayerID,
                            DashboardURL = _DashboardURL
                        };

                        PlayerDashboardRepository playerDashboardRepo = new PlayerDashboardRepository();
                        playerDashboardRepo.CreateOrUpdate(playerDashboardExt, ref Msg, ctrl);
                    }
                    PlayerID = dbmodel.PlayerID;

                    Msg = "New Player Record Created Successfully";

                    //TOD: Add to Audit Log
                    var playerExt = ReadOne(dbmodel.PlayerID);
                    AuditLog(ctrl, AuditAction.Create, playerExt, null);

                    // 2. Subscribe to Mailchimp List
                    /***MailChimp***/

                    //Whenever new Player Added into DB then add it to MailChimp
                    MailChimpRepository mcRepo = new MailChimpRepository();
                    mcRepo.Subscribe(playerExt);
                }
                else
                {
                    //Update Existing Record
                    var dbmodel = db.Players.FirstOrDefault(p => p.PlayerID == PlayerID);

                    var ForAuditLog = Map(dbmodel);

                    //TODO: Map to DB Object
                    MapUpdate(ref dbmodel, model);
                    //TODO: Update DB Changes
                    db.SaveChanges();
                    PlayerID = dbmodel.PlayerID;

                    Msg = "Player Record Updated Successfully";
                    //TOD: Add to Audit Log
                    var playerExt = ReadOne(dbmodel.PlayerID);
                    AuditLog(ctrl, AuditAction.Update, ForAuditLog, playerExt);

#if Debug == false
                    /***MailChimp***/
                    //Whenever new Player Updated into DB then Update its record in MailChimp
                    MailChimpRepository mcRepo = new MailChimpRepository();
                    mcRepo.UpdateSubscriber(playerExt);
#endif
                }
            }
            catch (Exception ex)
            {
                Msg = ErrorHandling.HandleException(ex);
                status = false;
            }

            return PlayerID;
        }

        //public bool Delete(long PlayerID, ref string Msg, Controller ctrl)
        //{
        //    bool status = true;
        //    try
        //    {
        //        //TODO: Get Current Object from DB
        //        var ItemToDelete = db.Players.FirstOrDefault(m => m.PlayerID == PlayerID);
        //        if (ItemToDelete != null)
        //        {
        //            var ForAudiLog = Map(ItemToDelete);
        //            //TODO: Check if it is not null, then Remove from DB
        //            db.DeletePlayer(PlayerID);

        //            //Add To Log
        //            AuditLog(ctrl, AuditAction.Delete, null, ForAudiLog);

        //            /***MailChimp***/
        //            //Whenever new Player Updated into DB then Update its record in MailChimp
        //            MailChimpRepository mcRepo = new MailChimpRepository();
        //            mcRepo.UnSubscribe(ForAudiLog.EmailAddress);
        //        }
        //    }
        //    catch (Exception ex)
        //    {
        //        Msg = ErrorHandling.HandleException(ex);
        //        status = false;
        //    }

        //    return status;
        //}

        public bool Delete_Archive(long PlayerID, ref string Msg, Controller ctrl)
        {
            bool status = true;
            try
            {
                //TODO: Get Current Object from DB
                var ItemToDelete = db.Players.FirstOrDefault(m => m.PlayerID == PlayerID);
                if (ItemToDelete != null)
                {
                    var ForAudiLog = Map(ItemToDelete);
                    //TODO: Check if it is not null, then Remove from DB
                    //db.DeletePlayer_MoveToArchive(PlayerID);

                    //Add To Log
                    AuditLog(ctrl, AuditAction.Delete, null, ForAudiLog, "and Moved to Players Archive");

                    /***MailChimp***/
                    //Whenever new Player Updated into DB then Update its record in MailChimp
                    //MailChimpRepository mcRepo = new MailChimpRepository();
                    //mcRepo.UnSubscribe(ForAudiLog.EmailAddress);
                }
            }
            catch (Exception ex)
            {
                Msg = ErrorHandling.HandleException(ex);
                status = false;
            }

            return status;
        }

        public Players Map(PlayersExt model)
        {
            Players tblModel = new Players()
            {
                PlayerID = model.PlayerID,
                FirstName = model.FirstName,
                LastName = model.LastName,
                FullName = model.FullName,
                EmailAddress = model.EmailAddress,
                DOB = model.DOB,
                Weight = model.Weight,
                Active = model.Active,
                Mobile = model.Mobile,
                HeightID = model.HeightID,
                PostCode = model.PostCode,
                RegistrationDate = model.RegistrationDate,
                AdvertisementID = model.AdvertisementID,
                AdvertisementOther = model.AdvertisementOther,
                AdvertisementOtherDetails = model.AdvertisementOtherDetails,
            };

            return tblModel;
        }

        public PlayersExt Map_Minimum(Players model)
        {
            PlayersExt tblModel = new PlayersExt()
            {
                PlayerID = model.PlayerID,
                FirstName = model.FirstName,
                LastName = model.LastName,
                FullName = model.FullName,
                EmailAddress = model.EmailAddress,
                DOB = model.DOB,
                Weight = model.Weight,
                RegWeight = model.Weight,
                Active = model.Active,
                Mobile = model.Mobile,
                HeightID = model.HeightID,
                PostCode = model.PostCode,
                RegistrationDate = model.RegistrationDate,
                AdvertisementID = model.AdvertisementID,
                AdvertisementOther = model.AdvertisementOther,
                AdvertisementOtherDetails = model.AdvertisementOtherDetails
            };

            return tblModel;
        }

        public PlayersExt Map(Players model)
        {
            PlayersExt tblModel = new PlayersExt()
            {
                PlayerID = model.PlayerID,
                FirstName = model.FirstName,
                LastName = model.LastName,
                FullName = model.FullName,
                EmailAddress = model.EmailAddress,
                DOB = model.DOB,
                Weight = model.Weight,

                WeightVM = model.Weight,

                Active = model.Active,

                Mobile = model.Mobile,
                HeightID = model.HeightID,
                Height = model.HeightID.HasValue ? model.Heights.Height_Display : "",

                PostCode = model.PostCode,

                Age = (model.Age.HasValue ? model.Age.Value : 0),

                RegistrationDate = model.RegistrationDate,

                AdvertisementID = model.AdvertisementID,
                AdvertisementOther = model.AdvertisementOther,
                AdvertisementOtherDetails = model.AdvertisementOtherDetails,
                Advertisement = model.AdvertisementID.HasValue ? model.Advertisements.Advertisement : ""
             ,
            };

            return tblModel;
        }

        public void MapUpdate(ref Players dbmodel, PlayersExt model)
        {
            dbmodel.PlayerID = model.PlayerID;
            dbmodel.FirstName = model.FirstName;
            dbmodel.LastName = model.LastName;
            dbmodel.FullName = model.FullName;
            dbmodel.EmailAddress = model.EmailAddress;
            dbmodel.DOB = model.DOB;
            dbmodel.Weight = model.Weight;

            dbmodel.Active = model.Active;

            dbmodel.Mobile = model.Mobile;
            dbmodel.HeightID = model.HeightID;

            dbmodel.PostCode = model.PostCode;
        }

        public PlayerEmergencyDetails MapEmergencyDetails(Players model)
        {
            PlayerEmergencyDetails m = new PlayerEmergencyDetails()
            {
                PlayerID = model.PlayerID,
                FullName = model.FullName,
                EmailAddress = model.EmailAddress,
            };

            return m;
        }

        /// <summary>
        /// Add the Action to Audit Log
        /// </summary>
        /// <param name="model">The Object for which this Auditlog took place</param>
        /// <param name="Action">"Added New User OR Updated User Details OR Deleted User"</param>

        private void AuditLog(Controller ctrl, AuditAction auditAction, PlayersExt dbmodel = null, PlayersExt model = null, string CustomMsg = "")
        {
            string AuditLogShortDesc = "", AuditLogLongDesc = "";

            //StringBuilder sb = new StringBuilder();

            if (auditAction == AuditAction.Create) //Creating new Record
            {
                AuditLogShortDesc = AuditLogLongDesc = "New Player has been Added/Registered " + CustomMsg + " Email = " + dbmodel.EmailAddress;

                //sb.Append("<table class='table table-stripped auditLogStyle'>");
                //sb.Append("<tr><th colspan='2'>New User has been Added</th></tr>");
                //sb.Append("<tr><th>Full Name</th><td>" + dbmodel.FullName + "</td></tr>");
                //sb.Append("<tr><th>Email</th><td>" + dbmodel.EmailAddress + "</td></tr>");
                //sb.Append("<tr><th>Role</th><td>" + dbmodel.Role + "</td></tr>");
                //sb.Append("<tr><th>Locked</th><td>" + dbmodel.Locked + "</td></tr>");
                //sb.Append("<tr><th>Deleted</th><td>" + dbmodel.Deleted + "</td></tr>");
                //sb.Append("</table>");

                //AuditLogLongDesc = sb.ToString();
            }
            else if (auditAction == AuditAction.Update)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Player has been Updated " + CustomMsg + " Email = " + dbmodel.EmailAddress;

                //sb.Append("<table class='table table-stripped auditLogStyle'>");

                //sb.Append("<tr><th colspan='2'>User has been Updated</th></tr>");
                //sb.Append("<tr><th colspan='2'><label>BEFORE</label></th></tr>");

                //sb.Append("<tr><th>Full Name</th><td>" + dbmodel.FullName + "</td></tr>");
                //sb.Append("<tr><th>Email</th><td>" + dbmodel.EmailAddress + "</td></tr>");
                //sb.Append("<tr><th>Role</th><td>" + dbmodel.Role + "</td></tr>");
                //sb.Append("<tr><th>Locked</th><td>" + dbmodel.Locked + "</td></tr>");
                //sb.Append("<tr><th>Deleted</th><td>" + dbmodel.Deleted + "</td></tr>");

                //sb.Append("<tr><th colspan='2'><label>AFTER</label></th></tr>");

                //sb.Append("<tr><th>Full Name</th><td>" + dbmodel.FullName + "</td></tr>");
                //sb.Append("<tr><th>Email</th><td>" + dbmodel.EmailAddress + "</td></tr>");
                //sb.Append("<tr><th>Role</th><td>" + dbmodel.Role + "</td></tr>");
                //sb.Append("<tr><th>Locked</th><td>" + dbmodel.Locked + "</td></tr>");
                //sb.Append("<tr><th>Deleted</th><td>" + dbmodel.Deleted + "</td></tr>");

                //sb.Append("</table>");

                //AuditLogLongDesc = sb.ToString();
            }
            else if (auditAction == AuditAction.Delete)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Player has been Deleted " + CustomMsg + " Email = " + model.EmailAddress;

                //sb.Append("<tr><th>Full Name</th><td>" + dbmodel.FullName + "</td></tr>");
                //sb.Append("<tr><th>Email</th><td>" + dbmodel.EmailAddress + "</td></tr>");
                //sb.Append("<tr><th>Role</th><td>" + dbmodel.Role + "</td></tr>");
                //sb.Append("<tr><th>Locked</th><td>" + dbmodel.Locked + "</td></tr>");
                //sb.Append("<tr><th>Deleted</th><td>" + dbmodel.Deleted + "</td></tr>");

                //sb.Append("</table>");

                //AuditLogLongDesc = sb.ToString();
            }

            SecurityUtils.AddAuditLog(AuditLogShortDesc, AuditLogLongDesc, ctrl);
        }

        #region Player Foreign Data

        public List<Heights> ReadAll_Heights()
        {
            return db.Heights.OrderBy(o => o.Height_Value).ToList();
        }

        #endregion Player Foreign Data
    }

    public class PlayersExt
    {
        public long Player_ArchiveID { get; set; }
        public long PlayerID { get; set; }

        [Required(ErrorMessage = "Please enter your First Name")]
        [DisplayName("First Name")]
        public string FirstName { get; set; }

        [Required(ErrorMessage = "Please enter your Last Name")]
        [DisplayName("Last Name")]
        public string LastName { get; set; }

        [DisplayName("Player Name")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Please enter your Email Address")]
        //[EmailAddress(ErrorMessage = "Please enter Valid Email Address")]
        [RegularExpression("^[a-zA-Z0-9_\\.-]+@([a-zA-Z0-9-]+\\.)+[a-zA-Z]{2,8}$", ErrorMessage = "Please enter Valid Email Address")]
        [DisplayName("Email Address")]
        public string EmailAddress { get; set; }

        [DisplayName("DOB")]
        [DisplayFormat(ApplyFormatInEditMode = true, DataFormatString = "{0:yyyy-MM-dd}")]
        public Nullable<System.DateTime> DOB { get; set; }
        public int? Age { get; set; }

        [DisplayName("Weight (KG)")]
        public decimal? Weight { get; set; }

        [DisplayName("Reg. Weight (KG)")]
        public decimal? RegWeight { get; set; }

        public decimal? WeightVM { get; set; }

        [DisplayName("Work Phone")]
        public string WorkPhone { get; set; }

        public string Mobile { get; set; }
        public bool Active { get; set; }
        public bool Featured { get; set; }

        [DisplayName("Registration Date")]
        public DateTime RegistrationDate { get; set; }

        public string Landline { get; set; }

        [DisplayName("Height")]
        public long? HeightID { get; set; }

        public string Height { get; set; }

        [DisplayName("Post Code")]
        [Required(ErrorMessage = "Please enter your Post Code")]
        public string PostCode { get; set; }

        public string Notes { get; set; }

        public decimal? BMI { get; set; }

        [DisplayName("Reg. BMI")]
        public decimal? RegBMI { get; set; }

        [DisplayName("Body Fat")]
        public decimal? BodyFat { get; set; }

        //Other Helping Members
        public string PlayerDefaultImageFileName { get; set; }

        // public string PlayerLeagueName { get; set; }
        public string ValidationMessage { get; set; }

        public bool IsApply { get; set; }

        public Nullable<long> AdvertisementID { get; set; }
        public string Advertisement { get; set; }
        public bool AdvertisementOther { get; set; }
        public string AdvertisementOtherDetails { get; set; }

        [DisplayName("Referral Code")]
        public string ReferralCode { get; set; }


        public List<MessagesExt> ListOfMsgs { get; set; }

        //Only for Player's Archive list
        public Nullable<System.DateTime> ArchiveDateTime { get; set; }

        public PlayersExt()
        {
            Active = true;
            ListOfMsgs = new List<MessagesExt>();
        }

        //Select Lists

        public string PaymentFlag { get; set; }


        [DisplayName("Name")]
        public string Emergency_ContactName { get; set; }

        [DisplayName("Phone")]
        public string Emergency_ContactPhone { get; set; }

        [DisplayName("Medication")]
        public string Emergency_Medication { get; set; }

        public string EmergencyContactPageLink { get; set; }
    }

    public class PlayerProgressGallery
    {
        public long? PlayerID { get; set; }
        public List<PlayerImagesExt> playerImages { get; set; }

        public PlayerProgressGallery()
        {
            playerImages = new List<PlayerImagesExt>();
        }
    }

    public class PlayerEmergencyDetails
    {
        public long PlayerID { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }

        [DisplayName("Name")]
        public string Emergency_ContactName { get; set; }

        [DisplayName("Phone")]
        public string Emergency_ContactPhone { get; set; }

        [AllowHtml]
        [DisplayName("Medication")]
        public string Emergency_Medication { get; set; }
    }

   

    public class PlayersGrid
    {
        public long PlayerID { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public decimal Weight { get; set; }
        public Nullable<decimal> BMI { get; set; }
        public string Position { get; set; }
        public string Mobile { get; set; }
        public string LeagueName { get; set; }
        public Nullable<long> LeagueID { get; set; }
        public string City { get; set; }
        public System.DateTime RegistrationDate { get; set; }
        public bool Active { get; set; }
        public bool Featured { get; set; }
        public string Advertisement { get; set; }
        public bool AdvertisementOther { get; set; }
        public string AdvertisementOtherDetails { get; set; }
        public string TeamName { get; set; }
        public string PaymentFlag { get; set; }
    }
}