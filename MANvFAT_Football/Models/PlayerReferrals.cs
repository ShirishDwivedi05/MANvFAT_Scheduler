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
    
    public partial class PlayerReferrals
    {
        public long PlayerReferralID { get; set; }
        public long ReferrerPlayerID { get; set; }
        public long ReferralPlayerID { get; set; }
        public System.DateTime DateReferred { get; set; }
        public byte[] RowVersion { get; set; }
        public bool IsPaid { get; set; }
        public Nullable<System.DateTime> PaymentSentDate { get; set; }
    
        public virtual Players Players { get; set; }
        public virtual Players Players1 { get; set; }
    }
}
