using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Enumerations;
using MANvFAT_Football.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Web.Mvc;

namespace MANvFAT_Football.Controllers
{

    public class SystemSettingsController : BaseController
    {
        #region System Setttings
        // GET: SystemSettings
        [Authorization(Permissions.Administrator)]
        [HttpGet]
        public ActionResult Index()
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sysSetting = sysRepo.GetSystemSettings();
            sysSetting.PaymentModes = DropDownLists.GetPaymentModes(sysSetting.GoCardless_Mode);
            ViewBag.Msg = "";
            ViewBag.MsgCss = "";
            return View(sysSetting);
        }
        [Authorization(Permissions.Administrator)]
        [HttpPost]
        public ActionResult Index(SystemSettingsExt model)
        {
            string _Msg = "", _MsgCss = "";
            if (ModelState.IsValid)
            {
                SystemSettingsRepository sysRepo = new SystemSettingsRepository();
                sysRepo.Update(model, ref _Msg, ref _MsgCss, this);
                ViewBag.Msg = _Msg;
                ViewBag.MsgCss = _MsgCss;
            }

            model.PaymentModes = DropDownLists.GetPaymentModes(model.GoCardless_Mode);

            return View(model);
        }

        public ActionResult GetCurrentDomain()
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var sys = sysRepo.GetSystemSettings();

            return new JsonResult { Data = sys.CurrentDomain };
        }

        #endregion

        #region System Message

        [Authorization(Permissions.Administrator)]
        [HttpGet]
        public ActionResult SystemMessage()
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            SystemMessages sysMsg = new SystemMessages();
            sysMsg = sysRepo.ReadSystemMessage();
            ViewBag.Msg = "";
            ViewBag.MsgCss = "";
            return View(sysMsg);
        }
        [Authorization(Permissions.Administrator)]
        [HttpPost]
        public ActionResult SystemMessage(SystemMessages SysMsg)
        {
            string _Msg = "", _MsgCss = "";
            if (ModelState.IsValid)
            {
                SystemSettingsRepository sysRepo = new SystemSettingsRepository();
                sysRepo.UpdateSystemMessage(SysMsg.SystemMessage, SysMsg.SystemMessageSubject, ref _Msg, ref _MsgCss, this);
                ViewBag.Msg = _Msg;
                ViewBag.MsgCss = _MsgCss;

                //  SysMsg.SystemMessage = Server.HtmlEncode(SysMsg.SystemMessage);
            }
            else
            {
                ViewBag.Msg = "Please enter Subject and Message to Continue.";
                ViewBag.MsgCss = "alert alert-danger";
            }


            return View(SysMsg);
        }

        [Authorization(Permissions.Administrator)]
        [HttpGet]
        public ActionResult HistoricalSystemMessages()
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var model = sysRepo.ReadHistoricalSystemMessage();
            ViewBag.Msg = "";
            ViewBag.MsgCss = "";
            return View(model);
        }

        #endregion
    }
}