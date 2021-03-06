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
    
    public partial class DashboardNotifications
    {
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2214:DoNotCallOverridableMethodsInConstructors")]
        public DashboardNotifications()
        {
            this.PlayerDashboardNotifications = new HashSet<PlayerDashboardNotifications>();
        }
    
        public long DashboardNotificationID { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public string Link { get; set; }
        public System.DateTime NotificationDateTime { get; set; }
        public Nullable<long> LeagueID { get; set; }
        public bool IsRecurring { get; set; }
        public Nullable<int> RecurringFrequency { get; set; }
        public Nullable<int> DayOfWeek { get; set; }
        public bool IsAchievementNotification { get; set; }
        public byte[] RowVersion { get; set; }
    
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA2227:CollectionPropertiesShouldBeReadOnly")]
        public virtual ICollection<PlayerDashboardNotifications> PlayerDashboardNotifications { get; set; }
    }
}
