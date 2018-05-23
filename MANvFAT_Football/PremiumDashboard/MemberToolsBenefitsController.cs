using Kendo.Mvc.Extensions;
using Kendo.Mvc.UI;
using MANvFAT_Football.Helpers;
using MANvFAT_Football.Models.Enumerations;
using MANvFAT_Football.Models.Repositories;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Web;
using System.Web.Mvc;

namespace MANvFAT_Football.Controllers
{
    [Authorization(Permissions.Administrator)]
    public class MemberToolsBenefitsController : Controller
    {
        // GET: MemberToolsBenefits

        public ActionResult Index()
        {
            return View();
        }

        #region Grid Create, Read, Update, Delete Functions

      

       

        
        #endregion Grid Create, Read, Update, Delete Functions

        #region Photo Upload

        public ActionResult SaveToolBenefit_Photo(IEnumerable<HttpPostedFileBase> ToolBenefit_Photo, long paramMemberToolsBenefitID)
        {
            string FinalFileName = "";
            // The Name of the Upload component is "files"
            if (ToolBenefit_Photo != null)
            {
                string ToolBenefit_ImageFolder = Server.MapPath("~" + SecurityUtils.ToolBenefit_ImagePath + "");

                foreach (var file in ToolBenefit_Photo)
                {
                    if (!(System.IO.Directory.Exists(ToolBenefit_ImageFolder)))
                    {
                        System.IO.Directory.CreateDirectory(ToolBenefit_ImageFolder);
                    }

                    using (Bitmap originalImage = new Bitmap(file.InputStream))
                    {
                        var width = originalImage.Width;
                        var height = originalImage.Height;

                        if(width>800 || height>600)
                        {
                            return Content("Maximum allowed Image resolution is 800x600 BUT Uploaded Image resolution is "+width.ToString()+"x"+height+" . Please resize the image and try again.");
                        }

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
                    newFullPath = Path.Combine(ToolBenefit_ImageFolder, fileNameOnly + extension);

                    while (System.IO.File.Exists(newFullPath))
                    {
                        string tempFileName = string.Format("{0}({1})", fileNameOnly, count++);
                        newFullPath = Path.Combine(ToolBenefit_ImageFolder, tempFileName + extension);
                    }

                    fileNameOnly = Path.GetFileNameWithoutExtension(newFullPath);

                    //Update Filename from Repository
                    string Msg = "";
                    bool status = true;
                    FinalFileName = fileNameOnly + extension;

                    //Save the file to the Disk
                    file.SaveAs(newFullPath);

                    // ImageWaterMark imgWaterMark = new ImageWaterMark();

                    string ToolBenefitImage_Path = Path.Combine(ToolBenefit_ImageFolder, FinalFileName);

                    MemberToolsBenefitsRepository modelRepo = new MemberToolsBenefitsRepository();
                    //var model = modelRepo.ReadOne(paramMemberToolsBenefitID);

                    ////Check if already had an Image then Delete the previous image before updating new in DB
                    //if(!string.IsNullOrEmpty(model.PictureName))
                    //{
                    //    string PrevImgPath = Path.Combine(ToolBenefit_ImageFolder, model.PictureName);

                    //    if (System.IO.File.Exists(PrevImgPath))
                    //    {
                    //        System.IO.File.Delete(PrevImgPath);
                    //    }
                    //}
                    ////Update currently Updloaded Image
                    //modelRepo.AddToolBenefitImage(paramMemberToolsBenefitID, FinalFileName, ref Msg, this);

                }
            }
            // Return an empty string to signify success
            return Content("");
        }

        #endregion Photo Upload
    }
}