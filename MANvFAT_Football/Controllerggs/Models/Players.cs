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
    
    public partial class Players
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public Players()
        {
            this.PlayerAchievements = new HashSet<PlayerAchievements>();
            this.PlayerDailyActivities = new HashSet<PlayerDailyActivities>();
            this.PlayerDashboard = new HashSet<PlayerDashboard>();
            this.PlayerDashboardNotifications = new HashSet<PlayerDashboardNotifications>();
            this.PlayerImages = new HashSet<PlayerImages>();
            this.PlayerScoreBreakdowns = new HashSet<PlayerScoreBreakdowns>();
            this.PlayerWeeklyActivities = new HashSet<PlayerWeeklyActivities>();
            this.PlayerWeightLogs = new HashSet<PlayerWeightLogs>();
            this.PlayerWeights = new HashSet<PlayerWeights>();
            this.PlayerWeightWeeks = new HashSet<PlayerWeightWeeks>();
        }
    
        public long PlayerID { get; set; }
        public long TitleID { get; set; }
        public string FirstName { get; set; }
        public string LastName { get; set; }
        public string FullName { get; set; }
        public string EmailAddress { get; set; }
        public Nullable<System.DateTime> DOB { get; set; }
        public Nullable<int> Age { get; set; }
        public decimal Weight { get; set; }
        public string Mobile { get; set; }
        public long HeightID { get; set; }
        public string PostCode { get; set; }
        public bool Active { get; set; }
        public bool Featured { get; set; }
        public byte[] RowVersion { get; set; }
        public System.DateTime RegistrationDate { get; set; }
        public Nullable<long> AdvertisementID { get; set; }
        public bool AdvertisementOther { get; set; }
        public string AdvertisementOtherDetails { get; set; }
    
        public virtual Advertisements Advertisements { get; set; }
        public virtual Heights Heights { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerAchievements> PlayerAchievements { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerDailyActivities> PlayerDailyActivities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerDashboard> PlayerDashboard { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerDashboardNotifications> PlayerDashboardNotifications { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerImages> PlayerImages { get; set; }
        public virtual Titles Titles { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerScoreBreakdowns> PlayerScoreBreakdowns { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerWeeklyActivities> PlayerWeeklyActivities { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerWeightLogs> PlayerWeightLogs { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerWeights> PlayerWeights { get; set; }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerWeightWeeks> PlayerWeightWeeks { get; set; }
    }
}
