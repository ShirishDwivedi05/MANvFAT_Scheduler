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
    
    public partial class SelectPlayers_Result
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
    }
}