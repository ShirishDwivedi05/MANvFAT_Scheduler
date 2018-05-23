using System.Collections.Generic;
using System.Linq;

namespace MANvFAT_Football.Models.Repositories
{
    public class ProgressChartsRepository : BaseRepository
    {

        public ProgressChart ReadAll_Dashboard(long PlayerID)
        {
            ProgressChart model = new ProgressChart();
            model.PlayerID = PlayerID;
            model.Percent_CompletedWeightTarget = ReadWeightPieChartData(PlayerID).value.ToString("N0");
            model.Percent_WeeklyCompletedActivities = ReadPercentWeeklyCompletedActivities(PlayerID).ToString("N0");

            return model;
        }
        public WeightPieChart ReadWeightPieChartData(long PlayerID)
        {
            var result = db.SelectPlayerCompleted_RemainingWeightPercentage_Dashboard(PlayerID).FirstOrDefault();

            WeightPieChart model = new WeightPieChart()
            {
                category = "Completed",
                color = "green",
                value = result.CompletedPercentAge.HasValue ? result.CompletedPercentAge.Value : 0.00M
            };

            return model;
        }

        public decimal ReadPercentWeeklyCompletedActivities(long PlayerID)
        {
            var result = db.SelectPlayerPercent_CompletedWeeklyActivities(PlayerID).FirstOrDefault();

            return result.HasValue ? result.Value : 0M;
        }

        public List<PlayerWeigtPerWeekChart> Read_PlayerWeigtPerWeekChart(long PlayerID)
        {
            List<PlayerWeigtPerWeekChart> ListOfChartData = new List<PlayerWeigtPerWeekChart>();

            return ListOfChartData;

            //As we have removed PlayerWeightWeeks now player need to enter their weight manually from Front end and that will be used for Chart

            /*  PlayerWeightWeeksRepository pwwRepo = new PlayerWeightWeeksRepository();
              var data = pwwRepo.ReadOne_ByPlayerID(PlayerID);


              if (data != null)
              {

                  {
                      PlayerWeigtPerWeekChart m = new PlayerWeigtPerWeekChart()
                      {
                          WeekNum = "Reg. Weight",
                          Weight = data.RegWeight
                      };

                      ListOfChartData.Add(m);
                  }

                  if (data.Wk1.HasValue)
                  {
                      PlayerWeigtPerWeekChart m = new PlayerWeigtPerWeekChart()
                      {
                          WeekNum = "Week 1",
                          Weight = data.Wk1.Value
                      };

                      ListOfChartData.Add(m);
                  }

                  if (data.Wk2.HasValue)
                  {
                      PlayerWeigtPerWeekChart m = new PlayerWeigtPerWeekChart()
                      {
                          WeekNum = "Week 2",
                          Weight = data.Wk2.Value
                      };

                      ListOfChartData.Add(m);
                  }
                  if (data.Wk3.HasValue)
                  {
                      PlayerWeigtPerWeekChart m = new PlayerWeigtPerWeekChart()
                      {
                          WeekNum = "Week 3",
                          Weight = data.Wk3.Value
                      };

                      ListOfChartData.Add(m);
                  }
                  if (data.Wk4.HasValue)
                  {
                      PlayerWeigtPerWeekChart m = new PlayerWeigtPerWeekChart()
                      {
                          WeekNum = "Week 4",
                          Weight = data.Wk4.Value
                      };

                      ListOfChartData.Add(m);
                  }
                  if (data.Wk5.HasValue)
                  {
                      PlayerWeigtPerWeekChart m = new PlayerWeigtPerWeekChart()
                      {
                          WeekNum = "Week 5",
                          Weight = data.Wk5.Value
                      };

                      ListOfChartData.Add(m);
                  }
                  if (data.Wk6.HasValue)
                  {
                      PlayerWeigtPerWeekChart m = new PlayerWeigtPerWeekChart()
                      {
                          WeekNum = "Week 6",
                          Weight = data.Wk6.Value
                      };

                      ListOfChartData.Add(m);
                  }
                  if (data.Wk7.HasValue)
                  {
                      PlayerWeigtPerWeekChart m = new PlayerWeigtPerWeekChart()
                      {
                          WeekNum = "Week 7",
                          Weight = data.Wk7.Value
                      };

                      ListOfChartData.Add(m);
                  }
                  if (data.Wk8.HasValue)
                  {
                      PlayerWeigtPerWeekChart m = new PlayerWeigtPerWeekChart()
                      {
                          WeekNum = "Week 8",
                          Weight = data.Wk8.Value
                      };

                      ListOfChartData.Add(m);
                  }
                  if (data.Wk9.HasValue)
                  {
                      PlayerWeigtPerWeekChart m = new PlayerWeigtPerWeekChart()
                      {
                          WeekNum = "Week 9",
                          Weight = data.Wk9.Value
                      };

                      ListOfChartData.Add(m);
                  }
                  if (data.Wk10.HasValue)
                  {
                      PlayerWeigtPerWeekChart m = new PlayerWeigtPerWeekChart()
                      {
                          WeekNum = "Week 10",
                          Weight = data.Wk10.Value
                      };

                      ListOfChartData.Add(m);
                  }
                  if (data.Wk11.HasValue)
                  {
                      PlayerWeigtPerWeekChart m = new PlayerWeigtPerWeekChart()
                      {
                          WeekNum = "Week 11",
                          Weight = data.Wk11.Value
                      };

                      ListOfChartData.Add(m);
                  }
                  if (data.Wk12.HasValue)
                  {
                      PlayerWeigtPerWeekChart m = new PlayerWeigtPerWeekChart()
                      {
                          WeekNum = "Week 12",
                          Weight = data.Wk12.Value
                      };

                      ListOfChartData.Add(m);
                  }
                  if (data.Wk13.HasValue)
                  {
                      PlayerWeigtPerWeekChart m = new PlayerWeigtPerWeekChart()
                      {
                          WeekNum = "Week 13",
                          Weight = data.Wk13.Value
                      };

                      ListOfChartData.Add(m);
                  }
                  if (data.Wk14.HasValue)
                  {
                      PlayerWeigtPerWeekChart m = new PlayerWeigtPerWeekChart()
                      {
                          WeekNum = "Week 14",
                          Weight = data.Wk14.Value
                      };

                      ListOfChartData.Add(m);
                  }
              }

              return ListOfChartData;*/
        }
    }

    public class ProgressChart
    {
        public string DashboardURL { get; set; }
        public long PlayerID { get; set; }
        public string Percent_CompletedWeightTarget { get; set; }
        public string Percent_WeeklyCompletedActivities { get; set; }
    }

    public class WeightPieChart
    {
        public string category { get; set; }
        public string color { get; set; }
        public decimal value { get; set; }
    }

    public class PlayerWeigtPerWeekChart
    {
        public string WeekNum { get; set; }
        public decimal Weight { get; set; }
    }
}