using Kendo.Mvc.UI;
using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Enumerations;
using MANvFAT_Football.Models.Repositories;
using NPOI.XSSF.UserModel;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Web.Mvc;

namespace MANvFAT_Football.Controllers
{
    [Authorization(Permissions.Administrator, Permissions.Coaches)]
    public class PlayersController : BaseController
    {
        #region Players Maintenance

        // GET: Players
        public ActionResult Index()
        {
            return View();
        }

        //id = PlayerID
        [HttpGet]
        public ActionResult Details(long id)
        {
            if (id != 0)
            {
                //Already Exist
                PlayersRepository modelRepo = new PlayersRepository();
                var model = modelRepo.ReadOne(id);

                //Create Player Premium Dashboard view bag only if Admin user is Logged in
                PlayerDashboardRepository pdRepo = new PlayerDashboardRepository();
                ViewBag.PlayerPremiumDashboard = pdRepo.ReadOne(model.PlayerID);

                if (Request.QueryString["m"] != null)
                {
                    List<MessagesExt> ListOfMsgs = new List<MessagesExt>();

                    new MessagesRepository().BuildMessageList(SecurityUtils.DecryptCypher(Request.QueryString["m"].ToString()), TypeOfMessage.Success, ref ListOfMsgs);

                    model.ListOfMsgs.AddRange(ListOfMsgs);
                }

                BindViewBags(ref model);
                return View(model);
            }
            else
            {
                //For New Customer Record
                var model = new PlayersExt();
                model.PlayerID = 0;
                BindViewBags(ref model);
                return View(model);
            }
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public ActionResult Details(PlayersExt model)
        {
            List<MessagesExt> ListOfMsgs = new List<MessagesExt>();

            string Msg = "";
            bool status = true;
            ModelState.Remove("id");
            ModelState.Remove("PlayerTeamID");

            if (ModelState.IsValid)
            {
                try
                {
                    PlayersRepository modelRepo = new PlayersRepository();

                    if (model.PlayerID == 0)
                    {
                        var BMI = modelRepo.CalculateBMI(model.Weight, model.HeightID);
                        model.RegBMI = BMI;
                    }

                    model.PlayerID = modelRepo.CreateOrUpdate(ref model, ref Msg, ref status, this);
                    if (status)
                    {
                        if (model.IsApply)
                        {
                            System.Web.Routing.RouteValueDictionary rt = new System.Web.Routing.RouteValueDictionary();
                            rt.Add("id", model.PlayerID);
                            rt.Add("m", SecurityUtils.EncryptText(Msg));

                            return RedirectToAction("Details", "Players", rt);
                        }
                        else
                        {
                            return RedirectToAction("Index");
                        }
                    }
                    else
                    {
                        ModelState.AddModelError("", Msg);
                        //BindViewBags(ref model);
                        //return View(model);
                    }
                }
                catch (Exception ex)
                {
                    string error = ErrorHandling.HandleException(ex);
                    ModelState.AddModelError("", error);
                    //BindViewBags(ref model);
                    //return View(model);
                }
            }

            {
                //Generate list of Error Messaged extracted from ModelState
                foreach (ModelState modelState in ViewData.ModelState.Values)
                {
                    foreach (ModelError error in modelState.Errors)
                    {
                        new MessagesRepository().BuildMessageList(error.ErrorMessage, TypeOfMessage.Error, ref ListOfMsgs);
                    }
                }

                model.ListOfMsgs.AddRange(ListOfMsgs);

                BindViewBags(ref model);

                return View(model);
            }
        }

        public void BindViewBags(ref PlayersExt model)
        {
            //SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            //var sys = sysRepo.GetSystemSettings();

            //string _EmergencyContactPageLink = sys.CurrentDomain + "/Home/EmergencyContact?p=" + EncryptedEmail;

            //model.EmergencyContactPageLink = _EmergencyContactPageLink;
        }

        #region Grid Create, Read, Update, Delete Functions

        public ActionResult _Read([DataSourceRequest]DataSourceRequest request)
        {
            long? CoachUserID = null;
            //Check if Coach Logged in then coach can only see leagues where he is coach for.
            var LoggedInUser = SecurityUtils.GetUserDetails();

            if (LoggedInUser.RoleID == (long)Permissions.Coaches)
            {
                CoachUserID = LoggedInUser.UserID;
            }

            PlayersRepository modelRepo = new PlayersRepository();
            DataSourceResult result = null;// modelRepo.ReadAll_ForGrid(request,CoachUserID);
            return Json(result);
        }

        public ActionResult DeletePlayer(long PlayerID)
        {
            string Msg = "";
            bool status = true;
            PlayersRepository modelRepo = new PlayersRepository();
            status = modelRepo.Delete_Archive(PlayerID, ref Msg, this);

            return new JsonResult { Data = new { status = status, Msg = Msg } };
        }

        #endregion Grid Create, Read, Update, Delete Functions

        #region Export to Excel

        public ActionResult ExportToExcel()
        {
            PlayersRepository modelRepo = new PlayersRepository();
            var players = modelRepo.ReadAll();

            Dictionary<string, int> SheetColumns = new Dictionary<string, int>();

            int ColumnNumber = 0;
            //Create new Excel workbook
            var workbook = new XSSFWorkbook();

            //Create new Excel sheet
            var sheet = workbook.CreateSheet("MVFF Players");

            //Create a header row
            var headerRow = sheet.CreateRow(0);

            SheetColumns.Add("First Name", 15);
            SheetColumns.Add("Last Name", 15);
            SheetColumns.Add("Email Address", 30);
            SheetColumns.Add("DOB", 15);
            SheetColumns.Add("Reg. Date", 15);
            SheetColumns.Add("League", 25);
            SheetColumns.Add("Weight", 15);
            SheetColumns.Add("Height", 18);
            SheetColumns.Add("How active?", 40);
            SheetColumns.Add("BMI", 15);
            SheetColumns.Add("Body Fat (%)", 15);
            SheetColumns.Add("Active", 15);
            SheetColumns.Add("Work Phone", 20);
            SheetColumns.Add("Landline", 15);
            SheetColumns.Add("Mobile", 15);
            //SheetColumns.Add("Address Line 1", 20);
            //SheetColumns.Add("Address Line 2", 20);
            //SheetColumns.Add("Country", 15);
            //SheetColumns.Add("City", 15);
            SheetColumns.Add("Post Code", 15);
            SheetColumns.Add("Notes", 30);
            SheetColumns.Add("How Hear about MANvFAT?", 80);

            CreateExcelFile_NPOI.AddHeaderColumns(ref SheetColumns, ref ColumnNumber, ref sheet, ref headerRow);

            ////Create Header Font BOLD
            headerRow.GetCell(0).CellStyle =
            headerRow.GetCell(1).CellStyle =
            headerRow.GetCell(2).CellStyle =
            headerRow.GetCell(3).CellStyle =
            headerRow.GetCell(4).CellStyle =
            headerRow.GetCell(5).CellStyle =
            headerRow.GetCell(6).CellStyle =
            headerRow.GetCell(7).CellStyle =
            headerRow.GetCell(8).CellStyle =
            headerRow.GetCell(9).CellStyle =
            headerRow.GetCell(10).CellStyle =
            headerRow.GetCell(11).CellStyle =
            headerRow.GetCell(12).CellStyle =
            headerRow.GetCell(13).CellStyle =
            headerRow.GetCell(14).CellStyle =
            headerRow.GetCell(15).CellStyle =
            headerRow.GetCell(16).CellStyle =
            headerRow.GetCell(17).CellStyle = CreateExcelFile_NPOI.CellStyle(ref workbook, true, true, false, null, null, null, null, null, null);

            //Fix the Header Row
            CreateExcelFile_NPOI.FixHeader(ref sheet);

            var Currency_Format = CreateExcelFile_NPOI.SetCellStyle_Currency(ref workbook, "£", 2);
            var Number_Format = CreateExcelFile_NPOI.SetCellStyle_Currency(ref workbook, "", 2);
            var DateTime_Format = CreateExcelFile_NPOI.SetCellStyle_DateTime(ref workbook, true);
            var DateOnly_Format = CreateExcelFile_NPOI.SetCellStyle_DateOnly(ref workbook);

            NumberFormatInfo provider = new NumberFormatInfo();
            provider.NumberDecimalSeparator = ".";
            provider.NumberGroupSeparator = ",";
            provider.NumberDecimalDigits = 2;
            provider.NumberGroupSizes = new int[] { 2 };

            int rowNumber = 1;

            //Populate the sheet with values from the grid data
            foreach (PlayersExt p in players)
            {
                var row = sheet.CreateRow(rowNumber++);

                row.CreateCell(1).SetCellValue(p.FirstName);
                row.CreateCell(2).SetCellValue(p.LastName);
                row.CreateCell(3).SetCellValue(p.EmailAddress);
                row.CreateCell(4).SetCellValue(p.DOB.HasValue ? p.DOB.Value.ToString("dd/MM/yyyy") : "");
                row.CreateCell(5).SetCellValue(p.RegistrationDate.ToString("dd/MM/yyyy"));
                row.CreateCell(6).SetCellValue(Convert.ToDouble(p.Weight, provider));
                row.CreateCell(7).SetCellValue(p.Height);
                row.CreateCell(8).SetCellValue("TODO");
                row.CreateCell(9).SetCellValue(Convert.ToDouble(p.BMI, provider));
                row.CreateCell(10).SetCellValue(Convert.ToDouble(p.BodyFat, provider));
                row.CreateCell(11).SetCellValue((p.Active ? "Yes" : "No"));
                row.CreateCell(12).SetCellValue((p.Featured ? "Yes" : "No"));
                row.CreateCell(13).SetCellValue(p.WorkPhone);
                row.CreateCell(14).SetCellValue(p.Landline);
                row.CreateCell(15).SetCellValue(p.Mobile);
                //row.CreateCell(18).SetCellValue(p.AddressLine1);
                //row.CreateCell(19).SetCellValue(p.AddressLine2);
                //row.CreateCell(20).SetCellValue(p.Country);
                //row.CreateCell(21).SetCellValue(p.City);
                row.CreateCell(16).SetCellValue(p.PostCode);
                row.CreateCell(17).SetCellValue(p.Notes);

                string HowHeardAboutMANvFAT = p.AdvertisementOther ? p.AdvertisementOtherDetails : p.Advertisement;
                row.CreateCell(24).SetCellValue(HowHeardAboutMANvFAT);

                row.GetCell(4).CellStyle = DateOnly_Format;
                row.GetCell(5).CellStyle = DateOnly_Format;

                row.GetCell(6).CellStyle =
                row.GetCell(7).CellStyle =
                row.GetCell(11).CellStyle =
                row.GetCell(12).CellStyle = Number_Format;

                row.GetCell(23).CellStyle = CreateExcelFile_NPOI.CellStyle(ref workbook, false, true, false, null, null, null, null, null, null);
            }

            MemoryStream output = new MemoryStream();
            workbook.Write(output);

            //Return the result to the end user
            return File(output.ToArray(),   //The binary data of the XLS file
                "application/vnd.ms-excel", //MIME type of Excel files
                "Players(" + DateTime.Now.ToString("dd-MM-yyyy_HHmm") + ").xlsx");     //Suggested file name in the "Save as" dialogue which will be displayed to the end user
        }

        #endregion Export to Excel

        #endregion Players Maintenance
    }
}