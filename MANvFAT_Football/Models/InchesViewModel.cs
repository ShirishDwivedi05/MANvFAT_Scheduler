using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MANvFAT_Football.ViewModels
{
    public class InchesViewModel
    {

        public string PartName { get; set; }

        public int CurrentInch { get; set; }

        public int InchLost { get; set; }

        public DateTime CreatedDate { get; set; }

        public static List<InchesViewModel> GetInchesData(string period)
        {
            var model = new List<InchesViewModel>() {
                new InchesViewModel()
                {
                    PartName = "Neck",
                    CurrentInch = 180,
                    InchLost = 186,
                    CreatedDate =DateTime.UtcNow.AddMonths(-1)
                },
                new InchesViewModel()
                {
                    PartName = "Right Bicep",
                    CurrentInch = 127,
                    InchLost = 198,
                    CreatedDate =DateTime.UtcNow.AddMonths(-1)
                },
                new InchesViewModel()
                {
                    PartName = "Left bicep",
                    CurrentInch = 73,
                    InchLost = 74,
                    CreatedDate =DateTime.UtcNow.AddMonths(-1)
                },
                 new InchesViewModel()
                 {
                     PartName = "Waist",
                     CurrentInch = 16,
                     InchLost = 125,
                     CreatedDate =DateTime.UtcNow.AddMonths(-1)
                 },
                 new InchesViewModel()
                 {
                     PartName = "Hips",
                     CurrentInch = 14,
                     InchLost = 97,
                     CreatedDate =DateTime.UtcNow.AddMonths(-1)
                 },
                  new InchesViewModel()
                  {
                      PartName = "Right Thigh",
                      CurrentInch = 93,
                      InchLost = 43,
                      CreatedDate =DateTime.UtcNow.AddMonths(-1)
                  },
                    new InchesViewModel()
                    {
                        PartName = "Left Thigh",
                        CurrentInch = 45,
                        InchLost = 162,
                        CreatedDate =DateTime.UtcNow.AddMonths(-1)
                    },



                     new InchesViewModel()
                {
                    PartName = "Neck",
                    CurrentInch = 180,
                    InchLost = 186,
                    CreatedDate =DateTime.UtcNow.AddMonths(-3)
                },
                new InchesViewModel()
                {
                    PartName = "Right Bicep",
                    CurrentInch = 127,
                    InchLost = 198,
                     CreatedDate =DateTime.UtcNow.AddMonths(-3)
                },
                new InchesViewModel()
                {
                    PartName = "Left bicep",
                    CurrentInch = 73,
                    InchLost = 74,
                     CreatedDate =DateTime.UtcNow.AddMonths(-3)
                },
                 new InchesViewModel()
                 {
                     PartName = "Waist",
                     CurrentInch = 16,
                     InchLost = 125,
                      CreatedDate =DateTime.UtcNow.AddMonths(-3)
                 },
                 new InchesViewModel()
                 {
                     PartName = "Hips",
                     CurrentInch = 14,
                     InchLost = 97,
                      CreatedDate =DateTime.UtcNow.AddMonths(-3)
                 },
                  new InchesViewModel()
                  {
                      PartName = "Right Thigh",
                      CurrentInch = 93,
                      InchLost = 43,
                       CreatedDate =DateTime.UtcNow.AddMonths(-3)
                  },
                    new InchesViewModel()
                    {
                        PartName = "Left Thigh",
                        CurrentInch = 45,
                        InchLost = 162,
                         CreatedDate =DateTime.UtcNow.AddMonths(-3)
                    },


                      new InchesViewModel()
                {
                    PartName = "Neck",
                    CurrentInch = 180,
                    InchLost = 186,
                    CreatedDate =DateTime.UtcNow.AddDays(-7)
                },
                new InchesViewModel()
                {
                    PartName = "Right Bicep",
                    CurrentInch = 127,
                    InchLost = 198,
                     CreatedDate =DateTime.UtcNow.AddDays(-7)
                },
                new InchesViewModel()
                {
                    PartName = "Left bicep",
                    CurrentInch = 73,
                    InchLost = 74,
                     CreatedDate =DateTime.UtcNow.AddDays(-7)
                },
                 new InchesViewModel()
                 {
                     PartName = "Waist",
                     CurrentInch = 16,
                     InchLost = 125,
                      CreatedDate =DateTime.UtcNow.AddDays(-7)
                 },
                 new InchesViewModel()
                 {
                     PartName = "Hips",
                     CurrentInch = 14,
                     InchLost = 97,
                      CreatedDate =DateTime.UtcNow.AddDays(-7)
                 },
                  new InchesViewModel()
                  {
                      PartName = "Right Thigh",
                      CurrentInch = 93,
                      InchLost = 43,
                       CreatedDate =DateTime.UtcNow.AddDays(-7)
                  },
                    new InchesViewModel()
                    {
                        PartName = "Left Thigh",
                        CurrentInch = 45,
                        InchLost = 162,
                         CreatedDate =DateTime.UtcNow.AddDays(-7)
                    },


                   new InchesViewModel()
                {
                    PartName = "Neck",
                    CurrentInch = 180,
                    InchLost = 186,
                    CreatedDate =DateTime.UtcNow.AddMonths(-6)
                },
                new InchesViewModel()
                {
                    PartName = "Right Bicep",
                    CurrentInch = 127,
                    InchLost = 198,
                     CreatedDate =DateTime.UtcNow.AddMonths(-6)
                },
                new InchesViewModel()
                {
                    PartName = "Left bicep",
                    CurrentInch = 73,
                    InchLost = 74,
                     CreatedDate =DateTime.UtcNow.AddMonths(-6)
                },
                 new InchesViewModel()
                 {
                     PartName = "Waist",
                     CurrentInch = 16,
                     InchLost = 125,
                      CreatedDate =DateTime.UtcNow.AddMonths(-6)
                 },
                 new InchesViewModel()
                 {
                     PartName = "Hips",
                     CurrentInch = 14,
                     InchLost = 97,
                      CreatedDate =DateTime.UtcNow.AddMonths(-6)
                 },
                  new InchesViewModel()
                  {
                      PartName = "Right Thigh",
                      CurrentInch = 93,
                      InchLost = 43,
                       CreatedDate =DateTime.UtcNow.AddMonths(-6)
                  },
                    new InchesViewModel()
                    {
                        PartName = "Left Thigh",
                        CurrentInch = 45,
                        InchLost = 162,
                         CreatedDate =DateTime.UtcNow.AddMonths(-6)
                    },

            };

            if (string.IsNullOrEmpty(period))
            {
                return model;
            }
            period = period.Trim().ToLower();
            if (period == "all" || period == "years")
            {
                return model;
            }

            else if (period == "week")
            {
                DateTime now = DateTime.UtcNow;
                DateTime endDate = now;
                DateTime startDate = now.AddDays(-7);
                return model.Where(p => p.CreatedDate >= startDate && p.CreatedDate <= endDate).ToList();
            }

            else if (period == "1month")
            {
                DateTime now = DateTime.UtcNow;
                DateTime endDate = now;
                DateTime startDate = now.AddMonths(-1);
                return model.Where(p => p.CreatedDate >= startDate && p.CreatedDate <= endDate).ToList();
            }
            else if (period == "3months")
            {
                DateTime now = DateTime.UtcNow;
                DateTime endDate = now;
                DateTime startDate = now.AddMonths(-3);
                return model.Where(p => p.CreatedDate >= startDate && p.CreatedDate <= endDate).ToList();
            }

            return model;
        }
    }

}