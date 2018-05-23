using MANvFAT_Football.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MANvFAT_Football.Controllers
{
    public class ChartsController : Controller
    {


        public JsonResult GetInchesData(string period)
        {
            List<InchesViewModel> model = InchesViewModel.GetInchesData(period);
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWeeklyActivityData(string period)
        {
            List<WeeklyActivityViewModel> model = WeeklyActivityViewModel.GetWeeklyActivity(period);
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetWeightData(string period)
        {
            List<WeightViewModel> model = WeightViewModel.GetWeightData(period);
            return Json(new { data = model }, JsonRequestBehavior.AllowGet);
        }
    }
}