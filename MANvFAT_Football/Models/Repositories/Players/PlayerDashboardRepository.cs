using EntityFramework.BulkInsert.Extensions;
using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Enumerations;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Web.Mvc;

namespace MANvFAT_Football.Models.Repositories
{
    public class PlayerDashboardRepository : BaseRepository
    {
        private Thread EmailThread;

        #region Maintenance

        public List<PlayerDashboardExt> ReadAll()
        {
            var model = db.PlayerDashboard.Include("Players").ToList().Select(m => Map(m)).ToList();

            return model;
        }

        public PlayerDashboardExt ReadOne(long PlayerID)
        {
            var model = db.PlayerDashboard.Include("Players").Where(m => m.PlayerID == PlayerID).FirstOrDefault();
            if (model != null)
                return Map(model);
            else
                return null;
        }

        public PlayerDashboardExt ReadOne(string DashboardURL)
        {
            var model = db.PlayerDashboard.Include("Players").Where(m => m.DashboardURL == DashboardURL).FirstOrDefault();
            if (model != null)
                return Map(model);
            else
                return null;
        }

        public PlayerDashboardExt ValidatePlayerDashboard_HomeLogin(string EmailAddress, string Password, ref string Reason)
        {
            if (string.IsNullOrEmpty(Password))
            {
                Reason = "Please enter Password.";
                return null;
            }
            DateTime TodayDate = DateTime.Now.Date;

            var EncryptedPassword = SecurityUtils.EncryptText(Password);
            var model = db.PlayerDashboard.Include("Players").Where(m => m.Players.EmailAddress.ToLower() == EmailAddress.ToLower() && m.DashboardPassword == EncryptedPassword && m.Locked == false && m.Deleted == false).FirstOrDefault();

            if (model != null)
            {
                if (model.Players.Active == false)
                {
                    Reason = "Your status is not Active.";
                    return null;
                }

                return Map(model);
            }
            else
                return null;
        }

        public PlayerDashboardExt ValidatePlayerDashboard(string DashboardURL, string Password, ref string Reason)
        {
            if(string.IsNullOrEmpty(Password))
            {
                Reason = "Please enter Password.";
                return null;
            }
            DateTime TodayDate = DateTime.Now.Date;

            var EncryptedPassword = SecurityUtils.EncryptText(Password);
            var model = db.PlayerDashboard.Include("Players").Where(m => m.DashboardURL == DashboardURL && m.DashboardPassword == EncryptedPassword && m.Locked == false && m.Deleted == false).FirstOrDefault();

            if (model != null)
            {
                if (model.Players.Active == false)
                {
                    Reason = "Your status is not Active.";
                    return null;
                }

                return Map(model);
            }
            else
                return null;
        }

        public bool ValidateLogin(PlayerDashboardExt playerDashboard, Controller ctrl, ref string Reason)
        {
            bool IsValid = false;
            DateTime TodayDate = DateTime.Now.Date;
            if (playerDashboard != null)
            {
                CookiesRespository cookie = new CookiesRespository();
                var LoginCookie = cookie.GetDashboardLoginCookie(ctrl);

                if (string.IsNullOrEmpty(LoginCookie.LoginProgressDashboard) == false
                    && LoginCookie.LoginProgressDashboard == playerDashboard.PlayerEmailAddress
                    // && playerDashboard.DashboardExpiryDate >= TodayDate
                    && playerDashboard.IsPlayerActive == true)
                {
                    IsValid = true;
                }
            }
            //if (IsValid == false && (playerDashboard.DashboardExpiryDate >= TodayDate) == false)
            //{
            //    Reason = "Your Premium Membership has been Expired.";
            //}
            //else 

            if (IsValid == false && playerDashboard.IsPlayerActive == false)
            {
                Reason = "Your status as a MANvFAT Player is not Active.";
            }
            return IsValid;
        }

        public bool CreateOrUpdate(PlayerDashboardExt model, ref string Msg, Controller ctrl)
        {
            bool status = true;

            if (model.PlayerDashboardID == 0)
            {
                try
                {
                    string RandomPassword = System.Web.Security.Membership.GeneratePassword(6, 1);
                    model.DashboardPassword = SecurityUtils.EncryptText(RandomPassword);

                    //TODO: Map to DB Object
                    var dbmodel = Map(model);
                    //TODO: Save DB Changes and Set the Return Primary Key ID
                    db.PlayerDashboard.Add(dbmodel);
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
                    var dbmodel = db.PlayerDashboard.Include("Players").FirstOrDefault(p => p.PlayerDashboardID == model.PlayerDashboardID);

                    var proceedToSave = true;
                    //Validate Dashboard URL
                    ValidateDashboardURL(dbmodel.DashboardURL, model.DashboardURL, ref Msg, ref status);

                    if (!string.IsNullOrEmpty(model.DashboardPassword))
                    {
                        if (SecurityUtils.DecryptCypher(dbmodel.DashboardPassword).Equals(model.DashboardPassword))
                        {
                            status = false;
                            Msg = "New Password shouldn't be same as your previous Password";
                        }
                    }
                    else
                    {
                        model.DashboardPassword = SecurityUtils.DecryptCypher(dbmodel.DashboardPassword);
                    }

                    proceedToSave = status;

                    if (proceedToSave)
                    {
                        var ForAuditLog = Map(dbmodel);
                        //TODO: Map to DB Object
                        MapUpdate(ref dbmodel, model);
                        //TODO: Update DB Changes
                        db.SaveChanges();

                        //TOD: Add to Audit Log
                        AuditLog(ctrl, AuditAction.Update, ForAuditLog, model);
                    }
                }
                catch (Exception ex)
                {
                    Msg = ErrorHandling.HandleException(ex);
                    status = false;
                }
            }

            return status;
        }

        //public void UpdateDashboardURLs()
        //{
        //    var playerDashboard = db.PlayerDashboard.Include("Players").ToList();
        //    List<PlayerDashboardExt> ListOfPlayerDashboardExt = new List<PlayerDashboardExt>();

        //    foreach (var item in playerDashboard)
        //    {
        //        PlayerDashboardExt model = Map(item);

        //        List<char> invalidURLChar = SecurityUtils.GetInvalidURLCharacters();

        //        if (item.DashboardURL.Contains(" ") || invalidURLChar.Any(m => item.DashboardURL.Contains(m)))
        //        {
        //            string _DashboardURL = item.DashboardURL;

        //            SecurityUtils.Check_RemoveInvalidURLChar(ref _DashboardURL);

        //            item.DashboardURL = _DashboardURL;
        //            db.SaveChanges();

        //            model.DashboardURL = _DashboardURL;

        //            ListOfPlayerDashboardExt.Add(model);
        //        }
        //    }

        //    //Now Re-Send Email to Each Player, the Standalone Email

        //    MandrillRepository mandrillRepo = new MandrillRepository();

        //    foreach (var item in ListOfPlayerDashboardExt)
        //    {
        //        mandrillRepo.ProgressDashboard_Standalone(item);
        //    }

        //}


        //Create Progress Dashboard for All Player of a League if Enable Progress Dashboard checkbox is ticked for this League
        public void CreateProgressDashboardForSinglePlayer(PlayersExt model)
        {
            try
            {
                PlayerDashboardRepository pdRepo = new PlayerDashboardRepository();

                //Create Premium Dashboard Default Setting for User
                string _DashboardURL = SecurityUtils.EncryptText(model.EmailAddress);

                SecurityUtils.Check_RemoveInvalidURLChar(ref _DashboardURL);

                PlayerDashboardExt modelDashExt = new PlayerDashboardExt()
                {
                    PlayerID = model.PlayerID,
                    DashboardURL = _DashboardURL,
                    IsFirstLogin = true,
                    PlayerFullName = model.FullName,
                    PlayerEmailAddress = model.EmailAddress,
                   
                   
                    ReminderTime = 19 //By Default 19:00 when Creating new Player's Dashboard
                };

                string RandomPassword = System.Web.Security.Membership.GeneratePassword(6, 1);
                modelDashExt.DashboardPassword = SecurityUtils.EncryptText(RandomPassword);

                //TODO: Map to DB Object
                var dbmodel = Map(modelDashExt);

                //Save to DB
                db.PlayerDashboard.Add(dbmodel);
                db.SaveChanges();

                //Now Send Email to Each Player, the Standalone Email

                MandrillRepository mandrillRepo = new MandrillRepository();

                mandrillRepo.ProgressDashboard_Standalone(modelDashExt);

                SecurityUtils.AddAuditLog("The Progress Dashboard For Player when Player Added to Team. Success", "For Player Email " + model.EmailAddress);
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);

                string ErrorMsg = ex.Message + " - " + ex.InnerException != null ? ex.InnerException.Message : "";
                SecurityUtils.AddAuditLog("The Progress Dashboard For Player when Player Added to Team. Error Occurred", "For Player Email " + model.EmailAddress + " But Error Occurred: " + ErrorMsg);
            }
        }

      
        public void ValidateDashboardURL(string CurrentDashboardURL, string DashboardURL, ref string Msg, ref bool status)
        {
            List<char> invalidURLChar = SecurityUtils.GetInvalidURLCharacters();

            if (DashboardURL.Contains(" ") || invalidURLChar.Any(m=> DashboardURL.Contains(m)))
            {
                Msg = "Special Characters or Blank space(s) are not allowed in Dashboard URL \"" + DashboardURL + "\" Only Characters and/or Numbers are allowed";
                status = false;
            }

            if (status==true && db.PlayerDashboard.Any(m => m.DashboardURL != CurrentDashboardURL && m.DashboardURL.ToLower().Equals(DashboardURL.ToLower())))
            {
                Msg = "Dashboard URL " + DashboardURL + "is already taken";
                status = false;
            }



        }

        public bool Delete(PlayerDashboardExt model, ref string Msg, Controller ctrl)
        {
            bool status = true;

            //TODO: Get Current Object from DB
            var ItemToDelete = db.PlayerDashboard.Include("Players").FirstOrDefault(m => m.PlayerID == model.PlayerID);
            try
            {
                if (ItemToDelete != null)
                {
                    var ForAudiLog = Map(ItemToDelete);
                    //TODO: Check if it is not null, then Remove from DB
                    db.PlayerDashboard.Remove(ItemToDelete);
                    db.SaveChanges();

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

        #endregion Maintenance

        #region Auto Data Share

        public void AutoDataShare(Controller ctrl)
        {
            DateTime TodayDate = DateTime.Now.Date;
            //Get The Players which are Active and their Dashboard is not Expired

            var result = db.PlayerDashboard.Include("Players").Where(m => m.Players.Active == true  && m.Share_FoodnDrinkDataFrequency.HasValue && m.Share_FoodnDrinkDataWith.HasValue);
            var playerDashboards = result.ToList().Select(m => Map(m)).ToList();

            foreach (var item in playerDashboards)
            {
                bool Proceed = false;
                //check if Daily, Weekly or Monthly

                if (item.ShareDataFrequency == (int)enumShareFrequency.Daily)
                { Proceed = true; }

                //Check if Share Data Frequency is Weekly and Seleted Day of Week is same to Today's Day of week then Send Data otherwise continue to next record in loop
                else if (item.ShareDataFrequency == (int)enumShareFrequency.Weekly && item.DayOfWeek == (int)TodayDate.DayOfWeek)
                { Proceed = true; }

                //Check if Share Data Frequency is Monthly and Today is 1st Date of the month then Send Data otherwise continue to next record in loop
                else if (item.ShareDataFrequency == (int)enumShareFrequency.Monthly && TodayDate.Day == 1)
                { Proceed = true; }

                if (Proceed)
                {
                    string EmailTo = "";
                    string LeagueName = "";
                    List<PlayerDailyActivityCSV> ListOfDailyActivities = new List<PlayerDailyActivityCSV>();
                    List<WeeklyActivityCSV> ListOfWeeklyActivities = new List<WeeklyActivityCSV>();
                    List<PlayerAchievementsExt> ListOfPlayerAchievements = new List<PlayerAchievementsExt>();

                    //LeaguesRepository leagueRepo = new LeaguesRepository();
                    //var league = leagueRepo.GetPlayerSelectedLeague(item.PlayerID);

                    //LeagueName = league.LeagueID.HasValue ? league.LeagueName : "No League Selected";

                    //#region Get Email To Addresses

                    //if (item.ShareDataWith == (int)enumShareWith.Coach)
                    //{
                    //    PlayersRepository playerRepo = new PlayersRepository();
                    //    var coach = playerRepo.GetPlayerLeagueCoach(item.PlayerID);

                    //    EmailTo = coach.CoachUserID.HasValue ? coach.CoachEmail + ";" : "";
                    //}

                    //if (item.ShareDataWith == (int)enumShareWith.Team)
                    //{
                    //    PlayersRepository playerRepo = new PlayersRepository();
                    //    var teamEmails = playerRepo.GetPlayerTeamMemberEmailAddresses(item.PlayerID);

                    //    EmailTo += teamEmails;
                    //}

                    //if (item.ShareDataWith == (int)enumShareWith.All)
                    //{
                    //    PlayersRepository playerRepo = new PlayersRepository();
                    //    var coach = playerRepo.GetPlayerLeagueCoach(item.PlayerID);

                    //    EmailTo = coach.CoachUserID.HasValue ? coach.CoachEmail + ";" : "";

                    //    var teamEmails = playerRepo.GetPlayerTeamMemberEmailAddresses(item.PlayerID);

                    //    EmailTo += teamEmails;
                    //}

                    if (!string.IsNullOrEmpty(item.AdditionalRecipients))
                    {
                        EmailTo += string.IsNullOrEmpty(EmailTo) ? item.AdditionalRecipients : ";" + item.AdditionalRecipients;
                    }

                    #endregion Get Email To Addresses

                    #region Get Daily Activitied List - Food & Drink

                    {
                        ShareActivity model = new ShareActivity()
                        {
                            ShareFrequencyID = item.ShareDataFrequency.HasValue ? item.ShareDataFrequency.Value == 1 ? 2 : item.ShareDataFrequency.Value : 2,
                            PlayerID = item.PlayerID
                        };
                        PlayerDailyActivityRepository pdaRepo = new PlayerDailyActivityRepository();
                        ListOfDailyActivities = pdaRepo.CreateListOfActivities(model, ctrl);
                    }

                    #endregion Get Daily Activitied List - Food & Drink

                    #region Get Weekly Activitied List

                    {
                        ShareActivity model = new ShareActivity()
                        {
                            ShareFrequencyID = item.ShareDataFrequency.HasValue ? item.ShareDataFrequency.Value : 2,
                            PlayerID = item.PlayerID
                        };
                        PlayerWeeklyActivityRepository pwaRepo = new PlayerWeeklyActivityRepository();
                        ListOfWeeklyActivities = pwaRepo.CreateListOfActivities(model, ctrl);
                    }

                    #endregion Get Weekly Activitied List

                    #region Get Achievements List

                    {
                        PlayerAchievementsRepository paRepo = new PlayerAchievementsRepository();
                        ListOfPlayerAchievements = paRepo.ReadAll_Dashboard(item.PlayerID);
                    }

                    #endregion Get Achievements List

                    #region Create Excel Spread sheet with 3 Sheets

                    var path = ctrl.Server.MapPath(SecurityUtils.ActivityData_Path);

                    //if Directory is not exists then create one
                    if (!System.IO.Directory.Exists(path))
                    {
                        System.IO.Directory.CreateDirectory(path);
                    }

                    string FileName = "DashboardData_" + item.PlayerFullName.Replace(" ", string.Empty) + "_" + DateTime.Now.ToString("ddMMyyyyHHmmss") + ".xlsx";
                    string FullFilePath = System.IO.Path.Combine(path, FileName);

                    ExportDashboardDataToExcel(FullFilePath, item.PlayerFullName, item.PlayerEmailAddress, LeagueName, ListOfDailyActivities, ListOfWeeklyActivities, ListOfPlayerAchievements);

                    //TODO: Now Send Email

                    EmailsRepository emailRepo = new EmailsRepository();

                    string Body = "Hi," +
                        "<br/><br/> I would like to share my MANvFAT Progress Data with you." +
                        "<br/><br/> Player Name: " + item.PlayerFullName +
                        "<br/> Player Email Address: " + item.PlayerEmailAddress +
                        "<br/> League Name: " + LeagueName;

                    Guid guid = Guid.NewGuid();
                    EmailThread = new Thread(() =>
                       emailRepo.SendEmailWithAttachment(SecurityUtils.SiteAdminEmail, EmailTo, "Dashboard Auto Data Sharing: " + item.PlayerFullName, Body, FullFilePath, EmailThread, ctrl, null, null));
                    EmailThread.Name = "DashboardDataShare_" + guid.ToString();
                    EmailThread.Start();

                    #endregion Create Excel Spread sheet with 3 Sheets
                }
            }
        }

        public void ExportDashboardDataToExcel(string fullFilePath, string PlayerFullName, string PlayerEmailAddress, string LeagueName, List<PlayerDailyActivityCSV> ListOfDailyActivities, List<WeeklyActivityCSV> ListOfWeeklyActivities, List<PlayerAchievementsExt> ListOfPlayerAchievements)
        {
            //Create new Excel workbook
            var workbook = new XSSFWorkbook();

            GenerateExcelSheet_DailyActivities(PlayerFullName, PlayerEmailAddress, LeagueName, ListOfDailyActivities, ref workbook);
            GenerateExcelSheet_WeeklyActivities(PlayerFullName, PlayerEmailAddress, LeagueName, ListOfWeeklyActivities, ref workbook);
            GenerateExcelSheet_Achievements(PlayerFullName, PlayerEmailAddress, LeagueName, ListOfPlayerAchievements, ref workbook);

            using (FileStream stream = new FileStream(fullFilePath, FileMode.Create, FileAccess.Write))
            {
                workbook.Write(stream);
            }

            MemoryStream output = new MemoryStream();
            workbook.Write(output);
        }

        public void GenerateExcelSheet_DailyActivities(string PlayerFullName, string PlayerEmailAddress, string LeagueName, List<PlayerDailyActivityCSV> ListOfDailyActivities, ref XSSFWorkbook workbook)
        {
            Dictionary<string, int> SheetColumns = new Dictionary<string, int>();

            int ColumnNumber = 0;

            //Create new Excel sheet
            var sheet = workbook.CreateSheet("Daily Activity");

            //Create a header row
            var headerRow = sheet.CreateRow(0);

            SheetColumns.Add("Activity Date", 35);
            SheetColumns.Add("Breakfast", 35);
            SheetColumns.Add("Lunch", 35);
            SheetColumns.Add("Dinner", 35);
            SheetColumns.Add("Snacks", 35);
            SheetColumns.Add("Drink", 35);
            SheetColumns.Add("HowHealthy", 35);

            CreateExcelFile_NPOI.AddHeaderColumns(ref SheetColumns, ref ColumnNumber, ref sheet, ref headerRow);

            //Create Header Font BOLD
            headerRow.GetCell(0).CellStyle =
            headerRow.GetCell(1).CellStyle =
            headerRow.GetCell(2).CellStyle =
            headerRow.GetCell(3).CellStyle =
            headerRow.GetCell(4).CellStyle =
            headerRow.GetCell(5).CellStyle =
            headerRow.GetCell(6).CellStyle = CreateExcelFile_NPOI.CellStyle(ref workbook, true, true, null, null, null, null, null, null, null);

            //Fix the Header Row
            CreateExcelFile_NPOI.FixHeader(ref sheet);

            var DateOnly_Format = CreateExcelFile_NPOI.SetCellStyle_DateOnly(ref workbook);

            int rowNumber = 1;

            foreach (var item in ListOfDailyActivities)
            {
                var row = sheet.CreateRow(rowNumber++);

                row.CreateCell(0).SetCellValue(item.ActivityDate);
                row.CreateCell(1).SetCellValue(item.Breakfast);
                row.CreateCell(2).SetCellValue(item.Lunch);

                row.CreateCell(3).SetCellValue(item.Dinner);
                row.CreateCell(4).SetCellValue(item.Snacks);
                row.CreateCell(5).SetCellValue(item.Drink);
                row.CreateCell(6).SetCellValue(item.HowHealthy.HasValue ? item.HowHealthy.Value.ToString() : "");
            }

            {
                var row = sheet.CreateRow(rowNumber++);
                row.CreateCell(0).SetCellValue("---------------");
            }
            {
                var row = sheet.CreateRow(rowNumber++);
                row.CreateCell(0).SetCellValue("Player Name: " + PlayerFullName);
            }

            {
                var row = sheet.CreateRow(rowNumber++);
                row.CreateCell(0).SetCellValue("Player Email Address: " + PlayerEmailAddress);
            }

            {
                var row = sheet.CreateRow(rowNumber++);
                row.CreateCell(0).SetCellValue("League Name: " + LeagueName);
            }
        }

        public void GenerateExcelSheet_WeeklyActivities(string PlayerFullName, string PlayerEmailAddress, string LeagueName, List<WeeklyActivityCSV> ListOfWeeklyActivities, ref XSSFWorkbook workbook)
        {
            Dictionary<string, int> SheetColumns = new Dictionary<string, int>();

            int ColumnNumber = 0;

            //Create new Excel sheet
            var sheet = workbook.CreateSheet("Weekly Activity");

            //Create a header row
            var headerRow = sheet.CreateRow(0);

            SheetColumns.Add("Activity Date", 35);
            SheetColumns.Add("Activity", 35);
            SheetColumns.Add("Activity Length", 35);
            SheetColumns.Add("Completed", 35);

            CreateExcelFile_NPOI.AddHeaderColumns(ref SheetColumns, ref ColumnNumber, ref sheet, ref headerRow);

            //Create Header Font BOLD
            headerRow.GetCell(0).CellStyle =
            headerRow.GetCell(1).CellStyle =
            headerRow.GetCell(2).CellStyle =
            headerRow.GetCell(3).CellStyle = CreateExcelFile_NPOI.CellStyle(ref workbook, true, true, null, null, null, null, null, null, null);

            //Fix the Header Row
            CreateExcelFile_NPOI.FixHeader(ref sheet);

            var DateOnly_Format = CreateExcelFile_NPOI.SetCellStyle_DateOnly(ref workbook);

            int rowNumber = 1;

            foreach (var item in ListOfWeeklyActivities)
            {
                var row = sheet.CreateRow(rowNumber++);

                row.CreateCell(0).SetCellValue(item.ActivityDate);
                row.CreateCell(1).SetCellValue(item.Activity);
                row.CreateCell(2).SetCellValue(item.ActivityTime);
                row.CreateCell(3).SetCellValue(item.Completed);
            }

            {
                var row = sheet.CreateRow(rowNumber++);
                row.CreateCell(0).SetCellValue("---------------");
            }
            {
                var row = sheet.CreateRow(rowNumber++);
                row.CreateCell(0).SetCellValue("Player Name: " + PlayerFullName);
            }

            {
                var row = sheet.CreateRow(rowNumber++);
                row.CreateCell(0).SetCellValue("Player Email Address: " + PlayerEmailAddress);
            }

            {
                var row = sheet.CreateRow(rowNumber++);
                row.CreateCell(0).SetCellValue("League Name: " + LeagueName);
            }
        }

        public void GenerateExcelSheet_Achievements(string PlayerFullName, string PlayerEmailAddress, string LeagueName, List<PlayerAchievementsExt> ListOfPlayerAchievements, ref XSSFWorkbook workbook)
        {
            Dictionary<string, int> SheetColumns = new Dictionary<string, int>();

            int ColumnNumber = 0;

            //Create new Excel sheet
            var sheet = workbook.CreateSheet("Achievements");

            //Create a header row
            var headerRow = sheet.CreateRow(0);

            SheetColumns.Add("Title", 35);
            SheetColumns.Add("Description", 35);
            SheetColumns.Add("Points", 35);

            CreateExcelFile_NPOI.AddHeaderColumns(ref SheetColumns, ref ColumnNumber, ref sheet, ref headerRow);

            //Create Header Font BOLD
            headerRow.GetCell(0).CellStyle =
            headerRow.GetCell(1).CellStyle =
            headerRow.GetCell(2).CellStyle = CreateExcelFile_NPOI.CellStyle(ref workbook, true, true, null, null, null, null, null, null, null);

            //Fix the Header Row
            CreateExcelFile_NPOI.FixHeader(ref sheet);

            var DateOnly_Format = CreateExcelFile_NPOI.SetCellStyle_DateOnly(ref workbook);

            int rowNumber = 1;

            foreach (var item in ListOfPlayerAchievements)
            {
                var row = sheet.CreateRow(rowNumber++);

                row.CreateCell(0).SetCellValue(item.Title);
                row.CreateCell(1).SetCellValue(item.Description);
                row.CreateCell(2).SetCellValue(item.Points);
            }

            {
                var row = sheet.CreateRow(rowNumber++);
                row.CreateCell(0).SetCellValue("---------------");
            }
            {
                var row = sheet.CreateRow(rowNumber++);
                row.CreateCell(0).SetCellValue("Player Name: " + PlayerFullName);
            }

            {
                var row = sheet.CreateRow(rowNumber++);
                row.CreateCell(0).SetCellValue("Player Email Address: " + PlayerEmailAddress);
            }

            {
                var row = sheet.CreateRow(rowNumber++);
                row.CreateCell(0).SetCellValue("League Name: " + LeagueName);
            }
        }

       

        #region Mapping

        public PlayerDashboard Map(PlayerDashboardExt model)
        {
            PlayerDashboard tblModel = new PlayerDashboard()
            {
                PlayerDashboardID = model.PlayerDashboardID,
                PlayerID = model.PlayerID,
                DashboardURL = model.DashboardURL,
                DashboardPassword = model.DashboardPassword,
            };

            return tblModel;
        }

        public PlayerDashboardExt Map(PlayerDashboard model)
        {
            PlayerDashboardExt tblModel = new PlayerDashboardExt()
            {
                PlayerDashboardID = model.PlayerDashboardID,
                PlayerID = model.PlayerID,
                PlayerFullName = model.Players.FullName,
                PlayerEmailAddress = model.Players.EmailAddress,
                PlayerHeight_cm = model.Players.HeightID.HasValue? model.Players.Heights.Height_Value : 0.00M,
                IsPlayerActive = model.Players.Active,
                DashboardURL = model.DashboardURL,
                DashboardPassword = SecurityUtils.DecryptCypher(model.DashboardPassword),
                TargetWeight = model.TargetWeight,
                PasswordResetCode = model.PasswordResetCode,
                ResetCodeExpiry = model.ResetCodeExpiry,
                Locked = model.Locked,
                Deleted = model.Deleted,
                IsFirstLogin = model.IsFirstLogin,
                PlayerRegistration = model.Players.RegistrationDate,

                //Share Data Settings
                
                OptionalMessage = model.OptionalMessage,
            };

            return tblModel;
        }

        public void MapUpdate(ref PlayerDashboard dbmodel, PlayerDashboardExt model)
        {
            dbmodel.PlayerID = model.PlayerID;
            dbmodel.DashboardURL = model.DashboardURL;
            dbmodel.DashboardPassword = SecurityUtils.EncryptText(model.DashboardPassword);
            dbmodel.IsFirstLogin = false;
            dbmodel.TargetWeight = model.TargetWeight;
            dbmodel.Locked = model.Locked;
            dbmodel.Deleted = model.Deleted;

            //Share Data Settings
         
            dbmodel.OptionalMessage = model.OptionalMessage;
        }

        #endregion Mapping

        /// <summary>
        /// Add the Action to Audit Log
        /// </summary>
        /// <param name="model">The Object for which this Auditlog took place</param>
        /// <param name="Action">"Added New User OR Updated User Details OR Deleted User"</param>

        private void AuditLog(Controller ctrl, AuditAction auditAction, PlayerDashboardExt dbmodel = null, PlayerDashboardExt model = null)
        {
            string AuditLogShortDesc = "", AuditLogLongDesc = "";

            StringBuilder sb = new StringBuilder();

            if (auditAction == AuditAction.Create) //Creating new Record
            {
                AuditLogShortDesc = AuditLogLongDesc = "New Player Dashboard has been Added PlayerID = " + dbmodel.PlayerID + " DashboardURL = " + dbmodel.DashboardURL;
            }
            else if (auditAction == AuditAction.Update)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Player Dashboard has been Updated PlayerID = " + model.PlayerID + " DashboardURL = " + model.DashboardURL;
            }
            else if (auditAction == AuditAction.Delete)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Player Dashboard has been Deleted PlayerID = " + model.PlayerID + " DashboardURL = " + model.DashboardURL;
            }

            SecurityUtils.AddAuditLog(AuditLogShortDesc, AuditLogLongDesc, ctrl);
        }
    }
    public class PlayerProgressDashboard
    {

        public List<PlayerDashboardNotificationExt> Notifications { get; set; }
        public PlayerDashboardExt PlayerDashboardExtension{get;set;}
        public PlayerProgressGallery PlayerProgressGallery { get;set;}
        public PlayerDailyActivityExt PlayerDailyActivityExtension { get; set; }
    }

    public class PlayerDashboardExt
    {
        public long PlayerDashboardID { get; set; }
        public long PlayerID { get; set; }

        public string PlayerFullName { get; set; }
        public string PlayerEmailAddress { get; set; }
        public bool IsPlayerActive { get; set; }

        [DisplayName("Dashboard URL")]
        public string DashboardURL { get; set; }

        [DisplayName("Dashboard Password")]
        [Required(ErrorMessage = "Password required")]
        [StringLength(int.MaxValue, ErrorMessage = "Minimum 6 characters required", MinimumLength = 6)]
        [RegularExpression(@"^((?=.*[A-Z])(?=.*\d)).+$", ErrorMessage = "at least one Capital letter and a Number required")]
        public string DashboardPassword { get; set; }
        public DateTime PlayerRegistration { get; set; } //is the Date When Player Registered
        [DisplayName("Target Weight")]
        public Nullable<decimal> TargetWeight { get; set; }

        public string PasswordResetCode { get; set; }
        public Nullable<System.DateTime> ResetCodeExpiry { get; set; }
        public bool Locked { get; set; }
        public bool Deleted { get; set; }
        public bool IsFirstLogin { get; set; }
        public decimal PlayerHeight_cm { get; set; }

        public Nullable<int> ShareDataFrequency { get; set; }
        public Nullable<int> ShareDataWith { get; set; }
        public int DayOfWeek { get; set; }
        public string AdditionalRecipients { get; set; }
        public string OptionalMessage { get; set; }
        public Nullable<System.DateTime> LastSentDate { get; set; }
        public int? ReminderTime { get; set; }
       public HeaderTextsExt HeaderTexts { get; set; }
    }

    public class PlayerDashboardLogin
    {
        public string DashboardURLId { get; set; }

        [Required(ErrorMessage = "Please enter Password")]
        [DataType(DataType.Password)]
        public string Password { get; set; }

        public string EmailAddress { get; set; }

        public bool IsRememberFor2Weeks { get; set; }

        public string Reason { get; set; }
    }

    public class ShareActivity
    {
        public long PlayerID { get; set; }
        public int ShareFrequencyID { get; set; }
        public DateTime? Activity_ShareDateFrom { get; set; }
        public DateTime? Activity_ShareDateTo { get; set; }
        public string EmailAddress { get; set; }
        public string MsgBody { get; set; }
        public int ActivityTypeId { get; set; } //ActivityTypeId = 1 > Daily Activity & 2 > Weekly Activity

        public string ShareWith { get; set; }
    }
}