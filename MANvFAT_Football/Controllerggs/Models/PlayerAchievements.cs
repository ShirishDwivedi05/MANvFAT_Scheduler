//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated from a template.
//
//     Manual changes to this file may cause unexpected behavior in your application.
//     Manual changes to this file will be overwritten if the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace MANvFAT_Football.Models
{
    using System;
    using System.Collections.Generic;
    
    public partial class PlayerAchievements
    {
        public long PlayerAchievementID { get; set; }
        public long PlayerID { get; set; }
        public long AchievementID { get; set; }
        public System.DateTime AchievementDateTime { get; set; }
        public byte[] RowVersion { get; set; }
    
        public virtual Achievements Achievements { get; set; }
        public virtual Players Players { get; set; }
    }
}
