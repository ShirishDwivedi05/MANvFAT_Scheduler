using System.Collections.Generic;
using System.Linq;

namespace MANvFAT_Football.Models.Repositories
{
    public class PlayerAchievementsRepository : BaseRepository
    {
        public List<PlayerAchievements> ReadAll(long PlayerID)
        {
            return db.PlayerAchievements.Where(m => m.PlayerID == PlayerID).ToList();
        }

        public List<PlayerAchievementsExt> ReadAll_Dashboard(long PlayerID)
        {
            return db.PlayerAchievements.Where(m => m.PlayerID == PlayerID).ToList().Select(m => Map(m)).ToList();
        }

        public List<SelectPlayer_PecentAchievementCategoryCompleted_Dashboard_Result> Read_PecentAchievementCategoryCompleted(long PlayerID)
        {
            return db.SelectPlayer_PecentAchievementCategoryCompleted_Dashboard(PlayerID).ToList();
        }

        public List<SelectPlayer_Locked_Unlocked_Achievements_Dashboard_Result> Read_PecentAchievementCategoryCompleted(long PlayerID, long AchievementCategoryID)
        {
            return db.SelectPlayer_Locked_Unlocked_Achievements_Dashboard(PlayerID, AchievementCategoryID).ToList();
        }

        public PlayerAchievementsExt Map(PlayerAchievements m)
        {
            PlayerAchievementsExt model = new PlayerAchievementsExt()
            {
                PlayerAchievementID = m.PlayerAchievementID,
                PlayerID = m.PlayerID,
                PlayerFullName = m.Players.FullName,
                AchievementID = m.AchievementID,
                Title = m.Achievements.Title,
                Description = m.Achievements.Description,
                ActionToEarn = m.Achievements.ActionToEarn,
                Points = m.Achievements.Points
            };

            return model;
        }
    }

    public class PlayerAchievementsExt
    {
        public long PlayerAchievementID { get; set; }
        public long PlayerID { get; set; }
        public string PlayerFullName { get; set; }
        public long AchievementID { get; set; }
        public string Title { get; set; }
        public string Description { get; set; }
        public string ActionToEarn { get; set; }
        public int Points { get; set; }
    }

    
}