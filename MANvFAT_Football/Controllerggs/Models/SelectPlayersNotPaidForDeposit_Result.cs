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
    
    public partial class SelectPlayersNotPaidForDeposit_Result
    {
        public Nullable<long> PlayerID { get; set; }
        public Nullable<long> PlayerPaymentID { get; set; }
        public string FullName { get; set; }
        public string Mobile { get; set; }
        public string EmailAddress { get; set; }
        public Nullable<System.DateTime> RegistrationDate { get; set; }
        public Nullable<System.DateTime> PaymentDateTime { get; set; }
        public string PaylinkID { get; set; }
        public Nullable<bool> IsLocalAuthorityScheme { get; set; }
        public Nullable<bool> PaidWithPayPal { get; set; }
        public Nullable<bool> isDepositAmount { get; set; }
    }
}
