using System;

namespace MANvFAT_Scheduler
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            string BaseURL = System.Configuration.ConfigurationManager.AppSettings["BaseURL"].ToString();

            #region 00 - Update GoCardless Payments

            try
            {
                new System.Net.WebClient().DownloadString(BaseURL + "/Automation/UpdateGoCardlessPayments?secret=M@nvF@t@123");
            }
            catch (Exception ex)
            { }

            #endregion 00 - Update GoCardless Payments

            #region 01 - Player Paid for MVFF Book

            try
            {
                new System.Net.WebClient().DownloadString(BaseURL + "/Automation/GenerateAndSendExcel?secret=M@nvF@t@123");
            }
            catch (Exception ex)
            { }

            #endregion 01 - Player Paid for MVFF Book

            #region 02 - Player Not Paid Today or Yesterday and Also Send Player Stat/Progress Emails

            try
            {
                new System.Net.WebClient().DownloadString(BaseURL + "/Automation/PlayersNotPaid?secret=M@nvF@t@123");
            }
            catch (Exception ex)
            { }

            #endregion 02 - Player Not Paid Today or Yesterday and Also Send Player Stat/Progress Emails

            #region 03 - Send Daily Live League Stats

            try
            {
                new System.Net.WebClient().DownloadString(BaseURL + "/Automation/SendDailyLiveLeagueStats?secret=M@nvF@t@123");
            }
            catch (Exception ex)
            { }

            #endregion 03 - Send Daily Live League Stats

            #region 04 - Update Player Scores Breakdown

            try
            {
                new System.Net.WebClient().DownloadString(BaseURL + "/Automation/UpdatePlayerScoresBreakdown?secret=M@nvF@t@123");
            }
            catch (Exception ex)
            { }

            #endregion 04 - Update Player Scores Breakdown

            #region 05 - Update Cities Google Locations

            //Not in Use by Automation, but it's here in case we need to Update the Leagues Latitude/Longitude Manually

            #endregion 05 - Update Cities Google Locations

            #region 06 - Send PayLink to Players which didn't Paid for Deposit

            try
            {
                new System.Net.WebClient().DownloadString(BaseURL + "/Automation/SendPaylinkEmailToPlayersNotPaidForDeposit?secret=M@nvF@t@123");
            }
            catch (Exception ex)
            { }

            #endregion 06 - Send PayLink to Players which didn't Paid for Deposit

            #region 07 - Auto Dismiss RED FLAG Alerts

            try
            {
                new System.Net.WebClient().DownloadString(BaseURL + "/Automation/AutoDismissRedFlagAlerts?secret=M@nvF@t@123");
            }
            catch (Exception ex)
            { }

            #endregion 07 - Auto Dismiss RED FLAG Alerts

            #region 08 - Facebook Leads

            try
            {
                new System.Net.WebClient().DownloadString(BaseURL + "/Automation/UpdateFacebookLeads?secret=M@nvF@t@123");
            }
            catch (Exception ex)
            { }

            #endregion 08 - Facebook Leads

            #region 09 - Dashboard Data Share

            try
            {
                new System.Net.WebClient().DownloadString(BaseURL + "/Automation/DashboardDataShare?secret=M@nvF@t@123");
            }
            catch (Exception ex)
            { }

            #endregion 09 - Dashboard Data Share

            #region 10 - Dashboard Achievements

            try
            {
                new System.Net.WebClient().DownloadString(BaseURL + "/Automation/DashboardAchievements?secret=M@nvF@t@123");
            }
            catch (Exception ex)
            { }

            #endregion 10 - Dashboard Achievements

            #region 11 - Create Recurring Dashboard Notifications

            try
            {
                new System.Net.WebClient().DownloadString(BaseURL + "/Automation/CreateRecurringNotifications?secret=M@nvF@t@123");
            }
            catch (Exception ex)
            { }

            #endregion 11 - Create Recurring Dashboard Notifications

            #region 12 - Delete Dismissed Dashboard Notifications

            try
            {
                new System.Net.WebClient().DownloadString(BaseURL + "/Automation/DeleteDismissedNotifications?secret=M@nvF@t@123");
            }
            catch (Exception ex)
            { }

            #endregion 12 - Delete Dismissed Dashboard Notifications

            #region 13 - Send Email to Players which was Missing this Week, and send another email 2 Days

            try
            {
                new System.Net.WebClient().DownloadString(BaseURL + "/Automation/SendPlayerMissingWeek_2DayAfterEmail?secret=M@nvF@t@123");
            }
            catch (Exception ex)
            { }

            #endregion 13 - Send Email to Players which was Missing this Week, and send another email 2 Days

            Environment.Exit(0);
        }
    }
}