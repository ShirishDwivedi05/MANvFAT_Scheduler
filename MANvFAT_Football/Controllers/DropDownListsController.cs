using MANvFAT_Football.Models;
using MANvFAT_Football.Models.Enumerations;
using MANvFAT_Football.Models.Repositories;
using System.Collections.Generic;
using System.Linq;
using System.Web.Mvc;

namespace MANvFAT_Football.Controllers
{
    public class DropDownListsController : Controller
    {
        // GET: DropDownLists
        private DBEntities db = new DBEntities();

        //Following Drop Downs can be Accessed be Controls on the View via Ajax Call and Return Data in json Format

        #region DropDowns

        public JsonResult GetRoles()
        {
            var data = db.Roles.ToList();
            return Json(data.OrderBy(o => o.RoleID).Select(c => new { RoleID = c.RoleID, Role = c.Role }).OrderBy(o => o.Role), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetCoachUsers()
        {
            UsersRepository modelRepo = new UsersRepository();
            var data = modelRepo.ReadAll().Where(m => m.RoleID == (int)Permissions.Coaches);
            return Json(data.Select(c => new { CoachID = c.UserID, FullName = c.FullName }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetAdminUsers()
        {
            UsersRepository modelRepo = new UsersRepository();
            var data = modelRepo.ReadAll().Where(m => m.RoleID == (int)Permissions.Administrator);
            return Json(data.Select(c => new { AdminUserID = c.UserID, FullName = c.FullName }), JsonRequestBehavior.AllowGet);
        }

        public JsonResult GetHeights()
        {
            PlayersRepository modelRepo = new PlayersRepository();
            var data = modelRepo.ReadAll_Heights();
            return Json(data.Select(c => new { HeightID = c.HeightID, Height_Display = c.Height_Display }), JsonRequestBehavior.AllowGet);
        }


       
        public ActionResult GetActivityShareFrequency(int ActivityTypeID)
        {

            List<SelectListItem> ListOfObejcts = new List<SelectListItem>();
            if (ActivityTypeID == 1)
            {
                {
                    SelectListItem item = new SelectListItem()
                    {
                        Text = "Today",
                        Value = "1"
                    };

                    ListOfObejcts.Add(item);
                }

                {
                    SelectListItem item = new SelectListItem()
                    {
                        Text = "Last 7 Days",
                        Value = "2"
                    };

                    ListOfObejcts.Add(item);
                }

                {
                    SelectListItem item = new SelectListItem()
                    {
                        Text = "Last Month",
                        Value = "3"
                    };

                    ListOfObejcts.Add(item);
                }

                {
                    SelectListItem item = new SelectListItem()
                    {
                        Text = "Date Range",
                        Value = "4"
                    };

                    ListOfObejcts.Add(item);
                }
            }
            else if (ActivityTypeID==2)
            {
                {
                    SelectListItem item = new SelectListItem()
                    {
                        Text = "Current Week",
                        Value = "1"
                    };

                    ListOfObejcts.Add(item);
                }

                {
                    SelectListItem item = new SelectListItem()
                    {
                        Text = "Last Week",
                        Value = "2"
                    };

                    ListOfObejcts.Add(item);
                }

                {
                    SelectListItem item = new SelectListItem()
                    {
                        Text = "Last Month",
                        Value = "3"
                    };

                    ListOfObejcts.Add(item);
                }

                {
                    SelectListItem item = new SelectListItem()
                    {
                        Text = "Date Range",
                        Value = "4"
                    };

                    ListOfObejcts.Add(item);
                }
            }
            else
            {
                {
                    {
                        SelectListItem item = new SelectListItem()
                        {
                            Text = "Today",
                            Value = "1"
                        };

                        ListOfObejcts.Add(item);
                    }

                    {
                        SelectListItem item = new SelectListItem()
                        {
                            Text = "Last 7 Days",
                            Value = "2"
                        };

                        ListOfObejcts.Add(item);
                    }

                    {
                        SelectListItem item = new SelectListItem()
                        {
                            Text = "Last Month",
                            Value = "3"
                        };

                        ListOfObejcts.Add(item);
                    }

                    {
                        SelectListItem item = new SelectListItem()
                        {
                            Text = "Date Range",
                            Value = "4"
                        };

                        ListOfObejcts.Add(item);
                    }
                }
            }

            SelectList result = new SelectList(ListOfObejcts, "Value", "Text");

            return Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetWeeklyActivityTime(int? ParamActivityTime)
        {
            PlayerWeeklyActivityRepository modelRepo = new PlayerWeeklyActivityRepository();
            List<SelectListItem> ListOfObejcts = new List<SelectListItem>();

            for (int i = 15; i <= 180; i=i+15)
            {
                
                SelectListItem item = new SelectListItem()
                {
                    Text = modelRepo.ActivityMinutesToString(i),
                    Value = i.ToString(),
                    Selected = ParamActivityTime == i
                };

                ListOfObejcts.Add(item);
            }
               
            SelectList result = new SelectList(ListOfObejcts, "Value", "Text");

            return Json(result, JsonRequestBehavior.AllowGet);
        }
        #endregion DropDowns
    }
}