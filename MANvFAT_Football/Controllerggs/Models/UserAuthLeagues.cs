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
    
    public partial class UserAuthLeagues
    {
        public long UserAuthLeagueID { get; set; }
        public long UserID { get; set; }
        public long LeagueID { get; set; }
        public byte[] RowVersion { get; set; }
    
        public virtual Leagues Leagues { get; set; }
        public virtual Users Users { get; set; }
    }
}