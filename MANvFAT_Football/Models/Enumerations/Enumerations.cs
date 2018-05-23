namespace MANvFAT_Football.Models.Enumerations
{
    public enum Permissions
    {
        /// <summary>
        /// AllUsers All type of users may have access
        /// </summary>
        None = -1,

        AllUsers = 0,
        Administrator = 1,
        Coaches = 2,
        LeagueViewer = 3
    }

    public enum LoginTypes
    {
        FormLogin = 0,
        WindowsLogin = 1
    }

    public enum AuditAction
    {
        Create = 1,
        Update = 2,
        Delete = 3
    }

    public enum LeagueFixtureType
    {
        Weight_Goals = 1,
        Pitch = 2,
        Scale = 3
    }

    public enum Payment_Mode
    {
        Sandbox = 0,
        Live = 1
    }

    public enum Payment_Statuses
    {
        pending_customer_approval = 1,
        pending_submission = 2,
        submitted = 3,
        confirmed = 4,
        paid_out = 5,
        cancelled = 6,
        customer_approval_denied = 7,
        failed = 8,
        charged_back
    }

    public enum CookieNames
    {
        manvfat101,
        manvfatPaypal,
        manvfatGoCardless
    }

    public enum AlertTypes
    {
        Player_Added_To_Team = 1,
        Player_Moved_To_Ghost_Team = 2,
        Red_Flag_Triggered = 3,
        Player_Moved_To_Active_Team = 4,
    }

    public enum TypeOfMessage
    {
        Information = 1,
        Success = 2,
        Warning = 3,
        Error = 4
    }

    public enum enumPaymentTypes
    {
        Registration = 1,
        Reg_Deposit = 2,
        Subscription = 3,
        Premium_Dashboard = 4,
        NA = 5
    }

    public enum UniqueGUIDTable
    {
        PlayerPayments = 1,
        Documents = 2,
        ProgressDashboard = 3
    }

    public enum enumDailyActivityTypes
    {
        Breakfast,
        Lunch,
        Dinner,
        Snacks,
        Drink
    }

    public enum enumShareWith
    {
        Coach=1,
        Team=2,
        All=3
    }

    public enum enumShareFrequency
    {
        Daily = 1,
        Weekly = 2,
        Monthly = 3
    }

    public enum enumAchievementTriggetType
    {
        OnlyOnce,
        EverySingleDay,
        EverySingleSeason
    }



    public enum enumAchievements
    {
        /// <summary> ActionToEarn = User fills in the food and drink diary for the first time. </summary>
        _First_Bite = 1,

        /// <summary> ActionToEarn = User has filled in 10 days of food and drink diary </summary>
        _Hanging_Ten = 2,

        /// <summary> ActionToEarn = Granted every day that a user logs their food </summary>
        _Minimum_Effective_Dosage = 3,

        /// <summary> ActionToEarn = The word cake appears in the food diary </summary>
        _The_cake_probably_wasnt_a_lie = 4,

        /// <summary> ActionToEarn = User has filled in all three meals in their food and drink diary </summary>
        _You_checked_in_on_threesquare = 5,

        /// <summary> ActionToEarn = User has logged water in their food diary </summary>
        _Hydrated = 6,

        /// <summary> ActionToEarn = User has filled in food and drink diary for entire season </summary>
        _Captains_Log = 7,

        /// <summary> ActionToEarn = Upload 3 or more progress photos </summary>
        _Looking_good = 8,

        /// <summary> ActionToEarn = A player hasn't uploaded a progress photo within 2 weeks of starting the progress dashboard.  </summary>
        _Not_Vain = 9,

        /// <summary> ActionToEarn = Player makes their first GIF </summary>
        _Getting_Giffy_with_it = 10,

        /// <summary> ActionToEarn = Player uploaded a progress pic every week for one season </summary>
        _Pic_or_it_didnt_happen = 11,

        /// <summary> ActionToEarn = User made first collage </summary>
        _Collage_Injection = 12,

        /// <summary> ActionToEarn = User tracked breakfast in food diary for the first time </summary>
        _It_IS_the_most_important_meal_of_the_day = 13,

        /// <summary> ActionToEarn = User has filled in their food and drink diary for 30 days </summary>
        _Monster_Month = 14,

        /// <summary> ActionToEarn = Missed 2 weigh ins in a row </summary>
        _No_weigh_Jose = 15,

        /// <summary> ActionToEarn = Attended three consecutive weigh ins </summary>
        _Weigh_to_go = 16,

        /// <summary> ActionToEarn = User has filled in 7 days of their food and drink diary </summary>
        _Chilled_on_Sunday = 17,

        /// <summary> ActionToEarn = Player has not uploaded a progress pic for the whole season </summary>
        _Vampire = 18,

        /// <summary> ActionToEarn = Player has filled in their food and drink diary 50 times </summary>
        _Bridget_Jones = 19,

        /// <summary> ActionToEarn = Player has filled in their food and drink diary 100 times </summary>
        _Adrian_Mole = 20,

        /// <summary> ActionToEarn = Player has filled in their breakfast 10 times </summary>
        _Snap = 21,

        /// <summary> ActionToEarn = Player has filled in their breakfast 50 times </summary>
        _Crackle = 22,

        /// <summary> ActionToEarn = Player has filled in their breakfast 100 times </summary>
        _Pop = 23,

        /// <summary> ActionToEarn = Player has filled in their lunch 10 times </summary>
        _Lets_do_lunch = 24,

        /// <summary> ActionToEarn = Player has filled in their lunch 50 times </summary>
        _Lets_do_lunch_again = 25,

        /// <summary> ActionToEarn = Player has filled in their lunch 100 times </summary>
        _Still_diggin_that_lunch_huh = 26,

        /// <summary> ActionToEarn = Player has filled in their dinner 10 times </summary>
        _Winner_winner_tracking_dinner = 27,

        /// <summary> ActionToEarn = Player has filled in their dinner 50 times </summary>
        _Dinner_dinner_on_the_wall = 28,

        /// <summary> ActionToEarn = Player has filled in their dinner 100 times </summary>
        _Whoa_dinner_man = 29,

        /// <summary> ActionToEarn = Player has tracked snacks 10 times </summary>
        _Snack_attack = 30,

        /// <summary> ActionToEarn = Player has tracked snacks 50 times </summary>
        _Snack_attack_II = 31,

        /// <summary> ActionToEarn = Player has tracked snacks 100 times </summary>
        _Snack_attack_III = 32,

        /// <summary> ActionToEarn = User’s BMI goes under 25 within 9 months of joining MAN v FAT </summary>
        _BMI_baby = 33,

        /// <summary> ActionToEarn = User loses 5% of starting weight </summary>
        _5_Percenter = 34,

        /// <summary> ActionToEarn = User loses 10% of starting weight </summary>
        _10_Percenter = 35,

        /// <summary> ActionToEarn = User records only losses for a whole season </summary>
        _No_gain_no_pain = 36,

        /// <summary> ActionToEarn = User goes 3 weeks without a weight gain </summary>
        _Not_a_loser = 37,

        /// <summary> ActionToEarn = User's BMI is no longer classed as obese </summary>
        _Nobese = 38,

        /// <summary> ActionToEarn = User records only losses for 28 days </summary>
        _28_Days_Lighter = 39,

        /// <summary> ActionToEarn = User gains weight </summary>
        _Gain_pain = 40,

        /// <summary> ActionToEarn = User’s weight stays the same for 2 weeks </summary>
        _Camped = 41,

        /// <summary> ActionToEarn = User loses weight after season break </summary>
        _Great_off_season = 42,

        /// <summary> ActionToEarn = User records weight loss in first week </summary>
        _Great_start = 43,

        /// <summary> ActionToEarn = User misses a weigh in </summary>
        _The_weigh_in_is_the_hardest_part = 44,

        /// <summary> ActionToEarn = User is on maintenance program for whole season </summary>
        _High_maintenance = 45,

        /// <summary> ActionToEarn = User attends every weigh in for one season </summary>
        _Committed = 46,

        /// <summary> ActionToEarn = User loses weight after gaining weight the previous week </summary>
        _Back_2_it = 47,

        /// <summary> ActionToEarn = User loses weight 3 weeks in a row </summary>
        _3s_a_crowd = 48,

        /// <summary> ActionToEarn = User loses 15% of starting weight </summary>
        _15_Percenter = 49,

        /// <summary> ActionToEarn = User loses 20% of starting weight </summary>
        _20_Percenter = 50,

        /// <summary> ActionToEarn = User loses 25% of starting weight </summary>
        _25_Percenter = 51,

        /// <summary> ActionToEarn = User loses 30% of starting weight </summary>
        _30_Percenter = 52,

        /// <summary> ActionToEarn = User loses 35% of starting weight </summary>
        _35_Percenter = 53,

        /// <summary> ActionToEarn = User loses 40% of starting weight </summary>
        _40_Percenter = 54,

        /// <summary> ActionToEarn = User loses 45% of starting weight </summary>
        _45_Percenter = 55,

        /// <summary> ActionToEarn = User loses 50% of starting weight </summary>
        _Half_off = 56,

        /// <summary> ActionToEarn = User loses 5lbs </summary>
        _5lbs = 57,

        /// <summary> ActionToEarn = User loses 10lbs </summary>
        _10lbs = 58,

        /// <summary> ActionToEarn = User loses 15lbs </summary>
        _15lbs = 59,

        /// <summary> ActionToEarn = User loses 20lbs </summary>
        _20lbs = 60,

        /// <summary> ActionToEarn = User loses 25lbs </summary>
        _25lbs = 61,

        /// <summary> ActionToEarn = User loses 30lbs </summary>
        _30lbs = 62,

        /// <summary> ActionToEarn = User loses 35lbs </summary>
        _35lbs = 63,

        /// <summary> ActionToEarn = User loses 40lbs </summary>
        _40lbs = 64,

        /// <summary> ActionToEarn = User loses 45lbs </summary>
        _45lbs = 65,

        /// <summary> ActionToEarn = User loses 50lbs </summary>
        _50lbs = 66,

        /// <summary> ActionToEarn = User loses 55lbs </summary>
        _55lbs = 67,

        /// <summary> ActionToEarn = User loses 60lbs </summary>
        _60lbs = 68,

        /// <summary> ActionToEarn = User loses 65lbs </summary>
        _65lbs = 69,

        /// <summary> ActionToEarn = User loses 70lbs </summary>
        _70lbs = 70,

        /// <summary> ActionToEarn = User loses 75lbs </summary>
        _75lbs = 71,

        /// <summary> ActionToEarn = User loses 80lbs </summary>
        _80lbs = 72,

        /// <summary> ActionToEarn = User loses 85lbs </summary>
        _85lbs = 73,

        /// <summary> ActionToEarn = User loses 90lbs </summary>
        _90lbs = 74,

        /// <summary> ActionToEarn = User loses 95lbs </summary>
        _95lbs = 75,

        /// <summary> ActionToEarn = User loses 100lbs </summary>
        _100lbs = 76,

        /// <summary> ActionToEarn = User loses 105lbs </summary>
        _105lbs = 77,

        /// <summary> ActionToEarn = User loses 110lbs </summary>
        _110lbs = 78,

        /// <summary> ActionToEarn = User loses 115lbs </summary>
        _115lbs = 79,

        /// <summary> ActionToEarn = User loses 120lbs </summary>
        _120lbs = 80,

        /// <summary> ActionToEarn = User loses 125lbs </summary>
        _125lbs = 81,

        /// <summary> ActionToEarn = User loses 130lbs </summary>
        _130lbs = 82,

        /// <summary> ActionToEarn = User loses 135lbs </summary>
        _135lbs = 83,

        /// <summary> ActionToEarn = User loses 140lbs </summary>
        _140lbs = 84,

        /// <summary> ActionToEarn = User loses 145lbs </summary>
        _145lbs = 85,

        /// <summary> ActionToEarn = User loses 150lbs </summary>
        _150lbs = 86,

        /// <summary> ActionToEarn = User loses 5kg </summary>
        _5kg = 87,

        /// <summary> ActionToEarn = User loses 10kg </summary>
        _10kg = 88,

        /// <summary> ActionToEarn = User loses 15kg </summary>
        _15kg = 89,

        /// <summary> ActionToEarn = User loses 20kg </summary>
        _20kg = 90,

        /// <summary> ActionToEarn = User loses 25kg </summary>
        _25kg = 91,

        /// <summary> ActionToEarn = User loses 30kg </summary>
        _30kg = 92,

        /// <summary> ActionToEarn = User loses 35kg </summary>
        _35kg = 93,

        /// <summary> ActionToEarn = User loses 40kg </summary>
        _40kg = 94,

        /// <summary> ActionToEarn = User loses 45kg </summary>
        _45kg = 95,

        /// <summary> ActionToEarn = User loses 50kg </summary>
        _50kg = 96,

        /// <summary> ActionToEarn = User loses 55kg </summary>
        _55kg = 97,

        /// <summary> ActionToEarn = User loses 60kg </summary>
        _60kg = 98,

        /// <summary> ActionToEarn = User loses 65kg </summary>
        _65kg = 99,

        /// <summary> ActionToEarn = User loses 70kg </summary>
        _70kg = 100,

        /// <summary> ActionToEarn = Player records weight entry 3 times </summary>
        _Scale_watcher = 101,

        /// <summary> ActionToEarn = Player records weight entry 10 times </summary>
        _Scale_fancier = 102,

        /// <summary> ActionToEarn = Player records weight entry 20 times </summary>
        _Scale_collector_magazine_monthly = 103,

        /// <summary> ActionToEarn = User is half way to their goal </summary>
        _Livin_on_a_prayer = 104,

        /// <summary> ActionToEarn = User reaches goal </summary>
        _Goooooooooooooaal = 105,

        /// <summary> ActionToEarn = User's BMI is no longer classed as overweight </summary>
        _Healthy_BMI = 106,

        /// <summary> ActionToEarn = User's weight in pounds now begins with a 1 </summary>
        _Onederland = 107,

        /// <summary> ActionToEarn = User's weight in kilos is now double digits </summary>
        _Double_Digits = 108,

        /// <summary> ActionToEarn = User has lost one stone </summary>
        _Stone_Roses = 109,

        /// <summary> ActionToEarn = User has lost two stone </summary>
        _Rolling_Stones = 110,

        /// <summary> ActionToEarn = User has lost five stone </summary>
        _Queens_of_the_Stone_Age = 111,

        /// <summary> ActionToEarn = User has lost ten stone </summary>
        _Joss_Stone = 112,

        /// <summary> ActionToEarn = User has lost twenty stone </summary>
        _Stone_Cold_Crazy = 113,

        /// <summary> ActionToEarn = Completed 100% of activity in 1 week </summary>
        _Gym_Class_Hero = 114,

        /// <summary> ActionToEarn = Logged first activity </summary>
        _First_timer = 115,

        /// <summary> ActionToEarn = Logged activity 7 days in a row </summary>
        _Active = 116,

        /// <summary> ActionToEarn = Logged activity 28 days in a row </summary>
        _Pro_Active = 117,

        /// <summary> ActionToEarn = Logged activity 3 days in a row </summary>
        _Consistency_is_the_key = 118,

        /// <summary> ActionToEarn = Logged activity every day for one season </summary>
        _Hyper_Active = 119,

        /// <summary> ActionToEarn = No activity logged for 7 days </summary>
        _Lazy_Bones = 120,

        /// <summary> ActionToEarn = Activity logged twice in one day </summary>
        _Twofer = 121,

        /// <summary> ActionToEarn = No activity logged for one season </summary>
        _Couch_Potato = 122,

        /// <summary> ActionToEarn = User has logged activity for 1 week </summary>
        _Tracker_bar = 123,

        /// <summary> ActionToEarn = User has logged activity for 3 weeks </summary>
        _Tracker_app = 124,

        /// <summary> ActionToEarn = User has logged activity for 6 weeks </summary>
        _Tracker_from_Paw_Patrol = 125,

        /// <summary> ActionToEarn = User has logged 150 minutes of activity in one week </summary>
        _NH_Yes = 126,

        /// <summary> ActionToEarn = If you have friends linked in the system then both players get this achievement. </summary>
        _Oooh_Friend = 127,

        /// <summary> ActionToEarn = User refers a friend </summary>
        _Referrer = 128,

        /// <summary> ActionToEarn = User has unlocked 2 achievements </summary>
        _Twomescent = 129,

        /// <summary> ActionToEarn = User has unlocked 10 achievements </summary>
        _Ten_Ting = 130,

        /// <summary> ActionToEarn = User is picked as a team captain </summary>
        _Designated_driver = 131,

        /// <summary> ActionToEarn = User has unlocked all achievements </summary>
        _Completer = 132,

        /// <summary> ActionToEarn = User has 10 friends </summary>
        _Popular = 133,

        /// <summary> ActionToEarn = User has referred 5 people </summary>
        _Talent_Scout = 134,

        /// <summary> ActionToEarn = User played first game </summary>
        _Kick_off = 135,

        /// <summary> ActionToEarn = User’s team loses on the pitch but they lose on the scales. </summary>
        _They_cant_blame_you = 136,

        /// <summary> ActionToEarn = User’s team wins the season </summary>
        _We_are_the_champions = 137,

        /// <summary> ActionToEarn = User's team wins a game on the scales that was lost on the pitch </summary>
        _Scale_x_Tricks = 138,

        /// <summary> ActionToEarn = User has completed one season </summary>
        _They_think_its_all_over = 139,

        /// <summary> ActionToEarn = User’s team wins a game on the pitch that was lost on the scales </summary>
        _Tipped_the_scales = 140,

        /// <summary> ActionToEarn = User changes teams </summary>
        _Move_Move_Move = 141,

        /// <summary> ActionToEarn = User loses the most weight on their team </summary>
        _MVP = 142,

        /// <summary> ActionToEarn = User's team wins 3 games in a row </summary>
        _Three_Games_Won = 143,

        /// <summary> ActionToEarn = User is the top scorer on his team </summary>
        _Goal_machine = 144,

        /// <summary> ActionToEarn = User scores a hatrick </summary>
        _Rabbit_rabbit_rabbit = 145,
    }
}