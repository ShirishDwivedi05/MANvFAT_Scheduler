using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models;
using MANvFAT_Football.Models.Enumerations;
using MANvFAT_Football.Models.Repositories;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading;
using System.Web;
using System.Web.Mvc;

namespace MANvFAT_Football.Controllers
{
    public partial class PlayerImagesController : Controller
    {
        private Thread AchievementThread;

        public ActionResult ReadFirstImages(long ParamPlayerID)
        {
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            var result = modelRepo.ReadAll(ParamPlayerID, false, true).ToList();
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }

        public ActionResult ReadSecondImages(long ParamPlayerID, long ParamImageID)
        {
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            var data = modelRepo.ReadAll(ParamPlayerID, false, true).Where(m => m.PlayerImageID != ParamImageID).ToList();
            return this.Json(data, JsonRequestBehavior.AllowGet);
        }

        // GET: PlayerImages
        public ActionResult Index()
        {
            return View();
        }

        #region Player Image Get, Upload, Remove, Make Default, Make Display Image Functions

        public ActionResult GetPlayerImagesAsync(long id)
        {
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            return this.Json(modelRepo.ReadAll(id,false,true), JsonRequestBehavior.AllowGet);
        }
        
        //id = PlayerID
        public ActionResult GetPlayerImages(long id, bool? Anim, bool All)
        {
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            return this.Json(modelRepo.ReadAll(id, Anim, All), JsonRequestBehavior.AllowGet);
        }

        public ActionResult GetPlayerGIFImages(long id, bool All)
        {
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            return this.Json(modelRepo.ReadAll(id, true, All), JsonRequestBehavior.AllowGet);
        }

        public ActionResult DownloadImage(long id)
        {
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            var playerimg = modelRepo.ReadOne(id);

            var filename = DateTime.Now.ToString("ddMMyyyy_HHmmss") + ".gif";

            var imgPath = Server.MapPath(playerimg.ImageLink);
            Response.AddHeader("Content-Disposition", "attachment;filename=\"" + filename + "\"");
            Response.WriteFile(imgPath);
            Response.End();
            return null;
        }

        public string ValidatePlayer_ForProgressImage(PlayersExt player, List<PlayerImagesExt> ListOfPlayerImages, long LeagueID, bool IsFront, bool IsSide)
        {
            DateTime TodayDate = DateTime.Now.Date.AddDays(2);

            string Msg = "";

            if (player == null)
            {
                Msg = "Your Email address is not valid OR it is not registered with MANvFAT Football";
            }
            else if (player.Active == false)
            {
                Msg = "Unable to upload your Photos, because your status is not Active in MANvFAT System";
            }
            else
            {
                if (ListOfPlayerImages.Count > 0)
                {
                    //if(ListOfPlayerImages.Any(m=> (m.UploadDateTime >= Last7Days && m.UploadDateTime <= TodayDate) && m.IsFront == IsFront))
                    if (ListOfPlayerImages.Any(m => (m.NextPhotoUploadDate.Date > TodayDate && m.UploadDateTime.Date <= TodayDate) && m.IsFront == IsFront && m.IsSide == false))
                    {
                        Msg = "Unable to upload your Photos, because you have already uploaded Front Image for this week";
                    }
                    else if (ListOfPlayerImages.Any(m => (m.NextPhotoUploadDate.Date > TodayDate && m.UploadDateTime.Date <= TodayDate) && m.IsSide == IsSide && m.IsFront == false))
                    {
                        Msg = "Unable to upload your Photos, because you have already uploaded Side Image for this week";
                    }
                }
            }

            return Msg;
        }

        /// <summary>
        /// Save Player Progress Image
        /// </summary>
        /// <param name="PlayerProgressImages">List of Uploaded Images</param>
        /// <param name="PlayerEmailAddress">Player Email Address to match and save against Player record in DB</param>
        /// <param name="LeagueID">League ID from current League Home Page</param>
        /// <param name="IsFront">IsFront</param>
        /// <param name="IsSide">IsSide</param>
        /// <returns></returns>
        public ActionResult SavePlayerProgressImages(IEnumerable<HttpPostedFileBase> PlayerProgressImages_Front, IEnumerable<HttpPostedFileBase> PlayerProgressImages_Side,
                                                    string PlayerEmailAddress, long LeagueID, bool IsFront, bool IsSide)
        {
            IEnumerable<HttpPostedFileBase> PlayerProgressImages = null;
            List<PlayerImagesExt> ListOfPlayerImages = new List<PlayerImagesExt>();
            if (PlayerProgressImages_Front != null)
            {
                PlayerProgressImages = PlayerProgressImages_Front;
            }
            else if (PlayerProgressImages_Side != null)
            {
                PlayerProgressImages = PlayerProgressImages_Side;
            }

            PlayersRepository playerRepo = new PlayersRepository();
            var player = playerRepo.ReadOne_ByEmailAddress(PlayerEmailAddress);

            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            if (player != null)
            {
                ListOfPlayerImages = modelRepo.ReadAll(player.PlayerID, false, false);
            }

            var ErrorMsg = ValidatePlayer_ForProgressImage(player, ListOfPlayerImages, LeagueID, IsFront, IsSide);
            if (!string.IsNullOrEmpty(ErrorMsg))
            {
                return Content(ErrorMsg);
            }
            else
            {
                var ParamPlayerID = player.PlayerID;

                string FinalFileName = "";
                // The Name of the Upload component is "files"
                if (PlayerProgressImages != null)
                {
                    string PlayerImagesFolder = Path.Combine(Server.MapPath("~" + SecurityUtils.Players_ImagePath + ""), ParamPlayerID.ToString());

                    foreach (var file in PlayerProgressImages)
                    {
                        if (!(System.IO.Directory.Exists(PlayerImagesFolder)))
                        {
                            System.IO.Directory.CreateDirectory(PlayerImagesFolder);
                        }

                        // Some browsers send file names with full path. This needs to be stripped.
                        string IEFileName = "", FileNameWithExtension = "";

                        if (Request.Browser.Browser.ToLower().Equals("ie") || Request.Browser.Browser.ToLower().Equals("internetexplorer"))
                        {
                            IEFileName = Path.GetFileName(file.FileName);
                        }

                        if (IEFileName != "")
                        {
                            FileNameWithExtension = IEFileName;
                            SecurityUtils.CheckforInvalidFileNameChar(ref FileNameWithExtension);
                        }
                        else
                        {
                            FileNameWithExtension = file.FileName;
                            SecurityUtils.CheckforInvalidFileNameChar(ref FileNameWithExtension);
                        }

                        var fileExtension = Path.GetExtension(FileNameWithExtension);

                        //Following Code will rename the file if it is already Exists
                        int count = 1;

                        string fileNameOnly = Guid.NewGuid().ToString(); //Path.GetFileNameWithoutExtension(FileNameWithExtension);
                        string extension = Path.GetExtension(FileNameWithExtension);
                        string newFullPath = FileNameWithExtension;
                        newFullPath = Path.Combine(PlayerImagesFolder, fileNameOnly + extension);

                        while (System.IO.File.Exists(newFullPath))
                        {
                            string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                            newFullPath = Path.Combine(PlayerImagesFolder, tempFileName + extension);
                        }

                        fileNameOnly = Path.GetFileNameWithoutExtension(newFullPath);

                        //Update Filename from Repository
                        string Msg = "";
                        bool status = true;
                        FinalFileName = fileNameOnly + extension;

                        //Save the file to the Disk
                        file.SaveAs(newFullPath);

                        // ImageWaterMark imgWaterMark = new ImageWaterMark();

                        string PlayerImagePath = Path.Combine(PlayerImagesFolder, FinalFileName);

                        string WaterMarkImagePath = Server.MapPath("~/Images/verticle_mvff.png");

                        Image_Combine imgCombine = new Image_Combine();

                        string FinalFileName_WaterMarked = imgCombine.CombineBitmap_WatermarkImage(PlayerImagesFolder, FinalFileName, WaterMarkImagePath);
                        //imgWaterMark.GenerateWaterMarkOnImage(PlayerImagesFolder, FinalFileName, false, true, WaterMarkImagePath, null, false, false, true, false, false);

                        //Clear the Object
                        imgCombine = null;

                        PlayerImagesExt PlayerImg = new PlayerImagesExt()
                        {
                            PlayerID = ParamPlayerID,
                            FileName = FinalFileName_WaterMarked,
                            IsAnimated = false,
                            Display = true,
                            DefaultImage = false,
                            IsFront = IsFront,
                            IsSide = IsSide,
                            UploadDateTime = DateTime.Now
                        };

                        modelRepo.CreateOrUpdate(ref PlayerImg, ref Msg, ref status, this);

                        string NonWaterMarkImagePath = Path.Combine(PlayerImagesFolder, FinalFileName);

                        if (System.IO.File.Exists(NonWaterMarkImagePath))
                        {
                            System.IO.File.Delete(NonWaterMarkImagePath);
                        }
                    }
                }

                // Return an empty string to signify success
                return Content("");
            }
        }
        public ActionResult SaveDropzoneJsUploadedFiles()
        {
            foreach (string fileName in Request.Files)
            {
                HttpPostedFileBase file = Request.Files[fileName];
                string fName = file.FileName;
                if (file != null && file.ContentLength > 0)
                {

                    SaveFiles(file, 4);

                }
            }

            return Json(new { Message = string.Empty });
        }

        private bool SaveFiles(HttpPostedFileBase PlayerImages, long ParamPlayerID = 0)
        {
            try
            {
                string FinalFileName = "";
                // The Name of the Upload component is "files"
                if (PlayerImages != null)
                {
                    PlayerImagesRepository modelRepo = new PlayerImagesRepository();

                    string PlayerImagesFolder = Path.Combine(Server.MapPath("~" + SecurityUtils.Players_ImagePath + ""), ParamPlayerID.ToString());

                    var file = PlayerImages;

                    if (!(System.IO.Directory.Exists(PlayerImagesFolder)))
                    {
                        System.IO.Directory.CreateDirectory(PlayerImagesFolder);
                    }

                    // Some browsers send file names with full path. This needs to be stripped.
                    string IEFileName = "", FileNameWithExtension = "";

                    if (Request.Browser.Browser.ToLower().Equals("ie") || Request.Browser.Browser.ToLower().Equals("internetexplorer"))
                    {
                        IEFileName = Path.GetFileName(file.FileName);
                    }

                    if (IEFileName != "")
                    {
                        FileNameWithExtension = IEFileName;
                        SecurityUtils.CheckforInvalidFileNameChar(ref FileNameWithExtension);
                    }
                    else
                    {
                        FileNameWithExtension = file.FileName;
                        SecurityUtils.CheckforInvalidFileNameChar(ref FileNameWithExtension);
                    }

                    var fileExtension = Path.GetExtension(FileNameWithExtension);

                    //Following Code will rename the file if it is already Exists
                    int count = 1;

                    string fileNameOnly = Guid.NewGuid().ToString(); //Path.GetFileNameWithoutExtension(FileNameWithExtension);
                    string extension = Path.GetExtension(FileNameWithExtension);
                    string newFullPath = FileNameWithExtension;
                    newFullPath = Path.Combine(PlayerImagesFolder, fileNameOnly + extension);

                    while (System.IO.File.Exists(newFullPath))
                    {
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                        newFullPath = Path.Combine(PlayerImagesFolder, tempFileName + extension);
                    }

                    fileNameOnly = Path.GetFileNameWithoutExtension(newFullPath);

                    //Update Filename from Repository
                    string Msg = "";
                    bool status = true;
                    FinalFileName = fileNameOnly + extension;

                    //Save the file to the Disk
                    file.SaveAs(newFullPath);

                    string PlayerImagePath = Path.Combine(PlayerImagesFolder, FinalFileName);

                    string WaterMarkImagePath = Server.MapPath("~/Images/verticle_mvff.png");

                    Image_Combine imgCombine = new Image_Combine();

                    string FinalFileName_WaterMarked = imgCombine.CombineBitmap_WatermarkImage(PlayerImagesFolder, FinalFileName, WaterMarkImagePath);

                    //Clear the Object
                    imgCombine = null;

                    PlayerImagesExt PlayerImg = new PlayerImagesExt()
                    {
                        PlayerID = ParamPlayerID,
                        FileName = FinalFileName_WaterMarked,
                        IsAnimated = false,
                        Display = true,
                        DefaultImage = false,
                        UploadDateTime = DateTime.Now
                    };

                    modelRepo.CreateOrUpdate(ref PlayerImg, ref Msg, ref status, this);

                    string NonWaterMarkImagePath = Path.Combine(PlayerImagesFolder, FinalFileName);

                    if (System.IO.File.Exists(NonWaterMarkImagePath))
                    {
                        System.IO.File.Delete(NonWaterMarkImagePath);
                    }

                    //if (status && SecurityUtils.Enable_PremiumDashboard)
                    //{
                    //    AchievementsRepository achRepo = new AchievementsRepository();

                    //    Guid guid = Guid.NewGuid();
                    //    AchievementThread = new Thread(() => achRepo.ImageGallery_AchievementPoints(ParamPlayerID, AchievementThread));
                    //    AchievementThread.Name = "ImageGallery_BeforeAfterImageCreated_AchievementPoints_" + guid.ToString();
                    //    AchievementThread.Start();
                    //}

                }
                return true;
            }
            catch(Exception ex)
            {
                return false;
            }
        }
        ////public ActionResult Async_Save(IEnumerable<HttpPostedFileBase> files)
        ////{
        ////    // The Name of the Upload component is "files"
        ////    if (files != null)
        ////    {
        ////        foreach (var file in files)
        ////        {
        ////            // Some browsers send file names with full path.
        ////            // We are only interested in the file name.
        ////            var fileName = Path.GetFileName(file.FileName);
        ////            var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

        ////            // The files are not actually saved in this demo
        ////            // file.SaveAs(physicalPath);
        ////        }
        ////    }

        ////    // Return an empty string to signify success
        ////    return Content("");
        ////}

        public ActionResult Async_Remove(string[] fileNames)
        {
            // The parameter of the Remove action must be called "fileNames"

            if (fileNames != null)
            {
                foreach (var fullName in fileNames)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(Server.MapPath("~/App_Data"), fileName);

                    // TODO: Verify user permissions

                    if (System.IO.File.Exists(physicalPath))
                    {
                        // The files are not actually removed in this demo
                        // System.IO.File.Delete(physicalPath);
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }
        public ActionResult Async_Save(IEnumerable<HttpPostedFileBase> files)
        {
            long ParamPlayerID = 4;
            string FinalFileName = "";
            // The Name of the Upload component is "files"
            if (files != null)
            {
                PlayerImagesRepository modelRepo = new PlayerImagesRepository();

                string PlayerImagesFolder = Path.Combine(Server.MapPath("~" + SecurityUtils.Players_ImagePath + ""), ParamPlayerID.ToString());

                foreach (var file in files)
                {
                    if (!(System.IO.Directory.Exists(PlayerImagesFolder)))
                    {
                        System.IO.Directory.CreateDirectory(PlayerImagesFolder);
                    }

                    // Some browsers send file names with full path. This needs to be stripped.
                    string IEFileName = "", FileNameWithExtension = "";

                    if (Request.Browser.Browser.ToLower().Equals("ie") || Request.Browser.Browser.ToLower().Equals("internetexplorer"))
                    {
                        IEFileName = Path.GetFileName(file.FileName);
                    }

                    if (IEFileName != "")
                    {
                        FileNameWithExtension = IEFileName;
                        SecurityUtils.CheckforInvalidFileNameChar(ref FileNameWithExtension);
                    }
                    else
                    {
                        FileNameWithExtension = file.FileName;
                        SecurityUtils.CheckforInvalidFileNameChar(ref FileNameWithExtension);
                    }

                    var fileExtension = Path.GetExtension(FileNameWithExtension);

                    //Following Code will rename the file if it is already Exists
                    int count = 1;

                    string fileNameOnly = Guid.NewGuid().ToString(); //Path.GetFileNameWithoutExtension(FileNameWithExtension);
                    string extension = Path.GetExtension(FileNameWithExtension);
                    string newFullPath = FileNameWithExtension;
                    newFullPath = Path.Combine(PlayerImagesFolder, fileNameOnly + extension);

                    while (System.IO.File.Exists(newFullPath))
                    {
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                        newFullPath = Path.Combine(PlayerImagesFolder, tempFileName + extension);
                    }

                    fileNameOnly = Path.GetFileNameWithoutExtension(newFullPath);

                    //Update Filename from Repository
                    string Msg = "";
                    bool status = true;
                    FinalFileName = fileNameOnly + extension;

                    //Save the file to the Disk
                    file.SaveAs(newFullPath);

                    string PlayerImagePath = Path.Combine(PlayerImagesFolder, FinalFileName);

                    string WaterMarkImagePath = Server.MapPath("~/Images/verticle_mvff.png");

                    Image_Combine imgCombine = new Image_Combine();

                    string FinalFileName_WaterMarked = imgCombine.CombineBitmap_WatermarkImage(PlayerImagesFolder, FinalFileName, WaterMarkImagePath);

                    //Clear the Object
                    imgCombine = null;

                    PlayerImagesExt PlayerImg = new PlayerImagesExt()
                    {
                        PlayerID = ParamPlayerID,
                        FileName = FinalFileName_WaterMarked,
                        IsAnimated = false,
                        Display = true,
                        DefaultImage = false,
                        UploadDateTime = DateTime.Now
                    };

                    modelRepo.CreateOrUpdate(ref PlayerImg, ref Msg, ref status, this);

                    string NonWaterMarkImagePath = Path.Combine(PlayerImagesFolder, FinalFileName);

                    if (System.IO.File.Exists(NonWaterMarkImagePath))
                    {
                        System.IO.File.Delete(NonWaterMarkImagePath);
                    }

                    if (status && SecurityUtils.Enable_PremiumDashboard)
                    {
                        AchievementsRepository achRepo = new AchievementsRepository();

                        Guid guid = Guid.NewGuid();
                        AchievementThread = new Thread(() => achRepo.ImageGallery_AchievementPoints(ParamPlayerID, AchievementThread));
                        AchievementThread.Name = "ImageGallery_BeforeAfterImageCreated_AchievementPoints_" + guid.ToString();
                        AchievementThread.Start();
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public ActionResult Combine_BeforeAfterImage(long PlayerID, string FirstImagePath, string SecondImagePath)
        {
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();

            string PlayerImagesFolder = Path.Combine(Server.MapPath("~" + SecurityUtils.Players_ImagePath + ""), PlayerID.ToString());

            FirstImagePath = FirstImagePath.Replace("/Content/Data/PlayerImages/"+ PlayerID + "/", "");
            SecondImagePath = SecondImagePath.Replace("/Content/Data/PlayerImages/" + PlayerID + "/", "");

            string FirstImageServerPath = Path.Combine(Server.MapPath("~" + SecurityUtils.Players_ImagePath + "/"+ PlayerID.ToString()), FirstImagePath);
            string SecondImageServerPath = Path.Combine(Server.MapPath("~" + SecurityUtils.Players_ImagePath + "/" + PlayerID.ToString()), SecondImagePath);

            Image_Combine imgCombine = new Image_Combine();
           string CombinedImageName = imgCombine.CombineBitmap_BeforeAfter(PlayerImagesFolder, FirstImageServerPath, SecondImageServerPath);

            //Clear the Object
            imgCombine = null;

            string Msg = "";
            bool status = true;

            PlayerImagesExt PlayerImg = new PlayerImagesExt()
            {
                PlayerID = PlayerID,
                FileName = CombinedImageName,
                IsAnimated = false,
                Display = true,
                DefaultImage = false,
                UploadDateTime = DateTime.Now,
                IsBeforeAfter = true
            };

            modelRepo.CreateOrUpdate(ref PlayerImg, ref Msg, ref status, this);

            if(status && SecurityUtils.Enable_PremiumDashboard)
            {
                AchievementsRepository achRepo = new AchievementsRepository();

                Guid guid = Guid.NewGuid();
                AchievementThread = new Thread(() => achRepo.ImageGallery_BeforeAfterImageCreated_AchievementPoints(PlayerID, AchievementThread));
                AchievementThread.Name = "ImageGallery_BeforeAfterImageCreated_AchievementPoints_" + guid.ToString();
                AchievementThread.Start();
            }

            return new JsonResult { Data = status };
        }

        [Helpers.Authorization(Permissions.AllUsers)]
        public ActionResult RemovePlayerImages(string[] PlayerImgs, long ParamPlayerID=4)
        {
            string Msg = "";
            // The parameter of the Remove action must be called "fileNames"
            string PlayerImagesFolder = Path.Combine(Server.MapPath("~" + SecurityUtils.Players_ImagePath + ""), ParamPlayerID.ToString());

            if (PlayerImgs != null)
            {
                foreach (var fullName in PlayerImgs)
                {
                    var fileName = Path.GetFileName(fullName);
                    var physicalPath = Path.Combine(PlayerImagesFolder, fileName);

                    PlayerImagesRepository mRepo = new PlayerImagesRepository();

                    if (mRepo.Delete(fileName, ref Msg, this))
                    {
                        if (System.IO.File.Exists(physicalPath))
                        {
                            // The files are not actually removed in this demo
                            System.IO.File.Delete(physicalPath);
                        }
                    }
                }
            }

            // Return an empty string to signify success
            return Content("");
        }

        public JsonResult DeleteImage(long PlayerImageID, long PlayerID)
        {
            string Msg = "";
            bool status = true;
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            string fileName = modelRepo.Delete(PlayerImageID, ref Msg, ref status, this);
            return this.Json(new { status = status, Msg = Msg }, JsonRequestBehavior.AllowGet);
        }

        public ActionResult MarkImageDelete(long PlayerImageID)
        {
            string Msg = "";
            bool status = true;
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            modelRepo.MarkImageDelete(PlayerImageID, ref Msg, ref status, this);

            return new JsonResult { Data = new { status = status, Msg = Msg } };
        }

        public ActionResult UnMarkForDeletion(long PlayerImageID)
        {
            string Msg = "";
            bool status = true;
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            modelRepo.UnMarkForDeletion(PlayerImageID, ref Msg, ref status, this);

            return new JsonResult { Data = new { status = status, Msg = Msg } };
        }

        public ActionResult MakeDefaultImage(long PlayerImageID)
        {
            string Msg = "";
            bool status = true;
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            modelRepo.MakeDefaultImage(PlayerImageID, ref Msg, ref status, this);

            return new JsonResult { Data = new { status = status, Msg = Msg } };
        }

        public ActionResult MakeImageDisplay(long PlayerImageID)
        {
            string Msg = "";
            bool status = true;
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            modelRepo.MakeImageDisplay(PlayerImageID, ref Msg, ref status, this);

            return new JsonResult { Data = new { status = status, Msg = Msg } };
        }

        public ActionResult MakeImageFront_SideDisplay(long PlayerImageID)
        {
            string Msg = "";
            bool status = true;
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            modelRepo.MakeImageFront_SideDisplay(PlayerImageID, ref Msg, ref status, this);

            return new JsonResult { Data = new { status = status, Msg = Msg } };
        }

        public ActionResult UpdateImageDate(long PlayerImageID, DateTime ImageDate)
        {
            string Msg = "";
            bool status = true;
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            modelRepo.UpdateImageDate(PlayerImageID, ImageDate, ref Msg, ref status, this);

            return new JsonResult { Data = new { status = status, Msg = Msg } };
        }

        #endregion Player Image Get, Upload, Remove, Make Default, Make Display Image Functions

        #region Player Animation "GIF" Image

        public ActionResult CreateGif(long[] PlayerImageIDs)
        {
            string Msg = "";
            bool status = true;
            long ParamPlayerID = 0;
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            // var Selected_PlayerImgs = modelRepo.ReadAll(false).Where(m => PlayerImageIDs.Contains(m.PlayerImageID)).ToList();
            List<string> ListOfImagePaths = new List<string>();
            foreach (var item in PlayerImageIDs)
            {
                var Selected_PlayerImgs = modelRepo.ReadOne(item);
                string FullPath = Server.MapPath(Selected_PlayerImgs.ImageLink);
                ListOfImagePaths.Add(FullPath);
                ParamPlayerID = Selected_PlayerImgs.PlayerID;
            }

            string PlayerImagesFolder = Path.Combine(Server.MapPath("~" + SecurityUtils.Players_ImagePath + ""), ParamPlayerID.ToString());

            modelRepo.CreateGif(ListOfImagePaths, PlayerImagesFolder, ParamPlayerID, ref Msg, ref status, this);

            //String[] imageFilePaths = new String[] { "D:\\temp\\01.png", "D:\\temp\\02.png", "D:\\temp\\03.png" };
            //String outputFilePath = "D:\\temp\\test.gif";
            //AnimatedGifEncoder e = new AnimatedGifEncoder();
            //e.Start(outputFilePath);
            //e.SetDelay(1000);
            ////-1:no repeat,0:always repeat
            //e.SetRepeat(0);
            //for (int i = 0, count = imageFilePaths.Length; i < count; i++)
            //{
            //    e.AddFrame(Image.FromFile(imageFilePaths[i]));
            //}
            //e.Finish();
            ///* extract Gif */
            //string outputPath = "D:\\temp";
            //GifDecoder gifDecoder = new GifDecoder();
            //gifDecoder.Read("D:\\temp\\Finalsss.gif");
            //for (int i = 0, count = gifDecoder.GetFrameCount(); i < count; i++)
            //{
            //    Image frame = gifDecoder.GetFrame(i);  // frame i
            //    frame.Save(outputPath + Guid.NewGuid().ToString() + ".png", ImageFormat.Png);
            //}

            if (status && SecurityUtils.Enable_PremiumDashboard)
            {
                AchievementsRepository achRepo = new AchievementsRepository();

                Guid guid = Guid.NewGuid();
                AchievementThread = new Thread(() => achRepo.ImageGallery_AchievementPoints(ParamPlayerID, AchievementThread));
                AchievementThread.Name = "ImageGallery_BeforeAfterImageCreated_AchievementPoints_" + guid.ToString();
                AchievementThread.Start();
            }

            return new JsonResult { Data = new { status = status, Msg = Msg } };
        }

        #endregion Player Animation "GIF" Image

        [HttpPost]
        public JsonResult CalculateWeight(decimal weight, string activityDate, long playerId)
        {
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            modelRepo.AddPlayerWeight(weight, activityDate, playerId);
            return this.Json(new { playerId = playerId, lbsValue = Math.Round(Decimal.Multiply(weight, 2.2m)), kgValue = weight }, JsonRequestBehavior.AllowGet);
        }
        public JsonResult GetPlayerWeight(long playerId)
        {
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            var result = modelRepo.GetPlayerWeight(playerId);
            if (result != null)
            {
                return this.Json(new { playerId = result.PlayerID, lbsValue = Math.Round(Decimal.Multiply(result.Weight, 2.2m)), kgValue = result.Weight }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public JsonResult GetPlayerWeightByDate(long playerId, string activityDate)
        {
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            var result = modelRepo.GetPlayerWeightByDate(playerId, activityDate);
            if (result != null)
            {
                return this.Json(new { playerId = result.PlayerID, lbsValue = Math.Round(Decimal.Multiply(result.Weight, 2.2m)), kgValue = result.Weight }, JsonRequestBehavior.AllowGet);
            }
            else
            {
                return null;
            }
        }

        public JsonResult GetAllBeforeAfter(long PlayerId)
        {
            PlayerImagesRepository modelRepo = new PlayerImagesRepository();
            var result = modelRepo.ReadAll(PlayerId, false, true).Where(i=>i.PlayerID==PlayerId && i.IsBeforeAfter==true).ToList();
            return this.Json(result, JsonRequestBehavior.AllowGet);
        }
    }
}