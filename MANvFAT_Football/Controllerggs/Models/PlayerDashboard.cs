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
    
    public partial class PlayerDashboard
    {
        public long PlayerDashboardID { get; set; }
        public long PlayerID { get; set; }
        public string DashboardURL { get; set; }
        public string DashboardPassword { get; set; }
        public Nullable<decimal> TargetWeight { get; set; }
        public string PasswordResetCode { get; set; }
        public Nullable<System.DateTime> ResetCodeExpiry { get; set; }
        public bool Locked { get; set; }
        public bool Deleted { get; set; }
        public string LoginSessionID { get; set; }
        public bool IsFirstLogin { get; set; }
        public System.DateTime DashboardCreatedDate { get; set; }
        public System.DateTime DashboardExpiryDate { get; set; }
        public Nullable<int> ShareDataFrequency { get; set; }
        public Nullable<int> ShareDataWith { get; set; }
        public int DayOfWeek { get; set; }
        public string AdditionalRecipients { get; set; }
        public string OptionalMessage { get; set; }
        public Nullable<System.DateTime> LastSentDate { get; set; }
        public Nullable<int> ReminderTime { get; set; }
        public byte[] RowVersion { get; set; }
    
        public virtual Players Players { get; set; }
    }
}
