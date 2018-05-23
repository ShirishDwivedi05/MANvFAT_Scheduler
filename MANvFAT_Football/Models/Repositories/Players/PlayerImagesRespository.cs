using Gif.Components;
using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Enumerations;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Web.Mvc;

namespace MANvFAT_Football.Models.Repositories
{
    public class PlayerImagesRepository : BaseRepository
    {
        public List<PlayerImagesExt> ReadAll(long PlayerID, bool? OnlyAnimated, bool All)
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var CurrentDomain = sysRepo.GetSystemSettings().CurrentDomain;

            var pImages = db.PlayerImages.Include("Players").Where(m => m.PlayerID == PlayerID).ToList().Select(m => Map(m, CurrentDomain));

            var Filtered_pImages = (from pimg in pImages
                                    where pimg.PlayerID == PlayerID &&
                           (OnlyAnimated.HasValue == false || pimg.IsAnimated == OnlyAnimated)
                           && (All == true || pimg.MarkForDeletion == false) //if it is calling from backend then return all pics but if from Player Progress Image page then only which are not marked as Deletetion
                                    select pimg);

            return Filtered_pImages.OrderByDescending(m => m.UploadDateTime).ToList();
        }

        public List<PlayerImagesExt> ReadAll_ProgressGallery(long PlayerID)
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var CurrentDomain = sysRepo.GetSystemSettings().CurrentDomain;

            var result = db.PlayerImages.Include("Players").Where(m => m.PlayerID == PlayerID && m.MarkForDeletion == false && m.Display == true).ToList().Select(m => Map(m, CurrentDomain)).OrderByDescending(m => m.UploadDateTime).ToList();

            return result;
        }

        public List<PlayerImagesExt> ReadAll_ProgressGallery(string PlayerEmailAddress)
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var CurrentDomain = sysRepo.GetSystemSettings().CurrentDomain;

            var result = db.PlayerImages.Include("Players").Where(m => m.Players.EmailAddress == PlayerEmailAddress && m.MarkForDeletion == false && m.Display == true).ToList().Select(m => Map(m, CurrentDomain)).OrderByDescending(m => m.UploadDateTime).ToList();

            return result;
        }

        public List<PlayerImagesExt> ReadAll(bool? OnlyAnimated)
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var CurrentDomain = sysRepo.GetSystemSettings().CurrentDomain;

            var pImages = db.PlayerImages.Include("Players").ToList().Select(m => Map(m, CurrentDomain)).ToList();

            var Filtered_pImages = (from pimg in pImages
                                    where (OnlyAnimated.HasValue == false || pimg.IsAnimated == OnlyAnimated)
                                    select pimg).ToList();

            return Filtered_pImages;
        }

        public PlayerImagesExt ReadOne(long PlayerImageID)
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var CurrentDomain = sysRepo.GetSystemSettings().CurrentDomain;

            var model = db.PlayerImages.Include("Players").Where(m => m.PlayerImageID == PlayerImageID).FirstOrDefault();
            return Map(model, CurrentDomain);
        }

        public PlayerImagesExt ReadOne(string FileName)
        {
            SystemSettingsRepository sysRepo = new SystemSettingsRepository();
            var CurrentDomain = sysRepo.GetSystemSettings().CurrentDomain;

            var model = db.PlayerImages.Include("Players").Where(m => m.FileName == FileName).FirstOrDefault();
            return Map(model, CurrentDomain);
        }

       
        public long CreateOrUpdate(ref PlayerImagesExt model, ref string Msg, ref bool status, Controller ctrl)
        {
            long PlayerImageID = model.PlayerImageID;
            try
            {
                if (model.PlayerImageID == 0)
                {
                    model.UploadDateTime = DateTime.Now;
                    //Create New Record
                    //TODO: Map to DB Object
                    var dbmodel = Map(model);
                    //TODO: Save DB Changes and Set the Return Primary Key ID
                    db.PlayerImages.Add(dbmodel);
                    db.SaveChanges();
                    PlayerImageID = dbmodel.PlayerID;
                    Msg = "New Player Image Created Successfully";
                    //TOD: Add to Audit Log
                    AuditLog(ctrl, AuditAction.Create, ReadOne(dbmodel.PlayerImageID), null);
                }
                else
                {
                    SystemSettingsRepository sysRepo = new SystemSettingsRepository();
                    var CurrentDomain = sysRepo.GetSystemSettings().CurrentDomain;

                    //Update Existing Record
                    var dbmodel = db.PlayerImages.FirstOrDefault(p => p.PlayerImageID == PlayerImageID);
                    var ForAuditLog = Map(dbmodel, CurrentDomain);
                    //TODO: Map to DB Object
                    MapUpdate(ref dbmodel, model);
                    //TODO: Update DB Changes
                    db.SaveChanges();
                    PlayerImageID = dbmodel.PlayerImageID;
                    Msg = "Player Image Updated Successfully";
                    //TOD: Add to Audit Log
                    AuditLog(ctrl, AuditAction.Update, ForAuditLog, ReadOne(dbmodel.PlayerImageID));
                }
            }
            catch (Exception ex)
            {
                Msg = ErrorHandling.HandleException(ex);
                status = false;
            }

            return PlayerImageID;
        }

        public bool MakeDefaultImage(long PlayerImageID, ref string Msg, ref bool status, Controller ctrl)
        {
            try
            {
                db.UpdatePlayerImageAsDefault(PlayerImageID);
            }
            catch (Exception ex)
            {
                status = false;
                Msg = ErrorHandling.HandleException(ex);
            }

            return status;
        }

        public bool MakeImageDisplay(long PlayerImageID, ref string Msg, ref bool status, Controller ctrl)
        {
            try
            {
                var pImage = ReadOne(PlayerImageID);
                pImage.Display = !pImage.Display;
                if (pImage.DefaultImage && pImage.Display == false)
                {
                    status = false;
                    Msg = "You cannot Hide Default Image, Please Upload/Select any other Default Image to continue";
                }
                else
                {
                    CreateOrUpdate(ref pImage, ref Msg, ref status, ctrl);
                }
            }
            catch (Exception ex)
            {
                status = false;
                Msg = ErrorHandling.HandleException(ex);
            }

            return status;
        }

        public bool MakeImageFront_SideDisplay(long PlayerImageID, ref string Msg, ref bool status, Controller ctrl)
        {
            try
            {
                var pImage = ReadOne(PlayerImageID);
                pImage.IsFront = !pImage.IsFront;
                pImage.IsSide = !pImage.IsSide;

                CreateOrUpdate(ref pImage, ref Msg, ref status, ctrl);
            }
            catch (Exception ex)
            {
                status = false;
                Msg = ErrorHandling.HandleException(ex);
            }

            return status;
        }

        public bool UpdateImageDate(long PlayerImageID, DateTime ImageDate, ref string Msg, ref bool status, Controller ctrl)
        {
            try
            {
                var pImage = ReadOne(PlayerImageID);
                pImage.UploadDateTime = ImageDate;

                CreateOrUpdate(ref pImage, ref Msg, ref status, ctrl);
            }
            catch (Exception ex)
            {
                status = false;
                Msg = ErrorHandling.HandleException(ex);
            }

            return status;
        }

        public bool Delete(PlayerImagesExt model, ref string Msg, Controller ctrl)
        {
            bool status = true;
            try
            {
                //TODO: Get Current Object from DB
                var ItemToDelete = db.PlayerImages.FirstOrDefault(m => m.PlayerImageID == model.PlayerImageID);
                if (ItemToDelete != null)
                {
                    if (ItemToDelete.DefaultImage)
                    {
                        status = false;
                        Msg = "You cannot Delete Default Image, Please mark any other image as default in order to Delete this Image.";
                    }
                    else
                    {
                        SystemSettingsRepository sysRepo = new SystemSettingsRepository();
                        var CurrentDomain = sysRepo.GetSystemSettings().CurrentDomain;

                        var ForAudiLog = Map(ItemToDelete, CurrentDomain);
                        //TODO: Check if it is not null, then Remove from DB
                        db.PlayerImages.Remove(ItemToDelete);
                        db.SaveChanges();

                        //Add To Log
                        AuditLog(ctrl, AuditAction.Delete, null, ForAudiLog);
                    }
                }
            }
            catch (Exception ex)
            {
                Msg = ErrorHandling.HandleException(ex);
                status = false;
            }

            return status;
        }

        public bool Delete(string FileName, ref string Msg, Controller ctrl)
        {
            bool status = true;
            try
            {
                //TODO: Get Current Object from DB
                var ItemToDelete = db.PlayerImages.FirstOrDefault(m => m.FileName == FileName);
                if (ItemToDelete != null)
                {
                    if (ItemToDelete.DefaultImage)
                    {
                        status = false;
                        Msg = "You cannot Delete Default Image, Please mark any other image as default in order to Delete this Image.";
                    }
                    else
                    {
                        SystemSettingsRepository sysRepo = new SystemSettingsRepository();
                        var CurrentDomain = sysRepo.GetSystemSettings().CurrentDomain;

                        var ForAudiLog = Map(ItemToDelete, CurrentDomain);
                        //TODO: Check if it is not null, then Remove from DB
                        db.PlayerImages.Remove(ItemToDelete);
                        db.SaveChanges();

                        //Add To Log
                        AuditLog(ctrl, AuditAction.Delete, null, ForAudiLog);
                    }
                }
            }
            catch (Exception ex)
            {
                Msg = ErrorHandling.HandleException(ex);
                status = false;
            }

            return status;
        }

        public string Delete(long PlayerImageID, ref string Msg, ref bool status, Controller ctrl)
        {
            string FileName = "";

            try
            {
                //TODO: Get Current Object from DB
                var ItemToDelete = db.PlayerImages.FirstOrDefault(m => m.PlayerImageID == PlayerImageID);
                if (ItemToDelete != null)
                {
                    if (ItemToDelete.DefaultImage)
                    {
                        status = false;
                        Msg = "You cannot Delete Default Image, Please mark any other image as default in order to Delete this Image.";
                    }
                    else
                    {
                        SystemSettingsRepository sysRepo = new SystemSettingsRepository();
                        var CurrentDomain = sysRepo.GetSystemSettings().CurrentDomain;

                        FileName = ItemToDelete.FileName;
                        long PlayerID = ItemToDelete.PlayerID;
                        var ForAudiLog = Map(ItemToDelete, CurrentDomain);
                        //TODO: Check if it is not null, then Remove from DB
                        db.PlayerImages.Remove(ItemToDelete);
                        db.SaveChanges();

                        //Add To Log
                        AuditLog(ctrl, AuditAction.Delete, null, ForAudiLog);

                        //Now Delete the Physical Image

                        string PlayerImagesFolder = Path.Combine(ctrl.Server.MapPath("~" + SecurityUtils.Players_ImagePath + ""), PlayerID.ToString());

                        string FullFilePath = Path.Combine(PlayerImagesFolder, FileName);

                        if (System.IO.File.Exists(FullFilePath))
                        {
                            // The files are not actually removed in this demo
                            System.IO.File.Delete(FullFilePath);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Msg = ErrorHandling.HandleException(ex);
                status = false;
            }

            return FileName;
        }

        public void MarkImageDelete(long PlayerImageID, ref string Msg, ref bool status, Controller ctrl)
        {
            try
            {
                var ItemToMarkDelete = db.PlayerImages.FirstOrDefault(m => m.PlayerImageID == PlayerImageID);
                if (ItemToMarkDelete != null)
                {
                    ItemToMarkDelete.MarkForDeletion = true;
                    db.SaveChanges();

                    SecurityUtils.AddAuditLog("Player Progress Gallery", "Player Marked their Picture as Deleted from Thier Picture Gallery", ctrl);
                }
            }
            catch (Exception ex)
            {
                Msg = ErrorHandling.HandleException(ex);
                status = false;
            }
        }

        public void UnMarkForDeletion(long PlayerImageID, ref string Msg, ref bool status, Controller ctrl)
        {
            try
            {
                var ItemToMarkDelete = db.PlayerImages.FirstOrDefault(m => m.PlayerImageID == PlayerImageID);
                if (ItemToMarkDelete != null)
                {
                    ItemToMarkDelete.MarkForDeletion = false;
                    db.SaveChanges();

                    SecurityUtils.AddAuditLog("Player Progress Gallery", "Admin Un-Marked Picture for Deletetion", ctrl);
                }
            }
            catch (Exception ex)
            {
                Msg = ErrorHandling.HandleException(ex);
                status = false;
            }
        }

        public void CreateGif(List<string> ListOfImagePaths, string PlayerImagesFolder, long ParamPlayerID, ref string Msg, ref bool status, Controller ctrl)
        {
            string[] imageFilePaths = ListOfImagePaths.ToArray();// new string[] { "D:\\temp\\01.png", "D:\\temp\\02.png", "D:\\temp\\03.png" };

            int MAX_Width = 0;
            int MAX_Height = 0;

            foreach (var item in imageFilePaths)
            {
                using (Image imgPhoto = Image.FromFile(item))
                {
                    //create a image object containing the photograph to watermark

                    int phWidth = imgPhoto.Width;
                    if (MAX_Width < phWidth) { MAX_Width = phWidth; }

                    int phHeight = imgPhoto.Height;
                    if (MAX_Height < phHeight) { MAX_Height = phHeight; }
                }
            }

            string fileNameOnly = Guid.NewGuid().ToString() + ".gif";

            string outputFilePath = Path.Combine(PlayerImagesFolder, fileNameOnly);

            ImageHandler imageHandler = new ImageHandler();
            List<Image> ListOfImages = new List<Image>();
            int i = 500;
            foreach (var item in imageFilePaths)
            {
                using (Image imgPhoto = Image.FromFile(item))
                {
                    //create a image object containing the photograph to watermark
                    Size size = new Size(MAX_Width, MAX_Height);
                    ListOfImages.Add(imageHandler.FixedSize(imgPhoto, size.Width, size.Height));
                    // ListOfImages.Add(imageHandler.Resize_Usman(imgPhoto, size, PlayerImagesFolder, i));
                }

                i = i + 1;
            }

            AnimatedGifEncoder e = new AnimatedGifEncoder();
            e.SetSize(MAX_Width, MAX_Height);
            e.Start(outputFilePath);
            e.SetDelay(1000);
            //-1:no repeat,0:always repeat
            e.SetRepeat(0);
            foreach (var item in ListOfImages)
            {
                e.AddFrame(item);
            }
            //for (int i = 0, count = imageFilePaths.Length; i < count; i++)
            //{
            //    e.AddFrame(Image.FromFile(imageFilePaths[i]));
            //}
            e.Finish();

            e.SetDispose(0);
            /* extract Gif */
            //string outputPath = "D:\\temp";
            //GifDecoder gifDecoder = new GifDecoder();
            //gifDecoder.Read("D:\\temp\\Finalsss.gif");
            //for (int i = 0, count = gifDecoder.GetFrameCount(); i < count; i++)
            //{
            //    Image frame = gifDecoder.GetFrame(i);  // frame i
            //    frame.Save(outputPath + Guid.NewGuid().ToString() + ".png", ImageFormat.Png);
            //}

            //Add it to Database
            PlayerImagesExt PlayerImg = new PlayerImagesExt()
            {
                PlayerID = ParamPlayerID,
                FileName = fileNameOnly,
                IsAnimated = true,
                Display = true,
                DefaultImage = false
            };

            CreateOrUpdate(ref PlayerImg, ref Msg, ref status, ctrl);
        }

        public PlayerImages Map(PlayerImagesExt model)
        {
            PlayerImages tblModel = new PlayerImages()
            {
                PlayerImageID = model.PlayerImageID,
                PlayerID = model.PlayerID,
                FileName = model.FileName,
                IsAnimated = model.IsAnimated,
                Display = model.Display,
                DefaultImage = model.DefaultImage,
                UploadDateTime = model.UploadDateTime,
            };

            return tblModel;
        }

        public PlayerImagesExt Map(PlayerImages model, string CurrentDomain)
        {
            PlayerImagesExt tblModel = new PlayerImagesExt()
            {
                PlayerImageID = model.PlayerImageID,
                PlayerID = model.PlayerID,

                FileName = model.FileName,
                IsAnimated = model.IsAnimated,
                Display = model.Display,
                DefaultImage = model.DefaultImage,
               // IsFront = model.IsFront,
                //IsSide = model.IsSide,
                btnCss = model.DefaultImage ? "btn-success" : "btn-primary",
                ImageLink = SecurityUtils.Players_ImagePath + model.PlayerID + "/" + model.FileName,
                MarkForDeletion = model.MarkForDeletion,

                Display_Text = model.Display ? "Exclulde" : "Include",
                Display_btnCss = model.Display ? "btn-info" : "btn-warning",
                Display_Title = model.Display ? "Click here to Exclude this picture from Progress Emails" : "Click here to Include this picture in Progress Emails",

               // FrontSide_Text = model.IsFront ? "Mark Side Image" : "Mark Front Image",
                //FrontSide_btnCss = model.IsFront ? "btn-info" : "btn-warning",
                //FrontSide_Title = model.IsFront ? "Click here to Mark this picture as Side image" : "Click here to Mark this picture as Front image",

                UploadDateTime = model.UploadDateTime,
                UploadDateTime_Str = model.UploadDateTime.ToString("dd/MM/yyyy HH:mm"),
                UploadDate_Str = model.UploadDateTime.ToString("dd-MM-yyyy"),
                NextPhotoUploadDate = model.UploadDateTime.Date.AddDays(7),
                AbsoluteImageLink = CurrentDomain + SecurityUtils.Players_ImagePath + model.PlayerID + "/" + model.FileName
            };

            return tblModel;
        }

        public void MapUpdate(ref PlayerImages dbmodel, PlayerImagesExt model)
        {
            dbmodel.PlayerImageID = model.PlayerImageID;
            dbmodel.PlayerID = model.PlayerID;
            dbmodel.FileName = model.FileName;
            dbmodel.IsAnimated = model.IsAnimated;
            dbmodel.Display = model.Display;
            dbmodel.DefaultImage = model.DefaultImage;
            dbmodel.UploadDateTime = model.UploadDateTime;
            dbmodel.IsBaforeAfter = model.IsBeforeAfter;
           
        }


        /// <summary>
        /// Add the Action to Audit Log
        /// </summary>
        /// <param name="model">The Object for which this Auditlog took place</param>
        /// <param name="Action">"Added New User OR Updated User Details OR Deleted User"</param>

        private void AuditLog(Controller ctrl, AuditAction auditAction, PlayerImagesExt dbmodel = null, PlayerImagesExt model = null)
        {
            string AuditLogShortDesc = "", AuditLogLongDesc = "";

            //StringBuilder sb = new StringBuilder();

            if (auditAction == AuditAction.Create) //Creating new Record
            {
                AuditLogShortDesc = AuditLogLongDesc = "New Player Image has been Added";

                //sb.Append("<table class='table table-stripped auditLogStyle'>");
                //sb.Append("<tr><th colspan='2'>New User has been Added</th></tr>");
                //sb.Append("<tr><th>Full Name</th><td>" + dbmodel.FullName + "</td></tr>");
                //sb.Append("<tr><th>Email</th><td>" + dbmodel.EmailAddress + "</td></tr>");
                //sb.Append("<tr><th>Role</th><td>" + dbmodel.Role + "</td></tr>");
                //sb.Append("<tr><th>Locked</th><td>" + dbmodel.Locked + "</td></tr>");
                //sb.Append("<tr><th>Deleted</th><td>" + dbmodel.Deleted + "</td></tr>");
                //sb.Append("</table>");

                //AuditLogLongDesc = sb.ToString();
            }
            else if (auditAction == AuditAction.Update)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Player Image has been Updated";

                //sb.Append("<table class='table table-stripped auditLogStyle'>");

                //sb.Append("<tr><th colspan='2'>User has been Updated</th></tr>");
                //sb.Append("<tr><th colspan='2'><label>BEFORE</label></th></tr>");

                //sb.Append("<tr><th>Full Name</th><td>" + dbmodel.FullName + "</td></tr>");
                //sb.Append("<tr><th>Email</th><td>" + dbmodel.EmailAddress + "</td></tr>");
                //sb.Append("<tr><th>Role</th><td>" + dbmodel.Role + "</td></tr>");
                //sb.Append("<tr><th>Locked</th><td>" + dbmodel.Locked + "</td></tr>");
                //sb.Append("<tr><th>Deleted</th><td>" + dbmodel.Deleted + "</td></tr>");

                //sb.Append("<tr><th colspan='2'><label>AFTER</label></th></tr>");

                //sb.Append("<tr><th>Full Name</th><td>" + dbmodel.FullName + "</td></tr>");
                //sb.Append("<tr><th>Email</th><td>" + dbmodel.EmailAddress + "</td></tr>");
                //sb.Append("<tr><th>Role</th><td>" + dbmodel.Role + "</td></tr>");
                //sb.Append("<tr><th>Locked</th><td>" + dbmodel.Locked + "</td></tr>");
                //sb.Append("<tr><th>Deleted</th><td>" + dbmodel.Deleted + "</td></tr>");

                //sb.Append("</table>");

                //AuditLogLongDesc = sb.ToString();
            }
            else if (auditAction == AuditAction.Delete)
            {
                AuditLogShortDesc = AuditLogLongDesc = "Player Image has been Deleted";

                //sb.Append("<tr><th>Full Name</th><td>" + dbmodel.FullName + "</td></tr>");
                //sb.Append("<tr><th>Email</th><td>" + dbmodel.EmailAddress + "</td></tr>");
                //sb.Append("<tr><th>Role</th><td>" + dbmodel.Role + "</td></tr>");
                //sb.Append("<tr><th>Locked</th><td>" + dbmodel.Locked + "</td></tr>");
                //sb.Append("<tr><th>Deleted</th><td>" + dbmodel.Deleted + "</td></tr>");

                //sb.Append("</table>");

                //AuditLogLongDesc = sb.ToString();
            }

            SecurityUtils.AddAuditLog(AuditLogShortDesc, AuditLogLongDesc, ctrl);
        }
    }

    public class PlayerImagesExt
    {
        public long PlayerImageID { get; set; }
        public long PlayerID { get; set; }
        public string FileName { get; set; }
        public bool IsAnimated { get; set; }
        public bool Display { get; set; }
        public bool DefaultImage { get; set; }
        public bool IsFront { get; set; }
        public bool IsSide { get; set; }
        public string ImageLink { get; set; }
        public string btnCss { get; set; }
        public string Display_btnCss { get; set; }
        public string Display_Title { get; set; }
        public string Display_Text { get; set; }

        public string FrontSide_btnCss { get; set; }
        public string FrontSide_Title { get; set; }
        public string FrontSide_Text { get; set; }

        public System.DateTime UploadDateTime { get; set; }
        public string UploadDateTime_Str { get; set; }
        public string UploadDate_Str { get; set; }
        public bool MarkForDeletion { get; set; }

        public string AbsoluteImageLink { get; set; }
        public string AbsoluteDefaultImageLink { get; set; }
        public string Current_Previous { get; set; }
        public DateTime NextPhotoUploadDate { get; set; }

        public bool IsBeforeAfter { get; set; }
        /*
         Email: Wed 16/08/2017 11:03

        I THINK THE SIMPLEST ANSWER IS THAT HE'S ALLOWED 2 PHOTOS PER 7 DAY PERIOD. WHEN HE UPLOADS THEM THAT PLAYER CAN'T UPLOAD PHOTOS UNTIL 7 DAYS HAVE ELAPSED OR HE HAS DELETED THEM ON HIS GALLERY PAGE - E.G. IF HE UPLOADS 2 AND THEN DECIDES THAT HE HATES THEM HE GOES TO HIS GALLERY PAGE AND DELETES THEM - HE WOULD THEN BE ALLOWED TO UPLOAD 2 MORE WITHIN THAT 7 DAY PERIOD. GENERALLY SPEAKING I DON'T THINK WE'LL GET PLAYERS DELETING THEIR PHOTOS BECAUSE WE'LL TELL THEM TO ONLY UPLOAD THE ONES THEY LIKE IN THE FIRST PLACE.
         */
    }
}