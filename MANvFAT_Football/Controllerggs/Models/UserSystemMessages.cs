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
    
    public partial class UserSystemMessages
    {
        public long UserSystemMessageID { get; set; }
        public long UserID { get; set; }
        public long SystemMessageID { get; set; }
        public bool Dismissed { get; set; }
        public Nullable<System.DateTime> DismissedDateTime { get; set; }
    
        public virtual SystemMessages SystemMessages { get; set; }
        public virtual Users Users { get; set; }
    }
}