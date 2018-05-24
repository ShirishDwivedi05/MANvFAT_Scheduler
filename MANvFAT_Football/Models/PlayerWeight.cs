using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MANvFAT_Football.Models
{
    public class PlayerWeight
    {
        public int PlayerId { get; set; }
        public int KgValue { get; set; }
        public int LbsValue { get; set; }
        public DateTime ActivityDate { get; set; } = DateTime.Now;
    }
}