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
    
    public partial class Cities
    {
        public long CityID { get; set; }
        public long RegionID { get; set; }
        public string CityName { get; set; }
    
        public virtual Regions Regions { get; set; }
    }
}
