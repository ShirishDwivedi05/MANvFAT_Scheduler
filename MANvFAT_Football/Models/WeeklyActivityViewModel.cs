using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace MANvFAT_Football.ViewModels
{
    public class WeeklyActivityViewModel
    {
        public string Week { get; set; }

        public int Over { get; set; }

        public int Under { get; set; }

        public int Target150Mins { get; set; }

        public DateTime CreatedDate { get; set; }

        public static List<WeeklyActivityViewModel> GetWeeklyActivity(string period)
        {
            var model = new List<WeeklyActivityViewModel>() {
                new WeeklyActivityViewModel()
                {
                    Week = "week 1",
                    Over = 98,
                    Under = 0,
                    Target150Mins =70,
                    CreatedDate =DateTime.UtcNow.AddMonths(-1)
                },
                 new WeeklyActivityViewModel()
                {
                    Week = "week 2",
                    Over = 78,
                    Under = 20,
                    Target150Mins =70,
                    CreatedDate =DateTime.UtcNow.AddMonths(-1)
                },
                    new WeeklyActivityViewModel()
                {
                    Week = "week 3",
                    Over = 78,
                    Under = 20,
                    Target150Mins =70,
                    CreatedDate =DateTime.UtcNow.AddMonths(-1)
                },
                       new WeeklyActivityViewModel()
                {
                    Week = "week 4",
                    Over = 78,
                    Under = 20,
                    Target150Mins =70,
                    CreatedDate =DateTime.UtcNow.AddMonths(-1)
                },
             new WeeklyActivityViewModel()
                {
                    Week = "week 1",
                    Over = 78,
                    Under = 20,
                    Target150Mins =70,
                    CreatedDate =DateTime.UtcNow.AddMonths(-3)
                },
               new WeeklyActivityViewModel()
                {
                    Week = "week 2",
                    Over = 78,
                    Under = 20,
                    Target150Mins =70,
                    CreatedDate =DateTime.UtcNow.AddMonths(-3)
                },
              new WeeklyActivityViewModel()
                {
                    Week = "week 3",
                    Over = 78,
                    Under = 20,
                    Target150Mins =70,
                    CreatedDate =DateTime.UtcNow.AddMonths(-3)
                },
             new WeeklyActivityViewModel()
                {
                    Week = "week 4",
                    Over = 78,
                    Under = 20,
                    Target150Mins =70,
                    CreatedDate =DateTime.UtcNow.AddMonths(-3)
                },
            new WeeklyActivityViewModel()
                {
                    Week = "week 1",
                    Over = 78,
                    Under = 20,
                    Target150Mins =70,
                    CreatedDate =DateTime.UtcNow.AddDays(-7)
                },
               new WeeklyActivityViewModel()
                {
                    Week = "week 2",
                    Over = 78,
                    Under = 20,
                    Target150Mins =70,
                    CreatedDate =DateTime.UtcNow.AddDays(-7)
                },
                 new WeeklyActivityViewModel()
                {
                    Week = "week 3",
                    Over = 78,
                    Under = 20,
                    Target150Mins =70,
                    CreatedDate =DateTime.UtcNow.AddDays(-7)
                },
                  new WeeklyActivityViewModel()
                {
                    Week = "week 4",
                    Over = 78,
                    Under = 20,
                    Target150Mins =70,
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