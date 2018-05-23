using EntityFramework.BulkInsert.Extensions;
using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace MANvFAT_Football.Models.Repositories
{
    public class AchievementsRepository : BaseRepository
    {
        //Execute after Daily Activity record saved

        #region DailyActivity_AchievementPoints

        public void DailyActivity_AchievementPoints(long PlayerID, Thread thread)
        {
            try
            {
                List<PlayerAchievements> ListOfPlayerAchievements = new List<PlayerAchievements>();

                DateTime TodayDateTime = DateTime.Now;
                DateTime TodayDate = DateTime.Now.Date;

                PlayerDailyActivityRepository pdaRepo = new PlayerDailyActivityRepository();
                var playerDailyActivities = pdaRepo.ReadAll(PlayerID);

                PlayerAchievementsRepository paRepo = new PlayerAchievementsRepository();
                var playerAchievements = paRepo.ReadAll(PlayerID);

                #region First Bite! - User fills in the food and drink diary for the first time.

                if (playerDailyActivities.All(m => m.ActivityDate == TodayDate))
                {
                    AddAchievementToList(PlayerID, enumAchievements._First_Bite, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                }

                #endregion First Bite! - User fills in the food and drink diary for the first time.

                #region Hanging Ten! - User has filled in 10 days of food and drink diary

                {
                    var NumOfDays_Activities = playerDailyActivities.GroupBy(m => m.ActivityDate).Count();

                    if (NumOfDays_Activities == 10)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Hanging_Ten, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                }

                #endregion Hanging Ten! - User has filled in 10 days of food and drink diary

                #region Minimum Effective Dosage - Granted every day that a user logs their food

                if (playerDailyActivities.Any(m => m.ActivityDate == TodayDate))
                {
                    AddAchievementToList(PlayerID, enumAchievements._Minimum_Effective_Dosage, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleDay);
                }

                #endregion Minimum Effective Dosage - Granted every day that a user logs their food

                #region The cake probably wasn’t a lie - The word cake appears in the food diary

                if (playerDailyActivities.Any(m => m.ActivityDate == TodayDate
                 && (CompareStringIfNotEmpty(m.Breakfast, "cake") ||
                     CompareStringIfNotEmpty(m.Lunch, "cake") ||
                     CompareStringIfNotEmpty(m.Dinner, "cake") ||
                     CompareStringIfNotEmpty(m.Snacks, "cake") ||
                     CompareStringIfNotEmpty(m.Drink, "cake"))))
                {
                    AddAchievementToList(PlayerID, enumAchievements._The_cake_probably_wasnt_a_lie, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleDay);
                }

                #endregion The cake probably wasn’t a lie - The word cake appears in the food diary

                #region You checked in on threesquare - User has filled in all three meals in their food and drink diary

                if (playerDailyActivities.Any(m => m.ActivityDate == TodayDate
                 && (!IsStringNullOrEmpty(m.Breakfast) && !IsStringNullOrEmpty(m.Lunch) && !IsStringNullOrEmpty(m.Dinner))))
                {
                    AddAchievementToList(PlayerID, enumAchievements._You_checked_in_on_threesquare, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleDay);
                }

                #endregion You checked in on threesquare - User has filled in all three meals in their food and drink diary

                #region Hydrated - User has logged water in their food diary

                if (playerDailyActivities.Any(m => m.ActivityDate == TodayDate
                 && !IsStringNullOrEmpty(m.Drink)))
                {
                    AddAchievementToList(PlayerID, enumAchievements._Hydrated, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleDay);
                }

                #endregion Hydrated - User has logged water in their food diary

                #region It IS the most important meal of the day - User tracked breakfast in food diary for the first time

                if (playerDailyActivities.Any(m => m.ActivityDate == TodayDate
                 && !IsStringNullOrEmpty(m.Breakfast)))
                {
                    AddAchievementToList(PlayerID, enumAchievements._It_IS_the_most_important_meal_of_the_day, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                }

                #endregion It IS the most important meal of the day - User tracked breakfast in food diary for the first time

                #region Chilled on Sunday - User has filled in 7 days of their food and drink diary

                {
                    DateTime Last7thDayDate = TodayDate.AddDays(-6);

                    var playerDailyActivity_GroupByDates = playerDailyActivities.Where(m => (m.ActivityDate >= Last7thDayDate && m.ActivityDate <= TodayDate) && (!IsStringNullOrEmpty(m.Breakfast) || !IsStringNullOrEmpty(m.Lunch) || !IsStringNullOrEmpty(m.Dinner) || !IsStringNullOrEmpty(m.Snacks) || !IsStringNullOrEmpty(m.Drink))).GroupBy(m => m.ActivityDate);

                    if (playerDailyActivity_GroupByDates.Count() == 7)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Chilled_on_Sunday, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                }

                #endregion Chilled on Sunday - User has filled in 7 days of their food and drink diary

                #region Monster Month, Bridget Jones, Adrian Mole - User has filled in their food and drink diary for 30, 50 or 100 days

                {
                    var playerDailyActivity_GroupByDates = playerDailyActivities.Where(m => (!IsStringNullOrEmpty(m.Breakfast) || !IsStringNullOrEmpty(m.Lunch) || !IsStringNullOrEmpty(m.Dinner) || !IsStringNullOrEmpty(m.Snacks) || !IsStringNullOrEmpty(m.Drink))).GroupBy(m => m.ActivityDate);

                    //Monster Month - User has filled in their food and drink diary for 30 days
                    if (playerDailyActivity_GroupByDates.Count() == 30)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Monster_Month, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                    // Bridget Jones - Player has filled in their food and drink diary 50 times
                    else if (playerDailyActivity_GroupByDates.Count() == 50)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Bridget_Jones, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                    //Adrian Mole - Player has filled in their food and drink diary 100 times
                    else if (playerDailyActivity_GroupByDates.Count() == 100)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Adrian_Mole, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                }

                #endregion Monster Month, Bridget Jones, Adrian Mole - User has filled in their food and drink diary for 30, 50 or 100 days

                #region Snap, Crackle, Pop - Player has filled in their breakfast 10, 50 or 100 times

                {
                    var playerDailyActivity_GroupByDates = playerDailyActivities.Where(m => (!IsStringNullOrEmpty(m.Breakfast))).GroupBy(m => m.ActivityDate);
                    //Snap - Player has filled in their breakfast 10 times

                    if (playerDailyActivity_GroupByDates.Count() == 10)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Snap, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                    //Crackle - Player has filled in their breakfast 50 times
                    else if (playerDailyActivity_GroupByDates.Count() == 50)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Crackle, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                    //Pop - Player has filled in their breakfast 100 times
                    else if (playerDailyActivity_GroupByDates.Count() == 100)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Pop, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                }

                #endregion Snap, Crackle, Pop - Player has filled in their breakfast 10, 50 or 100 times

                #region Let’s do lunch, Let's do lunch again, Still diggin’ that lunch huh - Player has filled in their lunch 10, 50 or 100 times

                {
                    var playerDailyActivity_GroupByDates = playerDailyActivities.Where(m => (!IsStringNullOrEmpty(m.Lunch))).GroupBy(m => m.ActivityDate);
                    //Let’s do lunch	You've tracked lunch 10 times!	1	Player has filled in their lunch 10 times

                    if (playerDailyActivity_GroupByDates.Count() == 10)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Lets_do_lunch, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                    //Let's do lunch again - Player has filled in their lunch 50 times
                    else if (playerDailyActivity_GroupByDates.Count() == 50)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Lets_do_lunch_again, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                    //Still diggin’ that lunch huh - Player has filled in their lunch 100 times
                    else if (playerDailyActivity_GroupByDates.Count() == 100)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Still_diggin_that_lunch_huh, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                }

                #endregion Let’s do lunch, Let's do lunch again, Still diggin’ that lunch huh - Player has filled in their lunch 10, 50 or 100 times

                #region Winner winner tracking dinner, Dinner dinner on the wall, Whoa dinner man - Player has filled in their dinner 10, 50 or 100 times

                {
                    var playerDailyActivity_GroupByDates = playerDailyActivities.Where(m => (!IsStringNullOrEmpty(m.Dinner))).GroupBy(m => m.ActivityDate);
                    //Winner winner tracking dinner - Player has filled in their dinner 10 times

                    if (playerDailyActivity_GroupByDates.Count() == 10)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Winner_winner_tracking_dinner, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                    //Dinner dinner on the wall - Player has filled in their dinner 50 times
                    else if (playerDailyActivity_GroupByDates.Count() == 50)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Dinner_dinner_on_the_wall, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                    //Whoa dinner man - Player has filled in their dinner 100 times
                    else if (playerDailyActivity_GroupByDates.Count() == 100)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Whoa_dinner_man, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                }

                #endregion Winner winner tracking dinner, Dinner dinner on the wall, Whoa dinner man - Player has filled in their dinner 10, 50 or 100 times

                #region Snack attack, Snack attack II, Snack attack III - Player has tracked snacks 10, 50, 100 times

                {
                    var playerDailyActivity_GroupByDates = playerDailyActivities.Where(m => (!IsStringNullOrEmpty(m.Dinner))).GroupBy(m => m.ActivityDate);
                    //Snack attack - Player has tracked snacks 10 times

                    if (playerDailyActivity_GroupByDates.Count() == 10)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Snack_attack, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                    //Snack attack II - Player has tracked snacks 50 times
                    else if (playerDailyActivity_GroupByDates.Count() == 50)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Snack_attack_II, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                    //Snack attack III - Player has tracked snacks 100 times
                    else if (playerDailyActivity_GroupByDates.Count() == 100)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Snack_attack_III, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }
                }

                #endregion Snack attack, Snack attack II, Snack attack III - Player has tracked snacks 10, 50, 100 times

                if (ListOfPlayerAchievements.Count > 0)
                {
                    //TODO: Now do the Bulk Insert into PlayerAchievements
                    db.BulkInsert(ListOfPlayerAchievements);
                    db.SaveChanges();

                    //Now ADD to Notifications
                    AddAchievementToNotification(ListOfPlayerAchievements);
                }

                thread.Join(0);
            }
            catch (ThreadAbortException e)
            {
                SecurityUtils.AddAuditLog("DailyActivity_AchievementPoints", "The DailyActivity_AchievementPoints Thread Aborted Successfully. PlayerID = " + PlayerID);
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
                SecurityUtils.AddAuditLog("DailyActivity_AchievementPoints", "The DailyActivity_AchievementPoints Error. PlayerID = " + PlayerID);
            }
            finally
            {
                SecurityUtils.AddAuditLog("DailyActivity_AchievementPoints", "The DailyActivity_AchievementPoints Thread Completed Successfully. PlayerID = " + PlayerID);
            }
        }

        #endregion DailyActivity_AchievementPoints

        //Execute after Weekly Activity record saved

        #region WeeklyActivity_AchievementPoints

        public void WeeklyActivity_AchievementPoints(long PlayerID, Thread thread)
        {
            try
            {
                List<PlayerAchievements> ListOfPlayerAchievements = new List<PlayerAchievements>();

                DateTime TodayDateTime = DateTime.Now;
                DateTime TodayDate = DateTime.Now.Date;

                var StartDateOfWeek = DateTimeExtensions.StartOfWeek(TodayDate, DayOfWeek.Monday);
                var EndDateOfWeek = StartDateOfWeek.AddDays(6);

                PlayerWeeklyActivityRepository pdwRepo = new PlayerWeeklyActivityRepository();
                var playerWeeklyActivities = pdwRepo.ReadAll(PlayerID);

                PlayerAchievementsRepository paRepo = new PlayerAchievementsRepository();
                var playerAchievements = paRepo.ReadAll(PlayerID);

                #region Gym Class Hero - You completed 100% of your activity goals in 1 week! - Completed 100% of activity in 1 week - 50

                //Only Once, If Player marked all the activities as completed for a week, then apply this achievement

                var playerWeeklyActivities_AWeek = playerWeeklyActivities.Where(m => m.Completed == true && (m.ActivityDate >= StartDateOfWeek && m.ActivityDate <= EndDateOfWeek));

                if (playerWeeklyActivities_AWeek.Count() == 7)
                {
                    AddAchievementToList(PlayerID, enumAchievements._Gym_Class_Hero, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                }

                #endregion Gym Class Hero - You completed 100% of your activity goals in 1 week! - Completed 100% of activity in 1 week - 50

                #region First timer - You logged your first activity! - Logged first activity - 10

                //Only Once, If Player Enter text in Weekly Activity Field, then apply this achievement

                if (playerWeeklyActivities.Any(m => !IsStringNullOrEmpty(m.Activity)))
                {
                    AddAchievementToList(PlayerID, enumAchievements._First_timer, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                }

                #endregion First timer - You logged your first activity! - Logged first activity - 10

                #region Logged Activity for 7, 28, 3 days in a row OR No Activity Logged for the week 7 days

                //Only Once, If Player Enter text in Weekly Activity Field for all 7, 28, 3 consective Days of that Week, then apply this achievement
                {
                    var result = playerWeeklyActivities.Where(m => !IsStringNullOrEmpty(m.Activity) && (m.ActivityDate >= StartDateOfWeek && m.ActivityDate <= EndDateOfWeek)).GroupBy(m => m.ActivityDate).ToList();

                    #region Active - log activity 7 days in a row - Logged activity 7 days in a row - 50

                    if (result.Count() == 7)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Active, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Active - log activity 7 days in a row - Logged activity 7 days in a row - 50

                    #region Lazy Bones - You haven’t logged any activity for 7 days - time to get moving! - No activity logged for 7 days - -50

                    if (result.Count() == 0)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Lazy_Bones, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Lazy Bones - You haven’t logged any activity for 7 days - time to get moving! - No activity logged for 7 days - -50

                    #region Pro Active - log activity 28 days in a row - Logged activity 28 days in a row - 100

                    if (result.Count() == 28)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Pro_Active, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Pro Active - log activity 28 days in a row - Logged activity 28 days in a row - 100

                    #region Consistency is the key - You’ve been active 3 days in a row - keep it up! - Logged activity 3 days in a row - 10

                    if (result.Count() == 3)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Consistency_is_the_key, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Consistency is the key - You’ve been active 3 days in a row - keep it up! - Logged activity 3 days in a row - 10

                    //the object GroupOfWeeklyActivities will contain the list of Player Weekly activities of the same date

                    #region Twofer - You've logged activity twice in one day! - Activity logged twice in one day - 10

                    if (CheckIfPlayerLoggedActivityTwiceADay(result))
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Twofer, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Twofer - You've logged activity twice in one day! - Activity logged twice in one day - 10
                }

                #endregion Logged Activity for 7, 28, 3 days in a row OR No Activity Logged for the week 7 days

                #region Player Logged Activity for 1 Week, 3 Weeks or 6 Weeks

                {
                    var result = playerWeeklyActivities.Where(m => !IsStringNullOrEmpty(m.Activity)).GroupBy(m => m.ActivityDate).ToList();

                    #region Tracker bar - You've tracked activity for 1 week - User has logged activity for 1 week - 25

                    //Only Once, If Player Enter text in Weekly Activity Field for all 7 consective Days of that Week, then apply this achievement

                    if (result.Count() == 7)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Tracker_bar, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Tracker bar - You've tracked activity for 1 week - User has logged activity for 1 week - 25

                    #region Tracker app - You've tracked activity for 3 weeks - User has logged activity for 3 weeks - 25

                    //Only Once, If Player Enter text in Weekly Activity Field for all 21 consective Days of that Week, then apply this achievement

                    if (result.Count() == 21)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Tracker_app, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Tracker app - You've tracked activity for 3 weeks - User has logged activity for 3 weeks - 25

                    #region Tracker from Paw Patrol - You've tracked activity for 6 weeks - User has logged activity for 6 weeks - 25

                    //Only Once, If Player Enter text in Weekly Activity Field for all 42 consective Days of that Week, then apply this achievement

                    if (result.Count() == 42)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Tracker_from_Paw_Patrol, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Tracker from Paw Patrol - You've tracked activity for 6 weeks - User has logged activity for 6 weeks - 25
                }

                #endregion Player Logged Activity for 1 Week, 3 Weeks or 6 Weeks

                #region Player Logged Activity for 1 Week, 3 Weeks or 6 Weeks

                {
                    var result = playerWeeklyActivities.Where(m => !IsStringNullOrEmpty(m.Activity) && (m.ActivityDate >= StartDateOfWeek && m.ActivityDate <= EndDateOfWeek)).ToList();

                    #region NH-Yes - You've logged the NHS-recommended weekly activity target of 150 minutes - User has logged 150 minutes of activity in one week - 50

                    //Only Once, If Player Enter text in Weekly Activity Field and Time total in minutes is equals to 150 minutes for that week, then apply this achievement

                    if (result.Sum(m => m.ActivityTime) == 150)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._NH_Yes, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion NH-Yes - You've logged the NHS-recommended weekly activity target of 150 minutes - User has logged 150 minutes of activity in one week - 50
                }

                #endregion Player Logged Activity for 1 Week, 3 Weeks or 6 Weeks

                if (ListOfPlayerAchievements.Count > 0)
                {
                    //TODO: Now do the Bulk Insert into PlayerAchievements
                    db.BulkInsert(ListOfPlayerAchievements);
                    db.SaveChanges();

                    //Now ADD to Notifications
                    AddAchievementToNotification(ListOfPlayerAchievements);
                }

                thread.Join(0);
            }
            catch (ThreadAbortException e)
            {
                SecurityUtils.AddAuditLog("WeeklyActivity_AchievementPoints", "The WeeklyActivity_AchievementPoints Thread Aborted Successfully. PlayerID = " + PlayerID);
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
                SecurityUtils.AddAuditLog("WeeklyActivity_AchievementPoints", "The WeeklyActivity_AchievementPoints Error. PlayerID = " + PlayerID);
            }
            finally
            {
                SecurityUtils.AddAuditLog("WeeklyActivity_AchievementPoints", "The WeeklyActivity_AchievementPoints Thread Completed Successfully. PlayerID = " + PlayerID);
            }
        }

        #endregion WeeklyActivity_AchievementPoints

        //Execute after Image uploaded OR GIF Created

        #region ImageGallery_AchievementPoints

        public void ImageGallery_AchievementPoints(long PlayerID, Thread thread)
        {
            try
            {
                List<PlayerAchievements> ListOfPlayerAchievements = new List<PlayerAchievements>();

                if (CheckIfPlayerHas_PremiumDashboard(PlayerID) == true)
                {
                    DateTime TodayDateTime = DateTime.Now;
                    DateTime TodayDate = DateTime.Now.Date;

                    PlayerDashboardRepository playerDashboardRepo = new PlayerDashboardRepository();
                    var playerDashboard = playerDashboardRepo.ReadOne(PlayerID);

                    PlayerImagesRepository playerImagesRepo = new PlayerImagesRepository();
                    var playerImages = playerImagesRepo.ReadAll_ProgressGallery(PlayerID);

                    PlayerAchievementsRepository paRepo = new PlayerAchievementsRepository();
                    var playerAchievements = paRepo.ReadAll(PlayerID);

                    #region Looking good! - Upload 3 or more progress photos

                    if (playerImages.Where(m => m.IsAnimated == false).Count() >= 3)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Looking_good, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Looking good! - Upload 3 or more progress photos

                    #region Getting Giffy with it - Player makes their first GIF

                    if (playerImages.Where(m => m.IsAnimated == true).Count() >= 1)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Getting_Giffy_with_it, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Getting Giffy with it - Player makes their first GIF
                }
                if (ListOfPlayerAchievements.Count > 0)
                {
                    //TODO: Now do the Bulk Insert into PlayerAchievements
                    db.BulkInsert(ListOfPlayerAchievements);
                    db.SaveChanges();

                    //Now ADD to Notifications
                    AddAchievementToNotification(ListOfPlayerAchievements);
                }
                thread.Join(0);
            }
            catch (ThreadAbortException e)
            {
                SecurityUtils.AddAuditLog("The ImageGallery_AchievementPoints Thread Aborted Successfully", "PlayerID = " + PlayerID);
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
                SecurityUtils.AddAuditLog("ImageGallery_AchievementPoints Error", "PlayerID = " + PlayerID);
            }
            finally
            {
                SecurityUtils.AddAuditLog("The ImageGallery_AchievementPoints Thread Completed Successfully", "PlayerID = " + PlayerID);
            }
        }

        #region ImageGallery_BeforeAfterImageCreated_AchievementPoints

        //called from BeforeAfterImageCreated Function
        public void ImageGallery_BeforeAfterImageCreated_AchievementPoints(long PlayerID, Thread thread)
        {
            try
            {
                List<PlayerAchievements> ListOfPlayerAchievements = new List<PlayerAchievements>();

                DateTime TodayDateTime = DateTime.Now;
                DateTime TodayDate = DateTime.Now.Date;

                if (CheckIfPlayerHas_PremiumDashboard(PlayerID) == true)
                {
                    PlayerAchievementsRepository paRepo = new PlayerAchievementsRepository();
                    var playerAchievements = paRepo.ReadAll(PlayerID);

                    #region Collage Injection - User made first collage

                    AddAchievementToList(PlayerID, enumAchievements._Collage_Injection, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);

                    #endregion Collage Injection - User made first collage
                }

                if (ListOfPlayerAchievements.Count > 0)
                {
                    //TODO: Now do the Bulk Insert into PlayerAchievements
                    db.BulkInsert(ListOfPlayerAchievements);
                    db.SaveChanges();
                }
                thread.Join(0);
            }
            catch (ThreadAbortException e)
            {
                SecurityUtils.AddAuditLog("The ImageGallery_BeforeAfterImageCreated_AchievementPoints Thread Aborted Successfully", "PlayerID = " + PlayerID);
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
                SecurityUtils.AddAuditLog("ImageGallery_BeforeAfterImageCreated_AchievementPoints Error", "PlayerID = " + PlayerID);
            }
            finally
            {
                SecurityUtils.AddAuditLog("The ImageGallery_BeforeAfterImageCreated_AchievementPoints Thread Completed Successfully", "PlayerID = " + PlayerID);
            }
        }

        #endregion ImageGallery_BeforeAfterImageCreated_AchievementPoints

        #endregion ImageGallery_AchievementPoints

        //This must be triggered from League Reset Function, Must Execute before League Data Reset


        /*
         #region LeagueReset_AchievementPoints
    public void LeagueReset_AchievementPoints(long LeagueID)
    {
        try
        {
            DateTime TodayDateTime = DateTime.Now;
            DateTime TodayDate = DateTime.Now.Date;

            List<PlayerAchievements> ListOfPlayerAchievements = new List<PlayerAchievements>();

            PlayerWeightWeeksRepository pwwRepo = new PlayerWeightWeeksRepository();
            var LeaguePlayerIDs = pwwRepo.ReadAll_PlayerIDs_ByLeague(LeagueID);

            LeaguesRepository leagueRepo = new LeaguesRepository();

            var LeaguePitchRecord = leagueRepo.ReadLeagueFixtureStats(LeagueID, LeagueFixtureType.Pitch);
            var Pitch_Team_WithMaxPts = LeaguePitchRecord.Max(m => m.TeamTotalPts_Pts);
            var Pitch_Team_WithWinsTheSeason = LeaguePitchRecord.FirstOrDefault(m => m.TeamTotalPts_Pts == Pitch_Team_WithMaxPts).TeamID;

            var LeagueScaleRecord = leagueRepo.ReadLeagueFixtureStats(LeagueID, LeagueFixtureType.Scale);
            var Scale_Team_WithMaxPts = LeagueScaleRecord.Max(m => m.TeamTotalPts_Pts);
            var Scale_Team_WithWinsTheSeason = LeagueScaleRecord.FirstOrDefault(m => m.TeamTotalPts_Pts == Scale_Team_WithMaxPts).TeamID;

            TeamPlayersRepository teamPlayersRepo = new TeamPlayersRepository();
            TeamPlayersExt teamPlayer = null;

            LeagueFixturesRepository modelRepo_Fixtures = new LeagueFixturesRepository();
            var TopScorerPerLeague = modelRepo_Fixtures.GetTopScorersPerLeague(LeagueID);
            var MAX_TopScore = TopScorerPerLeague.Max(m => m.WeightGoals);
            var MAX_TopScorerOfLeague = TopScorerPerLeague.FirstOrDefault(m => m.WeightGoals == MAX_TopScore);

            foreach (var _PlayerID in LeaguePlayerIDs)
            {
                long PlayerID = _PlayerID.Value;

                //if Player didn't have PRemium Dashboard then move to next Player Record and ignore rest
                if (CheckIfPlayerHas_PremiumDashboard(PlayerID) == false)
                {
                    continue;
                }

                teamPlayer = teamPlayersRepo.GetPlayerTeam(PlayerID);

                PlayerDailyActivityRepository pdaRepo = new PlayerDailyActivityRepository();
                var playerDailyActivities = pdaRepo.ReadAll(PlayerID);

                PlayerWeeklyActivityRepository pdwRepo = new PlayerWeeklyActivityRepository();
                var playerWeeklyActivities = pdwRepo.ReadAll(PlayerID);

                PlayerAchievementsRepository paRepo = new PlayerAchievementsRepository();
                var playerAchievements = paRepo.ReadAll(PlayerID);

                var playerSelectedLeague = leagueRepo.GetPlayerSelectedLeague(PlayerID);

                LeaguesExt league = null;

                if (playerSelectedLeague.LeagueID.HasValue)
                    league = leagueRepo.ReadOne(playerSelectedLeague.LeagueID.Value);

                if (league.DateTimeFrom.HasValue)
                {
                    DateTime StartDate = league.DateTimeFrom.Value.Date;
                    DateTime EndDate = StartDate.Date.AddDays(98);

                    int TotalSeasonDays = (EndDate - StartDate).Days;

                    #region Daily Activity Achievements after League Reset

                    #region Captain’s Log - User has filled in food and drink diary for entire season

                    var playerDailyActivityDates = playerDailyActivities.Where(p => p.ActivityDate >= StartDate && p.ActivityDate <= EndDate).GroupBy(m => m.ActivityDate);
                    //if total Unique Dates in Player Daily Activity table is equals to total season dates i.e. 98 then apply this Achievement
                    int TotalDailyActivityDays = playerDailyActivityDates.Count();

                    if (TotalSeasonDays == TotalDailyActivityDays)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Captains_Log, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);
                    }

                    #endregion Captain’s Log - User has filled in food and drink diary for entire season

                    #endregion Daily Activity Achievements after League Reset

                    #region Weekly Activity Achievements after League Reset

                    var playerWeeklyActivites_dates = playerWeeklyActivities.Where(p => p.ActivityDate >= StartDate && p.ActivityDate <= EndDate && !IsStringNullOrEmpty(p.Activity)).GroupBy(m => m.ActivityDate);
                    //if total Unique Dates in Player Daily Activity table is equals to total season dates i.e. 98 then apply this Achievement
                    int TotalWeeklyActivityDays = playerWeeklyActivites_dates.Count();

                    #region Hyper Active - You've logged activity every day for one season - ACTIVITY - Logged activity every day for one season - 250

                    if (TotalSeasonDays == TotalWeeklyActivityDays)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Hyper_Active, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);
                    }

                    #endregion Hyper Active - You've logged activity every day for one season - ACTIVITY - Logged activity every day for one season - 250

                    #region Couch Potato - You haven’t logged any activity for a season - ACTIVITY - No activity logged for one season - -50

                    if (TotalWeeklyActivityDays == 0)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Couch_Potato, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);
                    }

                    #endregion Couch Potato - You haven’t logged any activity for a season - ACTIVITY - No activity logged for one season - -50

                    #endregion Weekly Activity Achievements after League Reset

                    #endregion LeagueReset_AchievementPoints
                }

                #region Player Images Achievement after League Reset

                PlayerImagesRepository playerImagesRepo = new PlayerImagesRepository();
                var playerImages = playerImagesRepo.ReadAll(PlayerID, false, true);

                #region Vampire - You haven’t uploaded a progress pic for a whole season - TRACKING - Player has not uploaded a progress pic for the whole season - -100

                if (playerImages.Count() == 0)
                {
                    AddAchievementToList(PlayerID, enumAchievements._Vampire, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);
                }

                #endregion Vampire - You haven’t uploaded a progress pic for a whole season - TRACKING - Player has not uploaded a progress pic for the whole season - -100

                #endregion Player Images Achievement after League Reset

                #region They think it’s all over… - You've played one whole season! - FOOTBALL - User has completed one season - 50

                AddAchievementToList(PlayerID, enumAchievements._They_think_its_all_over, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);

                #endregion They think it’s all over… - You've played one whole season! - FOOTBALL - User has completed one season - 50

                #region We are the champions - Your team won! - FOOTBALL - User’s team wins the season - 250

                //teamPlayer.TeamID == Team_WithWinsTheSeason Player belongs to a team which Win the season
                if (teamPlayer != null && teamPlayer.TeamID == Pitch_Team_WithWinsTheSeason)
                {
                    AddAchievementToList(PlayerID, enumAchievements._We_are_the_champions, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);
                }

                #endregion We are the champions - Your team won! - FOOTBALL - User’s team wins the season - 250

                #region Scale x Tricks - You won a game on the scales that was lost on the pitch - FOOTBALL - User's team wins a game on the scales that was lost on the pitch - 50

                //teamPlayer.TeamID == Scale_Team_WithWinsTheSeason Player belongs to a team which Win the season on Scale but teamPlayer.TeamID != Pitch_Team_WithWinsTheSeason Didn't Win (Lose) on Pitch
                if (teamPlayer != null && teamPlayer.TeamID == Scale_Team_WithWinsTheSeason && teamPlayer.TeamID != Pitch_Team_WithWinsTheSeason)
                {
                    AddAchievementToList(PlayerID, enumAchievements._Scale_x_Tricks, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);
                }

                #endregion Scale x Tricks - You won a game on the scales that was lost on the pitch - FOOTBALL - User's team wins a game on the scales that was lost on the pitch - 50

                #region Tipped the scales - won a game on the pitch that was lost on the scales - FOOTBALL - User’s team wins a game on the pitch that was lost on the scales - 50

                //teamPlayer.TeamID == Pitch_Team_WithWinsTheSeason Player belongs to a team which Win the season on Pitch but teamPlayer.TeamID != Scale_Team_WithWinsTheSeason Didn't Win (Lose) on Scale
                if (teamPlayer != null && teamPlayer.TeamID == Pitch_Team_WithWinsTheSeason && teamPlayer.TeamID != Scale_Team_WithWinsTheSeason)
                {
                    AddAchievementToList(PlayerID, enumAchievements._Tipped_the_scales, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);
                }

                #endregion Tipped the scales - won a game on the pitch that was lost on the scales - FOOTBALL - User’s team wins a game on the pitch that was lost on the scales - 50

                #region Three Games Won - Your team has won 3 games - FOOTBALL - User's team wins 3 games in a row - 100

                //LeaguePitchRecord on the pitch player team wins 3 times then apply this achievement
                if (teamPlayer != null)
                {
                    var IsPlayersTeamWin3Times = LeaguePitchRecord.Any(m => m.TeamID == teamPlayer.TeamID && m.TeamTotalWon_W == 3);
                    if (IsPlayersTeamWin3Times)
                    {
                        AddAchievementToList(PlayerID, enumAchievements._Three_Games_Won, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);
                    }
                }

                #endregion Three Games Won - Your team has won 3 games - FOOTBALL - User's team wins 3 games in a row - 100
            }

            var ListOfData = pwwRepo.ReadAll_ByPlayerIDs(LeaguePlayerIDs);

            foreach (var item in ListOfData)
            {
                //if Player didn't have PRemium Dashboard then move to next Player Record and ignore rest
                if (CheckIfPlayerHas_PremiumDashboard(item.PlayerID) == false)
                {
                    continue;
                }

                PlayerAchievementsRepository paRepo = new PlayerAchievementsRepository();
                var playerAchievements = paRepo.ReadAll(item.PlayerID);

                #region No gain, no pain - You haven't gained weight all season! - User records only losses for a whole season - 250

                if (item.LossWk1 > 0 && item.LossWk2 > 0 && item.LossWk3 > 0 && item.LossWk4 > 0 && item.LossWk5 > 0 && item.LossWk6 > 0 && item.LossWk7 > 0 && item.LossWk8 > 0 && item.LossWk9 > 0 && item.LossWk10 > 0 && item.LossWk11 > 0 && item.LossWk12 > 0 && item.LossWk13 > 0 && item.LossWk14 > 0)
                {
                    AddAchievementToList(item.PlayerID, enumAchievements._No_gain_no_pain, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);
                }

                #endregion No gain, no pain - You haven't gained weight all season! - User records only losses for a whole season - 250

                #region The weigh-in is the hardest part - Miss a weigh-in - User misses a weigh in - -100

                if (CheckIfAnyWeekWeighInIsMissing(item))
                {
                    AddAchievementToList(item.PlayerID, enumAchievements._The_weigh_in_is_the_hardest_part, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);
                }

                #endregion The weigh-in is the hardest part - Miss a weigh-in - User misses a weigh in - -100

                #region High maintenance - You've been on the maintenance program for a whole season - User is on maintenance program for whole season - 250

                //if BMIUnder25 flag is true, it means player is on Maintenance at the time of League Reset
                if (item.BMIUnder25)
                {
                    AddAchievementToList(item.PlayerID, enumAchievements._High_maintenance, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);
                }

                #endregion High maintenance - You've been on the maintenance program for a whole season - User is on maintenance program for whole season - 250

                #region Committed - Attended every weigh in for one season - User attends every weigh in for one season - 250

                if (CheckIfPlayerDidNotMissAnyWeekWeighIn(item))
                {
                    AddAchievementToList(item.PlayerID, enumAchievements._Committed, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);
                }

                #endregion Committed - Attended every weigh in for one season - User attends every weigh in for one season - 250

                #region They can’t blame you - Your team lost on the pitch but you lost on the scales. - FOOTBALL - User’s team loses on the pitch but they lose on the scales. - 25

                //This achievement triggers when a player's team LOSES the game. BUT this particular player has lost weight that week. So the system looks for games that a player's team has lost, but where the player has lost weight.
                //teamPlayer.TeamID != Team_WithWinsTheSeason Player belongs to a team which didn't Win the season, BUT item.TotalLost > 0 Player Lose weight
                if (teamPlayer != null && teamPlayer.TeamID != Pitch_Team_WithWinsTheSeason && item.TotalLost > 0)
                {
                    AddAchievementToList(item.PlayerID, enumAchievements._They_cant_blame_you, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);
                }

                #endregion They can’t blame you - Your team lost on the pitch but you lost on the scales. - FOOTBALL - User’s team loses on the pitch but they lose on the scales. - 25
            }


            #region MVP - You've lost the most weight on your team - FOOTBALL - User loses the most weight on their team - 50

            //GetHowManyWeeksPlayer_LoseWeight
            List<PlayerLostWeightInTeam> PlayerWeightLostInTeam = new List<PlayerLostWeightInTeam>();
            foreach (var item in ListOfData)
            {
                PlayerLostWeightInTeam playerLostWeightInTeam = new PlayerLostWeightInTeam()
                {
                    PlayerID = item.PlayerID,
                    TeamName = item.TeamName,
                    TotalWeekWeightLost = GetHowManyWeeksPlayer_LoseWeight(item)
                };

                PlayerWeightLostInTeam.Add(playerLostWeightInTeam);
            }

            var group_playerLostWeightInTeam = PlayerWeightLostInTeam.GroupBy(m => m.TeamName);

            foreach (var group in group_playerLostWeightInTeam)
            {
                int MaxWeekWeightLost = 0;
                foreach (var item in group)
                {
                    if (MaxWeekWeightLost < item.TotalWeekWeightLost)
                    {
                        MaxWeekWeightLost = item.TotalWeekWeightLost;
                    }
                }

                var playersMAX_WeekLostWeightInTeam = PlayerWeightLostInTeam.Where(m => m.TotalWeekWeightLost == MaxWeekWeightLost);

                foreach (var item_Player in playersMAX_WeekLostWeightInTeam)
                {
                    //if Player didn't have PRemium Dashboard then move to next Player Record and ignore rest
                    if (CheckIfPlayerHas_PremiumDashboard(item_Player.PlayerID) == false)
                    {
                        continue;
                    }

                    PlayerAchievementsRepository paRepo = new PlayerAchievementsRepository();
                    var playerAchievements = paRepo.ReadAll(item_Player.PlayerID);

                    AddAchievementToList(item_Player.PlayerID, enumAchievements._MVP, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);
                }
            }

            #endregion MVP - You've lost the most weight on your team - FOOTBALL - User loses the most weight on their team - 50

            #region Goal machine - You’re the top scorer on your team! - FOOTBALL - User is the top scorer on his team - 100

            if (MAX_TopScorerOfLeague != null)
            {
                //if Player didn't have PRemium Dashboard then move to next Player Record and ignore rest
                if (CheckIfPlayerHas_PremiumDashboard(MAX_TopScorerOfLeague.PlayerID) == true)
                {
                    PlayerAchievementsRepository paRepo = new PlayerAchievementsRepository();
                    var playerAchievements = paRepo.ReadAll(MAX_TopScorerOfLeague.PlayerID);

                    AddAchievementToList(MAX_TopScorerOfLeague.PlayerID, enumAchievements._Goal_machine, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);
                }
            }

            #endregion Goal machine - You’re the top scorer on your team! - FOOTBALL - User is the top scorer on his team - 100

            if (ListOfPlayerAchievements.Count > 0)
            {
                //TODO: Now do the Bulk Insert into PlayerAchievements
                db.BulkInsert(ListOfPlayerAchievements);
                db.SaveChanges();

                //Now ADD to Notifications
                AddAchievementToNotification(ListOfPlayerAchievements);
            }
        }
        catch (Exception ex)
        {
            ErrorHandling.HandleException(ex);
            SecurityUtils.AddAuditLog("LeagueReset_AchievementPoints Error", "LeagueID = " + LeagueID);
        }
    }
    */
        //Execute Every Night Automatically

        #region AutoTrigger_AchievementPoints

        public void AutoTrigger_AchievementPoints()
        {
            try
            {
                List<PlayerAchievements> ListOfPlayerAchievements = new List<PlayerAchievements>();

                DateTime TodayDateTime = DateTime.Now;
                DateTime TodayDate = DateTime.Now.Date;

                PlayerDashboardRepository playerDashboardRepo = new PlayerDashboardRepository();
                var playerDashboards = playerDashboardRepo.ReadAll();

                foreach (var item in playerDashboards)
                {
                    //if Player didn't have PRemium Dashboard then move to next Player Record and ignore rest
                    if (CheckIfPlayerHas_PremiumDashboard(item.PlayerID) == false)
                    {
                        continue;
                    }

                    PlayerImagesRepository playerImagesRepo = new PlayerImagesRepository();
                    var playerImages = playerImagesRepo.ReadAll(item.PlayerID, false, false);

                    PlayerAchievementsRepository paRepo = new PlayerAchievementsRepository();
                    var playerAchievements = paRepo.ReadAll(item.PlayerID);

                    DateTime _2WeekDate = item.PlayerRegistration.AddDays(17);

                    #region Not Vain! - A player hasn't uploaded a progress photo within 2 weeks of starting the progress dashboard.

                    if (playerImages.Count() == 0 && TodayDate >= _2WeekDate)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Not_Vain, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Not Vain! - A player hasn't uploaded a progress photo within 2 weeks of starting the progress dashboard.

                    #region Twomescent  - unlocked 2 achievements - SYSTEM - User has unlocked 2 achievements - 5

                    //Only Once, Automatic over night Process, System go thru all the players which has Premium Dashboard, any player have 2 Achievements then apply this Achievement Points to that Player

                    if (playerAchievements.Count == 2)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Twomescent, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Twomescent  - unlocked 2 achievements - SYSTEM - User has unlocked 2 achievements - 5

                    #region Ten Ting - unlocked 10 achievements - SYSTEM - User has unlocked 10 achievements - 10

                    //Only Once, Automatic over night Process, System go thru all the players which has Premium Dashboard, any player have 10 Achievements then apply this Achievement Points to that Player

                    if (playerAchievements.Count == 10)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Ten_Ting, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Ten Ting - unlocked 10 achievements - SYSTEM - User has unlocked 10 achievements - 10

                    #region Completer - Unlocked all the achievements - SYSTEM - User has unlocked all achievements - 100

                    //Only Once, Automatic over night Process, System go thru all the players which has Premium Dashboard, any player have 145 But it could be changed, because we have ignored or deleted some of the achievements Achievements (currently we have 145 Achievements in total), then apply this Achievement Points to that Player

                    if (playerAchievements.Count == 145)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Completer, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Completer - Unlocked all the achievements - SYSTEM - User has unlocked all achievements - 100
                }

                if (ListOfPlayerAchievements.Count > 0)
                {
                    //TODO: Now do the Bulk Insert into PlayerAchievements
                    db.BulkInsert(ListOfPlayerAchievements);
                    db.SaveChanges();

                    //Now ADD to Notifications
                    AddAchievementToNotification(ListOfPlayerAchievements);

                    SecurityUtils.AddAuditLog("AutoTrigger_AchievementPoints", "AutoTrigger_AchievementPoints Bulk Insert Completed ListOfPlayerAchievements.Count = " + ListOfPlayerAchievements.Count);
                }
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
                SecurityUtils.AddAuditLog("AutoTrigger_AchievementPoints", "AutoTrigger_AchievementPoints Error Occurred Message = " + ex.Message);
            }
        }

        #endregion AutoTrigger_AchievementPoints

        //Player Weight data Input (Leagues => Players Tab), system will check after every weigh in

        #region Weight input Weekly
            /*
        public void WeightInWeekly_AchievementPoints(IEnumerable<PlayerWeightWeeksExt> ListOfModel, Thread thread)
        {
            try
            {
                DateTime TodayDateTime = DateTime.Now;
                DateTime TodayDate = DateTime.Now.Date;

                List<PlayerAchievements> ListOfPlayerAchievements = new List<PlayerAchievements>();

                var PlayerIDs = ListOfModel.Select(m => m.PlayerID).ToList();

                PlayerWeightWeeksRepository pwwRepo = new PlayerWeightWeeksRepository();
                var ListOfData = pwwRepo.ReadAll_ByPlayerIDs(PlayerIDs);

                foreach (var item in ListOfData)
                {
                    //if Player didn't have PRemium Dashboard then move to next Player Record and ignore rest
                    if (CheckIfPlayerHas_PremiumDashboard(item.PlayerID) == false)
                    {
                        continue;
                    }

                    PlayerAchievementsRepository paRepo = new PlayerAchievementsRepository();
                    var playerAchievements = paRepo.ReadAll(item.PlayerID);

                    #region BMI baby - You hit a healthy BMI within 9 months of joining! - User’s BMI goes under 25 within 9 months of joining MAN v FAT - 500

                    int RegMonthDiff = MonthDifference(item.PlayerRegistrationDateTime.Date, TodayDate);

                    if (item.CurrentBMI < 25 && RegMonthDiff == 9)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._BMI_baby, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion BMI baby - You hit a healthy BMI within 9 months of joining! - User’s BMI goes under 25 within 9 months of joining MAN v FAT - 500

                    #region 5%er - You’ve lost 5% of your starting weight! - User loses 5% of starting weight - 100

                    if (item.LossPercent == 5M)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._5_Percenter, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 5%er - You’ve lost 5% of your starting weight! - User loses 5% of starting weight - 100

                    #region 10%er - You’ve lost 10% of your starting weight! - User loses 10% of starting weight - 100

                    if (item.LossPercent == 10M)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._10_Percenter, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 10%er - You’ve lost 10% of your starting weight! - User loses 10% of starting weight - 100

                    #region Not a loser - Three weigh-ins with no weight gain! - User goes 3 weeks without a weight gain - 50

                    if (CheckIfPlayerCurrentAndLast2WeekLose(item))
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Not_a_loser, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Not a loser - Three weigh-ins with no weight gain! - User goes 3 weeks without a weight gain - 50

                    #region 3’s a crowd - Lose weight 3 weeks in a row - User loses weight 3 weeks in a row - 50

                    if (CheckIfPlayerLoseWeightInLast3Week(item))
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._3s_a_crowd, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 3’s a crowd - Lose weight 3 weeks in a row - User loses weight 3 weeks in a row - 50

                    #region Nobese - Your BMI is no longer obese! - User's BMI is no longer classed as obese - 50

                    if (item.RegBMI >= 30 && item.CurrentBMI < 30)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Nobese, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Nobese - Your BMI is no longer obese! - User's BMI is no longer classed as obese - 50

                    #region 28 Days Lighter - Record only losses (no plateaus) over 28 days - User records only losses for 28 days - 100

                    if (CheckIfPlayerLoseWeightForFirst4Weeks(item))
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._28_Days_Lighter, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 28 Days Lighter - Record only losses (no plateaus) over 28 days - User records only losses for 28 days - 100

                    #region Gain pain - You've gained weight - User gains weight - -10

                    if (CheckIfPlayerGainWait(item))
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Gain_pain, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Gain pain - You've gained weight - User gains weight - -10

                    #region Camped - Weight recorded is identical for 2 weeks - User’s weight stays the same for 2 weeks - 10

                    if (CheckIfCurrentAndPreviousWeekWeightSame(item))
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Camped, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Camped - Weight recorded is identical for 2 weeks - User’s weight stays the same for 2 weeks - 10

                    #region Great start! - Lose weight from reg to week 1 - User records weight loss in first week - 50

                    if (item.LossWk1.HasValue && item.LossWk1.Value > 0)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Great_start, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Great start! - Lose weight from reg to week 1 - User records weight loss in first week - 50

                    #region Back 2 it - lose weight after putting on weight the previous week - User loses weight after gaining weight the previous week - 25

                    if (CheckIfCurrentLose_PreviousWeekGain(item))
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Back_2_it, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleDay);
                    }

                    #endregion Back 2 it - lose weight after putting on weight the previous week - User loses weight after gaining weight the previous week - 25

                    #region 15%er - You've lost 15% of your starting weight! - User loses 15% of starting weight - 100

                    if (item.LossPercent == 15M)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._15_Percenter, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 15%er - You've lost 15% of your starting weight! - User loses 15% of starting weight - 100

                    #region 20%er - You've lost 20% of your starting weight! - User loses 20% of starting weight - 100

                    if (item.LossPercent == 20M)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._20_Percenter, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 20%er - You've lost 20% of your starting weight! - User loses 20% of starting weight - 100

                    #region 25%er - You've lost 25% of your starting weight! - User loses 25% of starting weight - 100

                    if (item.LossPercent == 25M)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._25_Percenter, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 25%er - You've lost 25% of your starting weight! - User loses 25% of starting weight - 100

                    #region 30%er - You've lost 30% of your starting weight! - User loses 30% of starting weight - 100

                    if (item.LossPercent == 30M)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._30_Percenter, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 30%er - You've lost 30% of your starting weight! - User loses 30% of starting weight - 100

                    #region 35%er - You've lost 35% of your starting weight! - User loses 35% of starting weight - 100

                    if (item.LossPercent == 35M)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._35_Percenter, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 35%er - You've lost 35% of your starting weight! - User loses 35% of starting weight - 100

                    #region 40%er - You've lost 40% of your starting weight! - User loses 40% of starting weight - 100

                    if (item.LossPercent == 40M)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._40_Percenter, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 40%er - You've lost 40% of your starting weight! - User loses 40% of starting weight - 100

                    #region 45%er - You've lost 45% of your starting weight! - User loses 45% of starting weight - 100

                    if (item.LossPercent == 45M)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._45_Percenter, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 45%er - You've lost 45% of your starting weight! - User loses 45% of starting weight - 100

                    #region Half off - You've lost 50% of your starting weight! - User loses 50% of starting weight - 200

                    if (item.LossPercent == 50M)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Half_off, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Half off - You've lost 50% of your starting weight! - User loses 50% of starting weight - 200

                    #region 5lbs to 150 lbs Weight Lose

                    #region 5lbs - You’ve lost 5lbs! - User loses 5lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 5)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._5lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 5lbs - You’ve lost 5lbs! - User loses 5lbs - 50

                    #region 10lbs - You’ve lost 10lbs! - User loses 10lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 10)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._10lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 10lbs - You’ve lost 10lbs! - User loses 10lbs - 50

                    #region 15lbs - You’ve lost 15lbs! - User loses 15lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 15)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._15lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 15lbs - You’ve lost 15lbs! - User loses 15lbs - 50

                    #region 20lbs - You’ve lost 20lbs! - User loses 20lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 20)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._20lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 20lbs - You’ve lost 20lbs! - User loses 20lbs - 50

                    #region 25lbs - You’ve lost 25lbs! - User loses 25lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 25)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._25lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 25lbs - You’ve lost 25lbs! - User loses 25lbs - 50

                    #region 30lbs - You’ve lost 30lbs! - User loses 30lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 30)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._30lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 30lbs - You’ve lost 30lbs! - User loses 30lbs - 50

                    #region 35lbs - You’ve lost 35lbs! - User loses 35lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 35)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._35lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 35lbs - You’ve lost 35lbs! - User loses 35lbs - 50

                    #region 40lbs - You’ve lost 40lbs! - User loses 40lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 40)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._40lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 40lbs - You’ve lost 40lbs! - User loses 40lbs - 50

                    #region 45lbs - You’ve lost 45lbs! - User loses 45lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 45)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._45lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 45lbs - You’ve lost 45lbs! - User loses 45lbs - 50

                    #region 50lbs - You’ve lost 50lbs! - User loses 50lbs - 100

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 50)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._50lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 50lbs - You’ve lost 50lbs! - User loses 50lbs - 100

                    #region 55lbs - You’ve lost 55lbs! - User loses 55lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 55)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._55lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 55lbs - You’ve lost 55lbs! - User loses 55lbs - 50

                    #region 60lbs - You’ve lost 60lbs! - User loses 60lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 60)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._60lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 60lbs - You’ve lost 60lbs! - User loses 60lbs - 50

                    #region 65lbs - You’ve lost 65lbs! - User loses 65lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 65)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._65lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 65lbs - You’ve lost 65lbs! - User loses 65lbs - 50

                    #region 70lbs - You’ve lost 70lbs! - User loses 70lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 70)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._70lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 70lbs - You’ve lost 70lbs! - User loses 70lbs - 50

                    #region 75lbs - You’ve lost 75lbs! - User loses 75lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 75)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._75lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 75lbs - You’ve lost 75lbs! - User loses 75lbs - 50

                    #region 80lbs - You’ve lost 80lbs! - User loses 80lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 80)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._80lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 80lbs - You’ve lost 80lbs! - User loses 80lbs - 50

                    #region 85lbs - You’ve lost 85lbs! - User loses 85lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 85)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._85lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 85lbs - You’ve lost 85lbs! - User loses 85lbs - 50

                    #region 90lbs - You’ve lost 90lbs! - User loses 90lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 90)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._90lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 90lbs - You’ve lost 90lbs! - User loses 90lbs - 50

                    #region 95lbs - You’ve lost 95lbs! - User loses 95lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 95)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._95lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 95lbs - You’ve lost 95lbs! - User loses 95lbs - 50

                    #region 100lbs - You’ve lost 100lbs! - User loses 100lbs - 200

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 100)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._100lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 100lbs - You’ve lost 100lbs! - User loses 100lbs - 200

                    #region 105lbs - You’ve lost 105lbs! - User loses 105lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 105)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._105lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 105lbs - You’ve lost 105lbs! - User loses 105lbs - 50

                    #region 110lbs - You’ve lost 110lbs! - User loses 110lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 110)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._110lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 110lbs - You’ve lost 110lbs! - User loses 110lbs - 50

                    #region 115lbs - You’ve lost 115lbs! - User loses 115lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 115)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._115lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 115lbs - You’ve lost 115lbs! - User loses 115lbs - 50

                    #region 120lbs - You’ve lost 120lbs! - User loses 120lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 120)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._120lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 120lbs - You’ve lost 120lbs! - User loses 120lbs - 50

                    #region 125lbs - You’ve lost 125lbs! - User loses 125lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 125)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._125lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 125lbs - You’ve lost 125lbs! - User loses 125lbs - 50

                    #region 130lbs - You’ve lost 130lbs! - User loses 130lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 130)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._130lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 130lbs - You’ve lost 130lbs! - User loses 130lbs - 50

                    #region 135lbs - You’ve lost 135lbs! - User loses 135lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 135)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._135lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 135lbs - You’ve lost 135lbs! - User loses 135lbs - 50

                    #region 140lbs - You’ve lost 140lbs! - User loses 140lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 140)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._140lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 140lbs - You’ve lost 140lbs! - User loses 140lbs - 50

                    #region 145lbs - You’ve lost 145lbs! - User loses 145lbs - 50

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 145)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._145lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 145lbs - You’ve lost 145lbs! - User loses 145lbs - 50

                    #region 150lbs - You’ve lost 150lbs! - User loses 150lbs - 200

                    if (ConvertWeight_kg_To_lbs(item.TotalLost) == 150)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._150lbs, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 150lbs - You’ve lost 150lbs! - User loses 150lbs - 200

                    #endregion 5lbs to 150 lbs Weight Lose

                    #region 5kg to 70kg Lose

                    #region 5kg - You've lost 5kg! - User loses 5kg - 50

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 5)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._5kg, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 5kg - You've lost 5kg! - User loses 5kg - 50

                    #region 10kg - You've lost 10kg! - User loses 10kg - 50

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 10)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._10kg, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 10kg - You've lost 10kg! - User loses 10kg - 50

                    #region 15kg - You've lost 15kg! - User loses 15kg - 50

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 15)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._15kg, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 15kg - You've lost 15kg! - User loses 15kg - 50

                    #region 20kg - You've lost 20kg! - User loses 20kg - 50

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 20)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._20kg, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 20kg - You've lost 20kg! - User loses 20kg - 50

                    #region 25kg - You've lost 25kg! - User loses 25kg - 50

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 25)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._25kg, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 25kg - You've lost 25kg! - User loses 25kg - 50

                    #region 30kg - You've lost 30kg! - User loses 30kg - 50

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 30)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._30kg, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 30kg - You've lost 30kg! - User loses 30kg - 50

                    #region 35kg - You've lost 35kg! - User loses 35kg - 50

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 35)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._35kg, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 35kg - You've lost 35kg! - User loses 35kg - 50

                    #region 40kg - You've lost 40kg! - User loses 40kg - 50

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 40)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._40kg, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 40kg - You've lost 40kg! - User loses 40kg - 50

                    #region 45kg - You've lost 45kg! - User loses 45kg - 50

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 45)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._45kg, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 45kg - You've lost 45kg! - User loses 45kg - 50

                    #region 50kg - You've lost 50kg! - User loses 50kg - 50

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 50)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._50kg, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 50kg - You've lost 50kg! - User loses 50kg - 50

                    #region 55kg - You've lost 55kg! - User loses 55kg - 50

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 55)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._55kg, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 55kg - You've lost 55kg! - User loses 55kg - 50

                    #region 60kg - You've lost 60kg! - User loses 60kg - 50

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 60)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._60kg, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 60kg - You've lost 60kg! - User loses 60kg - 50

                    #region 65kg - You've lost 65kg! - User loses 65kg - 50

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 65)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._65kg, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 65kg - You've lost 65kg! - User loses 65kg - 50

                    #region 70kg - You've lost 70kg! - User loses 70kg - 200

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 70)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._70kg, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion 70kg - You've lost 70kg! - User loses 70kg - 200

                    #endregion 5kg to 70kg Lose

                    #region Player records weight entry 3, 10 times

                    #region Scale watcher- You’ve been to 3 weigh ins!- Player records weight entry 3 times - 25

                    if (GetHowManyWeeksPlayerWeight(item) == 3)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Scale_watcher, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Scale watcher- You’ve been to 3 weigh ins!- Player records weight entry 3 times - 25

                    #region Scale fancier - You've been to 10 weigh ins! - Player records weight entry 10 times - 50

                    if (GetHowManyWeeksPlayerWeight(item) == 10)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Scale_fancier, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Scale fancier - You've been to 10 weigh ins! - Player records weight entry 10 times - 50

                    #endregion Player records weight entry 3, 10 times

                    #region Player Reaches 50% or 100% of their Target Weight (which they set in their Progress Dashboard Settting)

                    {
                        ProgressChartsRepository progressChartsRepo = new ProgressChartsRepository();

                        var PercentTargetCompleted = progressChartsRepo.ReadWeightPieChartData(item.PlayerID).value;

                        #region Livin’ on a prayer - You’re half way to your goal - keep going - User is half way to their goal - 50

                        if (PercentTargetCompleted == 50)
                        {
                            AddAchievementToList(item.PlayerID, enumAchievements._Livin_on_a_prayer, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                        }

                        #endregion Livin’ on a prayer - You’re half way to your goal - keep going - User is half way to their goal - 50

                        #region Goooooooooooooaal! - You’ve reached your goal - huge congratulations! - User reaches goal - 500

                        if (PercentTargetCompleted == 100)
                        {
                            AddAchievementToList(item.PlayerID, enumAchievements._Goooooooooooooaal, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                        }

                        #endregion Goooooooooooooaal! - You’ve reached your goal - huge congratulations! - User reaches goal - 500
                    }

                    #endregion Player Reaches 50% or 100% of their Target Weight (which they set in their Progress Dashboard Settting)

                    #region Healthy BMI - Your BMI is now classed as healthy! - User's BMI is no longer classed as overweight - 250

                    //Only Once,  Player Weight data Input (Leagues => Players Tab), system will check after every weigh in,  and compare the current BMI and Registration Time BMI, if Registration Time BMI was between 25 and 29.9 but current BMI after this weight in is less than 25 , then apply this achievement

                    if ((item.RegBMI > 25 && item.RegBMI < 29.9M) && item.CurrentBMI < 25)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Healthy_BMI, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Healthy BMI - Your BMI is now classed as healthy! - User's BMI is no longer classed as overweight - 250

                    #region Onederland - Your weight in pounds now begins with a 1 - User's weight in pounds now begins with a 1 - 100

                    //Only Once,  Player Weight data Input (Leagues => Players Tab), system will check after every weigh in, and convert the current week weight to lbs and if it is between 100lbs and 199lbs , then apply this achievement
                    {
                        decimal currentWeekWeight_Lbs = ConvertWeight_kg_To_lbs(GetCurrentWeekWeight(item));
                        if (currentWeekWeight_Lbs >= 100 && currentWeekWeight_Lbs <= 199)
                        {
                            AddAchievementToList(item.PlayerID, enumAchievements._Onederland, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                        }
                    }

                    #endregion Onederland - Your weight in pounds now begins with a 1 - User's weight in pounds now begins with a 1 - 100

                    #region Double Digits - Your weight in kilos is now in double digits - User's weight in kilos is now double digits - 100

                    //Only Once,  Player Weight data Input (Leagues => Players Tab), system will check after every weigh in, and if it is between 10kg and 99kg , then apply this achievement

                    {
                        decimal currentWeekWeight_kgs = GetCurrentWeekWeight(item);
                        if (currentWeekWeight_kgs >= 10 && currentWeekWeight_kgs <= 99)
                        {
                            AddAchievementToList(item.PlayerID, enumAchievements._Double_Digits, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                        }
                    }

                    #endregion Double Digits - Your weight in kilos is now in double digits - User's weight in kilos is now double digits - 100

                    #region Player Weight Lose in Stones

                    #region Stone Roses - You lost your first stone - User has lost one stone - 100

                    //Only Once,  Player Weight data Input (Leagues => Players Tab), system will check after every weigh in, and Player current total Weight Lose is 6.35kg , then apply this achievement

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 6.35M)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Stone_Roses, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Stone Roses - You lost your first stone - User has lost one stone - 100

                    #region Rolling Stones - You lost your second stone - User has lost two stone - 100

                    //Only Once,  Player Weight data Input (Leagues => Players Tab), system will check after every weigh in, and Player current total Weight Lose is 12.7kg , then apply this achievement

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 12.7M)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Rolling_Stones, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Rolling Stones - You lost your second stone - User has lost two stone - 100

                    #region Queens of the Stone Age - You lost five stone - User has lost five stone - 250

                    //Only Once,  Player Weight data Input (Leagues => Players Tab), system will check after every weigh in, and Player current total Weight Lose is 31.75kg , then apply this achievement

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 31.75M)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Queens_of_the_Stone_Age, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Queens of the Stone Age - You lost five stone - User has lost five stone - 250

                    #region Joss Stone - You lost ten stone - User has lost ten stone - 250

                    //Only Once,  Player Weight data Input (Leagues => Players Tab), system will check after every weigh in, and Player current total Weight Lose is 63.50kg , then apply this achievement

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 63.50M)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Joss_Stone, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Joss Stone - You lost ten stone - User has lost ten stone - 250

                    #region Stone Cold Crazy - You lost twenty stone - User has lost twenty stone - 500

                    //Only Once,  Player Weight data Input (Leagues => Players Tab), system will check after every weigh in, and Player current total Weight Lose is 127kg , then apply this achievement

                    if (item.TotalLost.HasValue && item.TotalLost.Value == 127M)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Stone_Cold_Crazy, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Stone Cold Crazy - You lost twenty stone - User has lost twenty stone - 500

                    #endregion Player Weight Lose in Stones

                    #region Kick off - You’ve played your first game of MAN v FAT Football! - FOOTBALL - User played first game - 10

                    //Only Once,  Player Weight data Input (Leagues => Players Tab), system will check after every weigh in, if player has Week 1 weigh in, then apply this achievement

                    if (item.Wk1.HasValue && item.Wk1.Value > 0)
                    {
                        AddAchievementToList(item.PlayerID, enumAchievements._Kick_off, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.OnlyOnce);
                    }

                    #endregion Kick off - You’ve played your first game of MAN v FAT Football! - FOOTBALL - User played first game - 10
                }

                if (ListOfPlayerAchievements.Count > 0)
                {
                    //TODO: Now do the Bulk Insert into PlayerAchievements
                    db.BulkInsert(ListOfPlayerAchievements);
                    db.SaveChanges();

                    //Now ADD to Notifications
                    AddAchievementToNotification(ListOfPlayerAchievements);
                }

                thread.Join(0);
            }
            catch (ThreadAbortException e)
            {
                SecurityUtils.AddAuditLog("WeightInWeekly_AchievementPoints", "The WeightInWeekly_AchievementPoints Thread Aborted Successfully");
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
                SecurityUtils.AddAuditLog("WeightInWeekly_AchievementPoints", "The WeightInWeekly_AchievementPoints Error");
            }
            finally
            {
                SecurityUtils.AddAuditLog("WeightInWeekly_AchievementPoints", "The WeightInWeekly_AchievementPoints Thread Completed Successfully");
            }
        }
        */
        #endregion Weight input Weekly

      

        

        #region Achievement Category FOOTBALL -  Fixtures_AchievementPoints

        public void PlayerScoreHatrick_AchievementPoints(long PlayerID, Thread thread)
        {
            try
            {
                List<PlayerAchievements> ListOfPlayerAchievements = new List<PlayerAchievements>();

                if (CheckIfPlayerHas_PremiumDashboard(PlayerID) == true)
                {
                    DateTime TodayDateTime = DateTime.Now;
                    DateTime TodayDate = DateTime.Now.Date;

                    PlayerAchievementsRepository paRepo = new PlayerAchievementsRepository();
                    var playerAchievements = paRepo.ReadAll(PlayerID);

                    #region Rabbit rabbit rabbit - You’ve scored a hatrick! - FOOTBALL - User scores a hatrick - 50

                    //Every Week Fixture Input, if Current Player Lose for 3, 6, 9 or 12 weeks, then apply this achievement

                    AddAchievementToList(PlayerID, enumAchievements._Rabbit_rabbit_rabbit, TodayDateTime, ref ListOfPlayerAchievements, ref playerAchievements, enumAchievementTriggetType.EverySingleSeason);

                    #endregion Rabbit rabbit rabbit - You’ve scored a hatrick! - FOOTBALL - User scores a hatrick - 50
                }
                if (ListOfPlayerAchievements.Count > 0)
                {
                    //TODO: Now do the Bulk Insert into PlayerAchievements
                    db.BulkInsert(ListOfPlayerAchievements);
                    db.SaveChanges();

                    //Now ADD to Notifications
                    AddAchievementToNotification(ListOfPlayerAchievements);
                }

                thread.Join(0);
            }
            catch (ThreadAbortException e)
            {
                SecurityUtils.AddAuditLog("PlayerScoreHatrick_AchievementPoints", "The PlayerScoreHatrick_AchievementPoints Thread Aborted Successfully. PlayerID = " + PlayerID);
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
                SecurityUtils.AddAuditLog("PlayerScoreHatrick_AchievementPoints", "The PlayerScoreHatrick_AchievementPoints Error. PlayerID = " + PlayerID);
            }
            finally
            {
                SecurityUtils.AddAuditLog("PlayerScoreHatrick_AchievementPoints", "The PlayerScoreHatrick_AchievementPoints Thread Completed Successfully. PlayerID = " + PlayerID);
            }
        }

        #endregion Achievement Category FOOTBALL -  Fixtures_AchievementPoints

        #region Helper Functions

        public void AddAchievementToList(long PlayerID, enumAchievements enumAchievement, DateTime TodayDateTime, ref List<PlayerAchievements> ListOfPlayerAchievements, ref List<PlayerAchievements> playerAchievements, enumAchievementTriggetType enumAchievementTriggetType)
        {
            PlayerAchievements model = new PlayerAchievements()
            {
                PlayerID = PlayerID,
                AchievementID = (long)enumAchievement,
                AchievementDateTime = TodayDateTime
            };

            //Some of the Achievements only applied Once and should never Repeate - Marked as Only Once
            if (enumAchievementTriggetType == enumAchievementTriggetType.OnlyOnce)
            {
                if (!playerAchievements.Any(m => m.AchievementID == model.AchievementID))//Check if this Achievement is not already added to PlayerAchievements
                {
                    ListOfPlayerAchievements.Add(model);
                }
            }
            else if (enumAchievementTriggetType == enumAchievementTriggetType.EverySingleDay) //Achievement Marked as Every Single Day, but not in the same day/Date
            {
                //if achievement if for "Granted every day that a user logs their food" then only apply the achievement For current Day/Date
                //only apply this achievement if Player didn't have this Achievement for Today Date

                if (!playerAchievements.Any(m => m.AchievementID == model.AchievementID && MatchForTodayDate(m.AchievementDateTime)))
                //Check if this Achievement is not already added to PlayerAchievements
                {
                    ListOfPlayerAchievements.Add(model);
                }
            }
            else if (enumAchievementTriggetType == enumAchievementTriggetType.EverySingleSeason)
            //Achievement Trigger When a League Reset, and Marked as Every Single Season, but not in the same season
            {
                ListOfPlayerAchievements.Add(model);
            }
        }

        public void AddAchievementToNotification(List<PlayerAchievements> ListOfPlayerAchievements)
        {
            foreach (var item in ListOfPlayerAchievements)
            {
                var achievement = db.Achievements.FirstOrDefault(m => m.AchievementID == item.AchievementID);

                DashboardNotifications model = new DashboardNotifications()
                {
                    Title = achievement.Title,
                    Summary = achievement.Description,
                    LeagueID = null,
                    IsRecurring = false,
                    RecurringFrequency = null,
                    DayOfWeek = null,
                    IsAchievementNotification = true,
                    NotificationDateTime = item.AchievementDateTime
                };

                db.DashboardNotifications.Add(model);
                db.SaveChanges();

                db.InsertDashboardNotificationforPlayers(model.DashboardNotificationID, item.PlayerID.ToString());
            }
        }

        public int MonthDifference(DateTime lValue, DateTime rValue)
        {
            return Math.Abs((lValue.Month - rValue.Month) + 12 * (lValue.Year - rValue.Year));
        }

        public bool MatchForTodayDate(DateTime DateToMatch)
        {
            return DateToMatch.Date == DateTime.Now.Date;
        }

        public bool IsStringNullOrEmpty(string value)
        {
            return string.IsNullOrEmpty(value);
        }

        public bool CompareStringIfNotEmpty(string dbField, string value)
        {
            bool val = false;

            if (string.IsNullOrEmpty(dbField))
            {
                val = false;
            }
            else if (dbField.ToLower().Contains(value.ToLower()))
            {
                val = true;
            }

            return val;
        }
        /*
        //Only Once,  Player Weight data Input (Leagues => Players Tab), system will check after every weigh in, if Current and previous 2 week's weight Lose is +ive value (i.e. he lose weight) then apply this achievement
        public bool CheckIfPlayerCurrentAndLast2WeekLose(PlayerWeightWeeksExt pww)
        {
            bool status = false;

            if ((pww.LossWk14.HasValue && pww.LossWk14.Value > 0) && (pww.LossWk13.HasValue && pww.LossWk13.Value > 0) && (pww.LossWk12.HasValue && pww.LossWk12.Value > 0))
            {
                return true;
            }
            else if ((pww.LossWk13.HasValue && pww.LossWk13.Value > 0) && (pww.LossWk12.HasValue && pww.LossWk12.Value > 0) && (pww.LossWk11.HasValue && pww.LossWk11.Value > 0))
            {
                return true;
            }
            else if ((pww.LossWk12.HasValue && pww.LossWk12.Value > 0) && (pww.LossWk11.HasValue && pww.LossWk11.Value > 0) && (pww.LossWk10.HasValue && pww.LossWk10.Value > 0))
            {
                return true;
            }
            else if ((pww.LossWk11.HasValue && pww.LossWk11.Value > 0) && (pww.LossWk10.HasValue && pww.LossWk10.Value > 0) && (pww.LossWk9.HasValue && pww.LossWk9.Value > 0))
            {
                return true;
            }
            else if ((pww.LossWk10.HasValue && pww.LossWk10.Value > 0) && (pww.LossWk9.HasValue && pww.LossWk9.Value > 0) && (pww.LossWk8.HasValue && pww.LossWk8.Value > 0))
            {
                return true;
            }
            else if ((pww.LossWk9.HasValue && pww.LossWk9.Value > 0) && (pww.LossWk8.HasValue && pww.LossWk8.Value > 0) && (pww.LossWk7.HasValue && pww.LossWk7.Value > 0))
            {
                return true;
            }
            else if ((pww.LossWk8.HasValue && pww.LossWk8.Value > 0) && (pww.LossWk7.HasValue && pww.LossWk7.Value > 0) && (pww.LossWk6.HasValue && pww.LossWk6.Value > 0))
            {
                return true;
            }
            else if ((pww.LossWk7.HasValue && pww.LossWk7.Value > 0) && (pww.LossWk6.HasValue && pww.LossWk6.Value > 0) && (pww.LossWk5.HasValue && pww.LossWk5.Value > 0))
            {
                return true;
            }
            else if ((pww.LossWk6.HasValue && pww.LossWk6.Value > 0) && (pww.LossWk5.HasValue && pww.LossWk5.Value > 0) && (pww.LossWk4.HasValue && pww.LossWk4.Value > 0))
            {
                return true;
            }
            else if ((pww.LossWk5.HasValue && pww.LossWk5.Value > 0) && (pww.LossWk4.HasValue && pww.LossWk4.Value > 0) && (pww.LossWk3.HasValue && pww.LossWk3.Value > 0))
            {
                return true;
            }
            else if ((pww.LossWk4.HasValue && pww.LossWk4.Value > 0) && (pww.LossWk3.HasValue && pww.LossWk3.Value > 0) && (pww.LossWk2.HasValue && pww.LossWk2.Value > 0))
            {
                return true;
            }
            else if ((pww.LossWk3.HasValue && pww.LossWk3.Value > 0) && (pww.LossWk2.HasValue && pww.LossWk2.Value > 0) && (pww.LossWk1.HasValue && pww.LossWk1.Value > 0))
            {
                return true;
            }

            return status;
        }

        //Player Must Lose weight Evey week. i.e. his LossWkx must be greater then last week
        public bool CheckIfPlayerLoseWeightInLast3Week(PlayerWeightWeeksExt pww)
        {
            bool status = false;

            if ((pww.LossWk14.HasValue && pww.LossWk14.Value > 0)
                && (pww.LossWk13.HasValue && (pww.LossWk13.Value > 0 && pww.LossWk13.Value > pww.LossWk14.Value))
                && (pww.LossWk12.HasValue && (pww.LossWk12.Value > 0 && pww.LossWk12.Value > pww.LossWk13.Value)))
            {
                return true;
            }
            else if ((pww.LossWk13.HasValue && pww.LossWk13.Value > 0)
                && (pww.LossWk12.HasValue && (pww.LossWk12.Value > 0 && pww.LossWk12.Value > pww.LossWk13.Value))
                && (pww.LossWk11.HasValue && (pww.LossWk11.Value > 0 && pww.LossWk11.Value > pww.LossWk12.Value)))
            {
                return true;
            }
            else if ((pww.LossWk12.HasValue && pww.LossWk12.Value > 0)
                && (pww.LossWk11.HasValue && (pww.LossWk11.Value > 0 && pww.LossWk11.Value > pww.LossWk12.Value))
                && (pww.LossWk10.HasValue && (pww.LossWk10.Value > 0 && pww.LossWk10.Value > pww.LossWk11.Value)))
            {
                return true;
            }
            else if ((pww.LossWk11.HasValue && pww.LossWk11.Value > 0)
                && (pww.LossWk10.HasValue && (pww.LossWk10.Value > 0 && pww.LossWk10.Value > pww.LossWk11.Value))
                && (pww.LossWk9.HasValue && (pww.LossWk9.Value > 0 && pww.LossWk9.Value > pww.LossWk10.Value)))
            {
                return true;
            }
            else if ((pww.LossWk10.HasValue && pww.LossWk10.Value > 0)
                && (pww.LossWk9.HasValue && (pww.LossWk9.Value > 0 && pww.LossWk9.Value > pww.LossWk10.Value))
                && (pww.LossWk8.HasValue && (pww.LossWk8.Value > 0 && pww.LossWk8.Value > pww.LossWk9.Value)))
            {
                return true;
            }
            else if ((pww.LossWk9.HasValue && pww.LossWk9.Value > 0)
                && (pww.LossWk8.HasValue && (pww.LossWk8.Value > 0 && pww.LossWk8.Value > pww.LossWk9.Value))
                && (pww.LossWk7.HasValue && (pww.LossWk7.Value > 0 && pww.LossWk7.Value > pww.LossWk8.Value)))
            {
                return true;
            }
            else if ((pww.LossWk8.HasValue && pww.LossWk8.Value > 0)
                && (pww.LossWk7.HasValue && (pww.LossWk7.Value > 0 && pww.LossWk7.Value > pww.LossWk8.Value))
                && (pww.LossWk6.HasValue && (pww.LossWk6.Value > 0 && pww.LossWk6.Value > pww.LossWk7.Value)))
            {
                return true;
            }
            else if ((pww.LossWk7.HasValue && pww.LossWk7.Value > 0)
                && (pww.LossWk6.HasValue && (pww.LossWk6.Value > 0 && pww.LossWk6.Value > pww.LossWk7.Value))
                && (pww.LossWk5.HasValue && (pww.LossWk5.Value > 0 && pww.LossWk5.Value > pww.LossWk6.Value)))
            {
                return true;
            }
            else if ((pww.LossWk6.HasValue && pww.LossWk6.Value > 0)
                && (pww.LossWk5.HasValue && (pww.LossWk5.Value > 0 && pww.LossWk5.Value > pww.LossWk6.Value))
                && (pww.LossWk4.HasValue && (pww.LossWk4.Value > 0 && pww.LossWk4.Value > pww.LossWk5.Value)))
            {
                return true;
            }
            else if ((pww.LossWk5.HasValue && pww.LossWk5.Value > 0)
                && (pww.LossWk4.HasValue && (pww.LossWk4.Value > 0 && pww.LossWk4.Value > pww.LossWk5.Value))
                && (pww.LossWk3.HasValue && (pww.LossWk3.Value > 0 && pww.LossWk3.Value > pww.LossWk4.Value)))
            {
                return true;
            }
            else if ((pww.LossWk4.HasValue && pww.LossWk4.Value > 0)
                && (pww.LossWk3.HasValue && (pww.LossWk3.Value > 0 && pww.LossWk3.Value > pww.LossWk4.Value))
                && (pww.LossWk2.HasValue && (pww.LossWk2.Value > 0 && pww.LossWk2.Value > pww.LossWk1.Value)))
            {
                return true;
            }
            else if ((pww.LossWk3.HasValue && pww.LossWk3.Value > 0)
                && (pww.LossWk2.HasValue && (pww.LossWk2.Value > 0 && pww.LossWk2.Value > pww.LossWk1.Value))
                && (pww.LossWk1.HasValue && pww.LossWk1.Value > 0))
            {
                return true;
            }

            return status;
        }

        //Every Week Weight Input, if Current Week lose is in -ive value (i.e. he gain weight) then apply this achivement

        public bool CheckIfPlayerGainWait(PlayerWeightWeeksExt pww)
        {
            bool status = false;

            if (pww.LossWk14.HasValue)
            {
                if (pww.LossWk14 < 0) { return true; }
            }
            else if (pww.LossWk13.HasValue)
            {
                if (pww.LossWk13 < 0) { return true; }
            }
            else if (pww.LossWk12.HasValue)
            {
                if (pww.LossWk12 < 0) { return true; }
            }
            else if (pww.LossWk11.HasValue)
            {
                if (pww.LossWk11 < 0) { return true; }
            }
            else if (pww.LossWk10.HasValue)
            {
                if (pww.LossWk10 < 0) { return true; }
            }
            else if (pww.LossWk9.HasValue)
            {
                if (pww.LossWk9 < 0) { return true; }
            }
            else if (pww.LossWk8.HasValue)
            {
                if (pww.LossWk8 < 0) { return true; }
            }
            else if (pww.LossWk7.HasValue)
            {
                if (pww.LossWk7 < 0) { return true; }
            }
            else if (pww.LossWk6.HasValue)
            {
                if (pww.LossWk6 < 0) { return true; }
            }
            else if (pww.LossWk5.HasValue)
            {
                if (pww.LossWk5 < 0) { return true; }
            }
            else if (pww.LossWk4.HasValue)
            {
                if (pww.LossWk4 < 0) { return true; }
            }
            else if (pww.LossWk3.HasValue)
            {
                if (pww.LossWk3 < 0) { return true; }
            }
            else if (pww.LossWk2.HasValue)
            {
                if (pww.LossWk2 < 0) { return true; }
            }
            else if (pww.LossWk1.HasValue)
            {
                if (pww.LossWk1 < 0) { return true; }
            }

            return status;
        }

        public bool CheckIfCurrentAndPreviousWeekWeightSame(PlayerWeightWeeksExt pww)
        {
            bool status = false;

            if ((pww.Wk14.HasValue && pww.Wk13.HasValue) && (pww.Wk14.Value == pww.Wk13.Value))
            {
                status = true;
            }
            else if ((pww.Wk13.HasValue && pww.Wk12.HasValue) && (pww.Wk13.Value == pww.Wk12.Value))
            {
                status = true;
            }
            else if ((pww.Wk12.HasValue && pww.Wk11.HasValue) && (pww.Wk12.Value == pww.Wk11.Value))
            {
                status = true;
            }
            else if ((pww.Wk11.HasValue && pww.Wk10.HasValue) && (pww.Wk11.Value == pww.Wk10.Value))
            {
                status = true;
            }
            else if ((pww.Wk10.HasValue && pww.Wk9.HasValue) && (pww.Wk10.Value == pww.Wk9.Value))
            {
                status = true;
            }
            else if ((pww.Wk9.HasValue && pww.Wk8.HasValue) && (pww.Wk9.Value == pww.Wk8.Value))
            {
                status = true;
            }
            else if ((pww.Wk8.HasValue && pww.Wk7.HasValue) && (pww.Wk8.Value == pww.Wk7.Value))
            {
                status = true;
            }
            else if ((pww.Wk7.HasValue && pww.Wk6.HasValue) && (pww.Wk7.Value == pww.Wk6.Value))
            {
                status = true;
            }
            else if ((pww.Wk6.HasValue && pww.Wk5.HasValue) && (pww.Wk6.Value == pww.Wk5.Value))
            {
                status = true;
            }
            else if ((pww.Wk5.HasValue && pww.Wk4.HasValue) && (pww.Wk5.Value == pww.Wk4.Value))
            {
                status = true;
            }
            else if ((pww.Wk4.HasValue && pww.Wk3.HasValue) && (pww.Wk4.Value == pww.Wk3.Value))
            {
                status = true;
            }
            else if ((pww.Wk3.HasValue && pww.Wk2.HasValue) && (pww.Wk3.Value == pww.Wk2.Value))
            {
                status = true;
            }
            else if ((pww.Wk2.HasValue && pww.Wk1.HasValue) && (pww.Wk2.Value == pww.Wk1.Value))
            {
                status = true;
            }

            return status;
        }

        //Only Once, THIS WOULD BE OVER A MONTH - SO FOUR WEIGH INS THAT ARE CONSECUTIVE - I.E. WEEK 1, 2, 3 AND 4 WHICH ALL SHOW LOSSES.
        public bool CheckIfPlayerLoseWeightForFirst4Weeks(PlayerWeightWeeksExt pww)
        {
            //if LossWkx is greater then zero then this is Lose in Weight i.e. +ive value is lose in weight
            bool status = false;

            if ((pww.LossWk1.HasValue && pww.LossWk1.Value > 0)
                && (pww.LossWk2.HasValue && pww.LossWk2.Value > 0)
                && (pww.LossWk3.HasValue && pww.LossWk3.Value > 0)
                && (pww.LossWk4.HasValue && pww.LossWk4.Value > 0))
            {
                status = true;
            }

            return status;
        }

        //Every Single Season, Trigger When a League Reset , System will check if any week weight data is missing then apply this achievement
        public bool CheckIfAnyWeekWeighInIsMissing(PlayerWeightWeeksExt pww)
        {
            //if LossWkx is greater then zero then this is Lose in Weight i.e. +ive value is lose in weight
            bool status = false;

            if (pww.Wk1.HasValue == false) { return false; }
            else if (pww.Wk2.HasValue == false) { return false; }
            else if (pww.Wk3.HasValue == false) { return false; }
            else if (pww.Wk4.HasValue == false) { return false; }
            else if (pww.Wk5.HasValue == false) { return false; }
            else if (pww.Wk6.HasValue == false) { return false; }
            else if (pww.Wk7.HasValue == false) { return false; }
            else if (pww.Wk8.HasValue == false) { return false; }
            else if (pww.Wk9.HasValue == false) { return false; }
            else if (pww.Wk10.HasValue == false) { return false; }
            else if (pww.Wk11.HasValue == false) { return false; }
            else if (pww.Wk12.HasValue == false) { return false; }
            else if (pww.Wk13.HasValue == false) { return false; }
            else if (pww.Wk14.HasValue == false) { return false; }

            return status;
        }

        //Only Once, Trigger When a League Reset , System will check if currently player didn't miss any weigh in for this season, only then apply this achievement
        public bool CheckIfPlayerDidNotMissAnyWeekWeighIn(PlayerWeightWeeksExt pww)
        {
            //if LossWkx is greater then zero then this is Lose in Weight i.e. +ive value is lose in weight
            bool status = false;

            if (pww.Wk1.HasValue
                && pww.Wk2.HasValue
                && pww.Wk3.HasValue
                && pww.Wk4.HasValue
                && pww.Wk5.HasValue
                && pww.Wk6.HasValue
                && pww.Wk7.HasValue
                && pww.Wk8.HasValue
                && pww.Wk9.HasValue
                && pww.Wk10.HasValue
                && pww.Wk11.HasValue
                && pww.Wk12.HasValue
                && pww.Wk13.HasValue
                && pww.Wk14.HasValue)
            {
                status = true;
            }

            return status;
        }

        //Every Week Weight Input, System will check if Current Week lose is in +ive value but Previous week lose was -ive value, only then apply this achivement

        public bool CheckIfCurrentLose_PreviousWeekGain(PlayerWeightWeeksExt pww)
        {
            //Current Week +ive and Previouse week -ive
            bool status = false;

            if ((pww.LossWk14.HasValue && pww.LossWk13.HasValue) && (pww.LossWk14.Value > 0 && pww.LossWk13.Value < 0))
            {
                status = true;
            }
            else if ((pww.LossWk13.HasValue && pww.LossWk12.HasValue) && (pww.LossWk13.Value > 0 && pww.LossWk12.Value < 0))
            {
                status = true;
            }
            else if ((pww.LossWk12.HasValue && pww.LossWk11.HasValue) && (pww.LossWk12.Value > 0 && pww.LossWk11.Value < 0))
            {
                status = true;
            }
            else if ((pww.LossWk11.HasValue && pww.LossWk10.HasValue) && (pww.LossWk11.Value > 0 && pww.LossWk10.Value < 0))
            {
                status = true;
            }
            else if ((pww.LossWk10.HasValue && pww.LossWk9.HasValue) && (pww.LossWk10.Value > 0 && pww.LossWk9.Value < 0))
            {
                status = true;
            }
            else if ((pww.LossWk9.HasValue && pww.LossWk8.HasValue) && (pww.LossWk9.Value > 0 && pww.LossWk8.Value < 0))
            {
                status = true;
            }
            else if ((pww.LossWk8.HasValue && pww.LossWk7.HasValue) && (pww.LossWk8.Value > 0 && pww.LossWk7.Value < 0))
            {
                status = true;
            }
            else if ((pww.LossWk7.HasValue && pww.LossWk6.HasValue) && (pww.LossWk7.Value > 0 && pww.LossWk6.Value < 0))
            {
                status = true;
            }
            else if ((pww.LossWk6.HasValue && pww.LossWk5.HasValue) && (pww.LossWk6.Value > 0 && pww.LossWk5.Value < 0))
            {
                status = true;
            }
            else if ((pww.LossWk5.HasValue && pww.LossWk4.HasValue) && (pww.LossWk5.Value > 0 && pww.LossWk4.Value < 0))
            {
                status = true;
            }
            else if ((pww.LossWk4.HasValue && pww.LossWk3.HasValue) && (pww.LossWk4.Value > 0 && pww.LossWk3.Value < 0))
            {
                status = true;
            }
            else if ((pww.LossWk3.HasValue && pww.LossWk2.HasValue) && (pww.LossWk3.Value > 0 && pww.LossWk2.Value < 0))
            {
                status = true;
            }
            else if ((pww.LossWk2.HasValue && pww.LossWk1.HasValue) && (pww.LossWk2.Value > 0 && pww.LossWk1.Value < 0))
            {
                status = true;
            }

            return status;
        }

        //Only Once,  Player Weight data Input (Leagues => Players Tab), system will check after every weigh in, and count if Player weigh in 3 times upto current Week, it could be previous week?, then apply this achievement
        //Check How many Weeks Player Weight In 3, 10, 20? (how it could be 20? when we have only 14 weeks of Weight Data Entry in a season)

        public int GetHowManyWeeksPlayerWeight(PlayerWeightWeeksExt pww)
        {
            int TotalWeighInWeeks = 0;

            if (pww.Wk14.HasValue)
            {
                TotalWeighInWeeks++;
            }
            if (pww.Wk13.HasValue)
            {
                TotalWeighInWeeks++;
            }
            if (pww.Wk12.HasValue)
            {
                TotalWeighInWeeks++;
            }
            if (pww.Wk11.HasValue)
            {
                TotalWeighInWeeks++;
            }
            if (pww.Wk10.HasValue)
            {
                TotalWeighInWeeks++;
            }
            if (pww.Wk9.HasValue)
            {
                TotalWeighInWeeks++;
            }
            if (pww.Wk8.HasValue)
            {
                TotalWeighInWeeks++;
            }
            if (pww.Wk7.HasValue)
            {
                TotalWeighInWeeks++;
            }
            if (pww.Wk6.HasValue)
            {
                TotalWeighInWeeks++;
            }
            if (pww.Wk5.HasValue)
            {
                TotalWeighInWeeks++;
            }
            if (pww.Wk4.HasValue)
            {
                TotalWeighInWeeks++;
            }
            if (pww.Wk3.HasValue)
            {
                TotalWeighInWeeks++;
            }
            if (pww.Wk2.HasValue)
            {
                TotalWeighInWeeks++;
            }
            if (pww.Wk1.HasValue)
            {
                TotalWeighInWeeks++;
            }

            return TotalWeighInWeeks;
        }

        public int GetHowManyWeeksPlayer_LoseWeight(PlayerWeightWeeksExt pww)
        {
            int TotalWeeksPlayerLossWeight = 0;

            if (pww.LossWk14.HasValue && pww.LossWk14.Value > 0)
            {
                TotalWeeksPlayerLossWeight++;
            }
            if (pww.LossWk13.HasValue && pww.LossWk13.Value > 0)
            {
                TotalWeeksPlayerLossWeight++;
            }
            if (pww.LossWk12.HasValue && pww.LossWk12.Value > 0)
            {
                TotalWeeksPlayerLossWeight++;
            }
            if (pww.LossWk11.HasValue && pww.LossWk11.Value > 0)
            {
                TotalWeeksPlayerLossWeight++;
            }
            if (pww.LossWk10.HasValue && pww.LossWk10.Value > 0)
            {
                TotalWeeksPlayerLossWeight++;
            }
            if (pww.LossWk9.HasValue && pww.LossWk9.Value > 0)
            {
                TotalWeeksPlayerLossWeight++;
            }
            if (pww.LossWk8.HasValue && pww.LossWk8.Value > 0)
            {
                TotalWeeksPlayerLossWeight++;
            }
            if (pww.LossWk7.HasValue && pww.LossWk7.Value > 0)
            {
                TotalWeeksPlayerLossWeight++;
            }
            if (pww.LossWk6.HasValue && pww.LossWk6.Value > 0)
            {
                TotalWeeksPlayerLossWeight++;
            }
            if (pww.LossWk5.HasValue && pww.LossWk5.Value > 0)
            {
                TotalWeeksPlayerLossWeight++;
            }
            if (pww.LossWk4.HasValue && pww.LossWk4.Value > 0)
            {
                TotalWeeksPlayerLossWeight++;
            }
            if (pww.LossWk3.HasValue && pww.LossWk3.Value > 0)
            {
                TotalWeeksPlayerLossWeight++;
            }
            if (pww.LossWk2.HasValue && pww.LossWk2.Value > 0)
            {
                TotalWeeksPlayerLossWeight++;
            }
            if (pww.LossWk1.HasValue && pww.LossWk1.Value > 0)
            {
                TotalWeeksPlayerLossWeight++;
            }

            return TotalWeeksPlayerLossWeight;
        }

        public decimal GetCurrentWeekWeight(PlayerWeightWeeksExt pww)
        {
            decimal CurrentWeekWeight = 0.00M;

            if (pww.Wk14.HasValue)
            {
                CurrentWeekWeight = pww.Wk14.Value;
            }
            else if (pww.Wk13.HasValue)
            {
                CurrentWeekWeight = pww.Wk13.Value;
            }
            else if (pww.Wk12.HasValue)
            {
                CurrentWeekWeight = pww.Wk12.Value;
            }
            else if (pww.Wk11.HasValue)
            {
                CurrentWeekWeight = pww.Wk11.Value;
            }
            else if (pww.Wk10.HasValue)
            {
                CurrentWeekWeight = pww.Wk10.Value;
            }
            else if (pww.Wk9.HasValue)
            {
                CurrentWeekWeight = pww.Wk9.Value;
            }
            else if (pww.Wk8.HasValue)
            {
                CurrentWeekWeight = pww.Wk8.Value;
            }
            else if (pww.Wk7.HasValue)
            {
                CurrentWeekWeight = pww.Wk7.Value;
            }
            else if (pww.Wk6.HasValue)
            {
                CurrentWeekWeight = pww.Wk6.Value;
            }
            else if (pww.Wk5.HasValue)
            {
                CurrentWeekWeight = pww.Wk5.Value;
            }
            else if (pww.Wk4.HasValue)
            {
                CurrentWeekWeight = pww.Wk4.Value;
            }
            else if (pww.Wk3.HasValue)
            {
                CurrentWeekWeight = pww.Wk3.Value;
            }
            else if (pww.Wk2.HasValue)
            {
                CurrentWeekWeight = pww.Wk2.Value;
            }
            else if (pww.Wk1.HasValue)
            {
                CurrentWeekWeight = pww.Wk1.Value;
            }

            return CurrentWeekWeight;
        }

        public decimal GetCurrentWeekLoss(PlayerWeightWeeksExt pww)
        {
            decimal CurrentWeekLoss = 0.00M;

            if (pww.LossWk14.HasValue)
            {
                CurrentWeekLoss = pww.LossWk14.Value;
            }
            else if (pww.LossWk13.HasValue)
            {
                CurrentWeekLoss = pww.LossWk13.Value;
            }
            else if (pww.LossWk12.HasValue)
            {
                CurrentWeekLoss = pww.LossWk12.Value;
            }
            else if (pww.LossWk11.HasValue)
            {
                CurrentWeekLoss = pww.LossWk11.Value;
            }
            else if (pww.LossWk10.HasValue)
            {
                CurrentWeekLoss = pww.LossWk10.Value;
            }
            else if (pww.LossWk9.HasValue)
            {
                CurrentWeekLoss = pww.LossWk9.Value;
            }
            else if (pww.LossWk8.HasValue)
            {
                CurrentWeekLoss = pww.LossWk8.Value;
            }
            else if (pww.LossWk7.HasValue)
            {
                CurrentWeekLoss = pww.LossWk7.Value;
            }
            else if (pww.LossWk6.HasValue)
            {
                CurrentWeekLoss = pww.LossWk6.Value;
            }
            else if (pww.LossWk5.HasValue)
            {
                CurrentWeekLoss = pww.LossWk5.Value;
            }
            else if (pww.LossWk4.HasValue)
            {
                CurrentWeekLoss = pww.LossWk4.Value;
            }
            else if (pww.LossWk3.HasValue)
            {
                CurrentWeekLoss = pww.LossWk3.Value;
            }
            else if (pww.LossWk2.HasValue)
            {
                CurrentWeekLoss = pww.LossWk2.Value;
            }
            else if (pww.LossWk1.HasValue)
            {
                CurrentWeekLoss = pww.LossWk1.Value;
            }

            return CurrentWeekLoss;
        }
        */

        #region Weekly Activity Helper Functions

        public bool CheckIfPlayerLoggedActivityTwiceADay(List<IGrouping<DateTime, PlayerWeeklyActivityExt>> groups)
        {
            bool status = false;
            Dictionary<DateTime, int> ActivitiesPerDay = new Dictionary<DateTime, int>();

            foreach (var group in groups)
            {
                var datetime = group.Key;
                int TotalActivities = 0;
                foreach (var item in group)
                {
                    TotalActivities++;
                }

                ActivitiesPerDay.Add(datetime, TotalActivities);
            }

            if (ActivitiesPerDay.Any(m => m.Value >= 2))
            {
                status = true;
            }

            return status;
        }

        #endregion Weekly Activity Helper Functions

        public bool CheckIfPlayerHas_PremiumDashboard(long PlayerID)
        {
            bool status = true;
            //Check if Player has Premium Dashboard?
            PlayerDashboardRepository playerDashboardRepo = new PlayerDashboardRepository();
            var dashboard = playerDashboardRepo.ReadOne(PlayerID);

            //if Player didn't have PRemium Dashboard then move to next Player Record and ignore rest
            if (dashboard == null)
            {
                status = false;
            }

            return status;
        }

        public decimal ConvertWeight_kg_To_lbs(decimal? WeightKg)
        {
            if (WeightKg.HasValue)
            {
                decimal lbs_factor = 0.45359237M;

                decimal weight_In_lbs = WeightKg.Value / lbs_factor;

                weight_In_lbs = Math.Round(weight_In_lbs, 0);

                return weight_In_lbs;
            }
            else
            {
                return 0M;
            }
        }

        #endregion Helper Functions
    }

    public class PlayerLostWeightInTeam
    {
        public long PlayerID { get; set; }
        public string TeamName { get; set; }
        public int TotalWeekWeightLost { get; set; }
    }
}