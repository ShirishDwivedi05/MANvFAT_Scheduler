using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MANvFAT_Football.ViewModels
{
    public class WeightViewModel
    {
        public int ActualWeight { get; set; }

        public int TargetWeight { get; set; }

        public int ProjectedWeight { get; set; }

        public DateTime CreatedDate { get; set; }

        public static List<WeightViewModel> GetWeightData(string period)
        {
            var model = new List<WeightViewModel>() {
                new WeightViewModel()
                {
                   ActualWeight = 100,
                    TargetWeight = 80,
                    ProjectedWeight = 85,
                    CreatedDate =DateTime.UtcNow.AddMonths(-1)
                },
                new WeightViewModel()
                {
                   ActualWeight = 100,
                    TargetWeight = 80,
                    ProjectedWeight = 85,
                    CreatedDate =DateTime.UtcNow.AddMonths(-1)
                },
                new WeightViewModel()
                {
                   ActualWeight = 90,
                    TargetWeight = 75,
                    ProjectedWeight = 80,
                    CreatedDate =DateTime.UtcNow.AddMonths(-1)
                },
               new WeightViewModel()
                {
                   ActualWeight = 100,
                    TargetWeight = 80,
                    ProjectedWeight = 85,
                    CreatedDate =DateTime.UtcNow.AddMonths(-1)
                },
                new WeightViewModel()
                {
                   ActualWeight = 90,
                    TargetWeight = 75,
                    ProjectedWeight = 80,
                    CreatedDate =DateTime.UtcNow.AddMonths(-1)
                },

                  new WeightViewModel()
                {
                   ActualWeight = 100,
                    TargetWeight = 80,
                    ProjectedWeight = 85,
                    CreatedDate =DateTime.UtcNow.AddMonths(-3)
                },
                new WeightViewModel()
                {
                   ActualWeight = 90,
                    TargetWeight = 75,
                    ProjectedWeight = 80,
                    CreatedDate =DateTime.UtcNow.AddMonths(-3)
                },
               new WeightViewModel()
                {
                   ActualWeight = 100,
                    TargetWeight = 80,
                    ProjectedWeight = 85,
                    CreatedDate =DateTime.UtcNow.AddMonths(-3)
                },
                new WeightViewModel()
                {
                   ActualWeight = 90,
                    TargetWeight = 75,
                    ProjectedWeight = 80,
                    CreatedDate =DateTime.UtcNow.AddMonths(-3)
                },

                  new WeightViewModel()
                {
                   ActualWeight = 100,
                    TargetWeight = 80,
                    ProjectedWeight = 85,
                    CreatedDate =DateTime.UtcNow.AddMonths(-6)
                },
                new WeightViewModel()
                {
                   ActualWeight = 90,
                    TargetWeight = 75,
                    ProjectedWeight = 80,
                    CreatedDate =DateTime.UtcNow.AddMonths(-6)
                },
               new WeightViewModel()
                {
                   ActualWeight = 100,
                    TargetWeight = 80,
                    ProjectedWeight = 85,
                    CreatedDate =DateTime.UtcNow.AddMonths(-6)
                },
                new WeightViewModel()
                {
                   ActualWeight = 90,
                    TargetWeight = 75,
                    ProjectedWeight = 80,
                    CreatedDate =DateTime.UtcNow.AddMonths(-6)
                },

                  new WeightViewModel()
                {
                   ActualWeight = 100,
                    TargetWeight = 80,
                    ProjectedWeight = 85,
                    CreatedDate =DateTime.UtcNow.AddDays(-7)
                },
                new WeightViewModel()
                {
                   ActualWeight = 90,
                    TargetWeight = 75,
                    ProjectedWeight = 80,
                    CreatedDate =DateTime.UtcNow.AddDays(-7)
                },
               new WeightViewModel()
                {
                   ActualWeight = 100,
                    TargetWeight = 80,
                    ProjectedWeight = 85,
                    CreatedDate =DateTime.UtcNow.AddDays(-7)
                },
                new WeightViewModel()
                {
                   ActualWeight = 90,
                    TargetWeight = 75,
                    ProjectedWeight = 80,
                    CreatedDate =DateTime.UtcNow.AddDays(-7)
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