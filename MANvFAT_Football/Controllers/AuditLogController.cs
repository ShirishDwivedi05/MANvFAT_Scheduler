using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models;
using MANvFAT_Football.Models.Enumerations;
using MANvFAT_Football.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MANvFAT_Football.Controllers
{
    [Authorization(Permissions.Administrator)]
    public class AuditLogController : BaseController
    {
        // GET: AuditLog
        #region Index & Audit Log Search Functionality

        public ActionResult Index(string secret)
        {
            if (secret == "M@nvF@t@123")
            {
                AuditLogSearch au = new AuditLogSearch();
                if (Session["AuditLogSearch"] != null)
                    au = (AuditLogSearch)Session["AuditLogSearch"];


                ViewData["dateFrom"] = au.StxtDateFrom;
                ViewData["dateTo"] = au.StxtDateTo;
                ViewData["action"] = au.StxtAction;

                ViewBag.Users = GetUsersSelectList();

                return View(GetListOfAuditLog(au));
            }
            else
            {
                return RedirectToAction("Index", "Admin");
            }
        }

        public List<AuditLogsExt> GetListOfAuditLog(AuditLogSearch au)
        {
            AuditLogRepository modelRepo = new AuditLogRepository();
            var logs = modelRepo.searchResult(au.StxtDateFrom, au.StxtDateTo, au.StxtAction, au.SUserID).ToList();
            return logs;
        }


        [HttpPost]
        public ActionResult Index(FormCollection formcollection)
        {

            AuditLogSearch au = new AuditLogSearch();
            if (Session["AuditLogSearch"] != null)
                au = (AuditLogSearch)Session["AuditLogSearch"];


            string txtDateFrom = formcollection["txtDateFrom"].ToString();
            string txtDateTo = formcollection["txtDateTo"].ToString();
            string txtAction = formcollection["txtAction"].ToString();
            string userID = formcollection["UserID"].ToString();

            ViewData["dateFrom"] = au.StxtDateFrom = txtDateFrom;
            ViewData["dateTo"] = au.StxtDateTo = txtDateTo;
            ViewData["action"] = au.StxtAction = txtAction;
            au.SUserID = userID;

            Session["AuditLogSearch"] = au;

            ViewBag.Users = GetUsersSelectList();

            return RedirectToAction("Index");

        }

        public ActionResult ResetForm()
        {
            ViewData.Remove("dateFrom");
            ViewData.Remove("dateTo");
            ViewData.Remove("action");

            Session.Remove("AuditLogSearch");


            return RedirectToAction("Index");
        }

        public SelectList GetUsersSelectList()
        {
            AuditLogSearch au = new AuditLogSearch();
            if (Session["AuditLogSearch"] != null)
                au = (AuditLogSearch)Session["AuditLogSearch"];

            List<SelectListItem> listUsers = new List<SelectListItem>();
            {

                UsersRepository modelRepo = new UsersRepository();


                foreach (var item in modelRepo.ReadAll())
                {
                    SelectListItem itr = new SelectListItem();
                    itr.Text = item.FullName;
                    itr.Value = item.UserID.ToString();
                    itr.Selected = false;

                    listUsers.Add(itr);
                }
            }
            return new SelectList(listUsers, "Value", "Text", au.SUserID);
        }

        #endregion

        public ActionResult GenerateAuditLogDetails(long AuditLogID)
        {
            AuditLogRepository modelRepo = new AuditLogRepository();
            var auditLog = modelRepo.ReadOne(AuditLogID);
            string InnerBody = SecurityUtils.RenderPartialToString(this, "GenerateAuditLogDetails", auditLog, ViewData, TempData);

            return new JsonResult { Data = InnerBody };
        }
    }
}