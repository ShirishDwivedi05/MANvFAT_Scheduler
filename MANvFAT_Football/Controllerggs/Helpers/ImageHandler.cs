using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.IO;
using System.Drawing.Imaging;

namespace MANvFAT_Football.Helpers
{
    public class ImageHandler
    {
        public Image resize(Image imgToResize, Size size)
        {
            int sourceWidth = imgToResize.Width;
            int sourceHeight = imgToResize.Height;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)size.Width / (float)sourceWidth);
            nPercentH = ((float)size.Height / (float)sourceHeight);

            if (nPercentH < nPercentW)
                nPercent = nPercentH;
            else
                nPercent = nPercentW;

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap b = new Bitmap(destWidth, destHeight);
            Graphics g = Graphics.FromImage((Image)b);
            g.InterpolationMode = InterpolationMode.HighQualityBicubic;

            g.DrawImage(imgToResize, 0, 0, destWidth, destHeight);
            g.Dispose();

            return (Image)b;
        }

        public Image Resize_Usman(Image ImgToResize, Size size, string PlayerImagesFolder, int i)
        {
            Image imgPhoto = ImgToResize;

            int sourceWidth, sourceHeight, sourceX, sourceY, destX, destY;
            double nPercent, nPercentW, nPercentH;
            sourceWidth = imgPhoto.Width;
            sourceHeight = imgPhoto.Height;
            sourceX = 0;
            sourceY = 0;
            destX = 0;
            destY = 0;

            nPercent = 0;
            nPercentW = 0;
            nPercentH = 0;

            nPercentW = (Convert.ToDouble(size.Width) / Convert.ToDouble(sourceWidth));
            nPercentH = (Convert.ToDouble(size.Height) / Convert.ToDouble(sourceHeight));

            if (nPercentH < nPercentW)
            {
                nPercent = nPercentW;
                destY = Convert.ToInt32((size.Height - (sourceHeight * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentH;
                destX = Convert.ToInt32((size.Width - (sourceWidth * nPercent)) / 2);
            }

            int destWidth, destHeight;
            destWidth = Convert.ToInt32(sourceWidth * nPercent);
            destHeight = Convert.ToInt32(sourceHeight * nPercent);

            Bitmap bmPhoto;
            bmPhoto = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
            Graphics grPhoto;
            grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto, new Rectangle(destX, destY, destWidth, destHeight), new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), GraphicsUnit.Pixel);

            grPhoto.Dispose();


            string filenameToSave = Path.Combine(PlayerImagesFolder, i + ".png");
            bmPhoto.Save(filenameToSave, System.Drawing.Imaging.ImageFormat.Jpeg);
          //  bmPhoto.Dispose();

            imgPhoto.Dispose();

            return (Image)bmPhoto;
        }

        public string Get_Unique_FileName(string filePath, string fileName)
        {
            string imgName, imgExt;
            string renamedFile = "";
            int renameCount = 0;
            string newFileName = "";

            string[] NameExtArray;
            NameExtArray = fileName.Split('.');
            imgName = NameExtArray[0];
            imgExt = NameExtArray[1];

            while (File.Exists(filePath + imgName + renamedFile + "." + imgExt))
            {
                renameCount += 1;
                renamedFile = renameCount.ToString();
            }

            newFileName = imgName + renamedFile + "." + imgExt;
            return newFileName;
        }

        public Image cropImage(Image img, Rectangle cropArea)
        {
            Bitmap bmpImage = new Bitmap(img);
            Bitmap bmpCrop = bmpImage.Clone(cropArea,
            bmpImage.PixelFormat);

            bmpImage.Dispose();
            return (Image)(bmpCrop);
        }

        public void makeThumbnail(int width, int height, string FilePath, string UpFileName)
        {

            //making thumbnail...
            System.Drawing.Image imag;
            imag = System.Drawing.Image.FromFile(FilePath + UpFileName);
            System.Drawing.Image Thumbnailimag;
            System.Drawing.Image Cropedimag;
            //System.Drawing.Image Resizedimag;
            Size ThumbSize = new Size();
            //Size imagSize = new Size();
            ThumbSize.Width = width;
            ThumbSize.Height = height;

            ImageHandler resizeimage = new ImageHandler();
            Rectangle rectan = new Rectangle();

            if (imag.Height > imag.Width)
            {
                rectan.Height = imag.Width;
                rectan.Width = imag.Width;
                Point point = new Point();
                point.Y = (imag.Height - imag.Width) / 2;
                rectan.Offset(point);
                Cropedimag = resizeimage.cropImage(imag, rectan);
                Thumbnailimag = resizeimage.resize(Cropedimag, ThumbSize);
                UpFileName = resizeimage.Get_Unique_FileName(FilePath, UpFileName);
                Thumbnailimag.Save(FilePath + "Thumbnail_" + UpFileName);
            }
            else if (imag.Height <= imag.Width)
            {
                rectan.Width = imag.Height;
                rectan.Height = imag.Height;
                Point point = new Point();
                point.X = (imag.Width - imag.Height) / 2;
                rectan.Offset(point);
                Cropedimag = resizeimage.cropImage(imag, rectan);
                Thumbnailimag = resizeimage.resize(Cropedimag, ThumbSize);
                UpFileName = resizeimage.Get_Unique_FileName(FilePath, UpFileName);
                Thumbnailimag.Save(FilePath + "Thumbnail_" + UpFileName);
            }
        }

        public string Resize_Image_Thumb(string filePath, string srcImgName, int newWidth, int newHeight, string SessionID)
        {
            string newImgName, newImagePath, CurrentImagePath;

            try
            {
                CurrentImagePath = filePath + srcImgName;
                newImgName = SessionID;
                newImagePath = filePath + SessionID;

                Image imgPhoto = Image.FromFile(CurrentImagePath);

                int sourceWidth, sourceHeight, sourceX, sourceY, destX, destY;
                double nPercent, nPercentW, nPercentH;
                sourceWidth = imgPhoto.Width;
                sourceHeight = imgPhoto.Height;
                sourceX = 0;
                sourceY = 0;
                destX = 0;
                destY = 0;

                nPercent = 0;
                nPercentW = 0;
                nPercentH = 0;

                nPercentW = (Convert.ToDouble(newWidth) / Convert.ToDouble(sourceWidth));
                nPercentH = (Convert.ToDouble(newHeight) / Convert.ToDouble(sourceHeight));

                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentW;
                    destY = Convert.ToInt32((newHeight - (sourceHeight * nPercent)) / 2);
                }
                else
                {
                    nPercent = nPercentH;
                    destX = Convert.ToInt32((newWidth - (sourceWidth * nPercent)) / 2);
                }

                int destWidth, destHeight;
                destWidth = Convert.ToInt32(sourceWidth * nPercent);
                destHeight = Convert.ToInt32(sourceHeight * nPercent);

                Bitmap bmPhoto;
                bmPhoto = new Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
                Graphics grPhoto;
                grPhoto = Graphics.FromImage(bmPhoto);
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(imgPhoto, new Rectangle(destX, destY, destWidth, destHeight), new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), GraphicsUnit.Pixel);

                grPhoto.Dispose();

                bmPhoto.Save(newImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                bmPhoto.Dispose();

                imgPhoto.Dispose();

                //Delete the Old File from Server
                if (System.IO.File.Exists(CurrentImagePath))
                {
                    System.IO.File.Delete(CurrentImagePath);
                }
            }
            catch (Exception exp)
            {
                throw exp;
            }
            return newImgName;
        }

        public string Resize_Image(string filePath, string srcImgName, string newFilePath, int newWidth, string imgPrefix)
        {
            string newImgName, newImagePath;

            try
            {
                int newHeight = 0;

                newImgName = Get_Unique_FileName(newFilePath, imgPrefix + srcImgName);
                newImagePath = newFilePath + newImgName;

                Image imgPhoto = Image.FromFile(filePath + srcImgName);

                int sourceWidth, sourceHeight, sourceX, sourceY, destX, destY;
                double nPercent, nPercentW, nPercentH;
                sourceWidth = imgPhoto.Width;
                sourceHeight = imgPhoto.Height;
                sourceX = 0;
                sourceY = 0;
                destX = 0;
                destY = 0;

                nPercent = 0;
                nPercentW = 0;
                nPercentH = 0;

                if (sourceWidth > newWidth)
                {
                    newHeight = (int)(sourceHeight *
                      ((float)newWidth / (float)sourceWidth));
                }
                else
                {
                    newHeight = sourceHeight;
                }

                nPercentW = (Convert.ToDouble(newWidth) / Convert.ToDouble(sourceWidth));
                nPercentH = (Convert.ToDouble(newHeight) / Convert.ToDouble(sourceHeight));

                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentW;
                    destY = Convert.ToInt32((newHeight - (sourceHeight * nPercent)) / 2);
                }
                else
                {
                    nPercent = nPercentH;
                    destX = Convert.ToInt32((newWidth - (sourceWidth * nPercent)) / 2);
                }

                int destWidth, destHeight;
                destWidth = Convert.ToInt32(sourceWidth * nPercent);
                destHeight = Convert.ToInt32(sourceHeight * nPercent);

                Bitmap bmPhoto;
                bmPhoto = new Bitmap(newWidth, newHeight, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
                Graphics grPhoto;
                grPhoto = Graphics.FromImage(bmPhoto);
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(imgPhoto, new Rectangle(destX, destY, destWidth, destHeight), new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), GraphicsUnit.Pixel);

                grPhoto.Dispose();

                bmPhoto.Save(newImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                bmPhoto.Dispose();

                imgPhoto.Dispose();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            return newImgName;
        }

        public string Resize_Image_USMAN(Image imgToResize, string srcImgName, string newFilePath, Size size, string member_id)
        {
            string newImgName, newImagePath;

            try
            {
                newImgName = member_id + ".jpg";
                newImagePath = newFilePath + newImgName;

                Image imgPhoto = imgToResize;

                int sourceWidth, sourceHeight, sourceX, sourceY, destX, destY;
                double nPercent, nPercentW, nPercentH;
                sourceWidth = imgPhoto.Width;
                sourceHeight = imgPhoto.Height;
                sourceX = 0;
                sourceY = 0;
                destX = 0;
                destY = 0;

                nPercent = 0;
                nPercentW = 0;
                nPercentH = 0;

                nPercentW = (Convert.ToDouble(size.Width) / Convert.ToDouble(sourceWidth));
                nPercentH = (Convert.ToDouble(size.Height) / Convert.ToDouble(sourceHeight));

                if (nPercentH < nPercentW)
                {
                    nPercent = nPercentW;
                    destY = Convert.ToInt32((size.Height - (sourceHeight * nPercent)) / 2);
                }
                else
                {
                    nPercent = nPercentH;
                    destX = Convert.ToInt32((size.Width - (sourceWidth * nPercent)) / 2);
                }

                int destWidth, destHeight;
                destWidth = Convert.ToInt32(sourceWidth * nPercent);
                destHeight = Convert.ToInt32(sourceHeight * nPercent);

                Bitmap bmPhoto;
                bmPhoto = new Bitmap(size.Width, size.Height, System.Drawing.Imaging.PixelFormat.Format24bppRgb);

                bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);
                Graphics grPhoto;
                grPhoto = Graphics.FromImage(bmPhoto);
                grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

                grPhoto.DrawImage(imgPhoto, new Rectangle(destX, destY, destWidth, destHeight), new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight), GraphicsUnit.Pixel);

                grPhoto.Dispose();

                bmPhoto.Save(newImagePath, System.Drawing.Imaging.ImageFormat.Jpeg);
                bmPhoto.Dispose();

                imgPhoto.Dispose();
            }
            catch (Exception exp)
            {
                throw exp;
            }
            return newImgName;
        }

        /// <summary>
        /// Resize the Image by adding Padding around the image but original image will be untouched
        /// </summary>
        /// <param name="imgPhoto"></param>
        /// <param name="Width"></param>
        /// <param name="Height"></param>
        /// <returns></returns>
        public Image FixedSize(Image imgPhoto, int Width, int Height)
        {
            int sourceWidth = imgPhoto.Width;
            int sourceHeight = imgPhoto.Height;
            int sourceX = 0;
            int sourceY = 0;
            int destX = 0;
            int destY = 0;

            float nPercent = 0;
            float nPercentW = 0;
            float nPercentH = 0;

            nPercentW = ((float)Width / (float)sourceWidth);
            nPercentH = ((float)Height / (float)sourceHeight);

            //if we have to pad the height pad both the top and the bottom
            //with the difference between the scaled height and the desired height
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = (int)((Width - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = (int)((Height - (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.Black);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;

            grPhoto.DrawImage(imgPhoto,
                new Rectangle(destX, destY, destWidth, destHeight),
                new Rectangle(sourceX, sourceY, sourceWidth, sourceHeight),
                GraphicsUnit.Pixel);

            grPhoto.Dispose();
            return bmPhoto;
        }

        public void ResizeImage(string originalPath, string originalFileName, string newPath, string newFileName, int maximumWidth, int maximumHeight, bool enforceRatio, bool addPadding)
        {
            var image = Image.FromFile(originalPath + "\\" + originalFileName);
            var imageEncoders = ImageCodecInfo.GetImageEncoders();
            EncoderParameters encoderParameters = new EncoderParameters(1);
            encoderParameters.Param[0] = new EncoderParameter(System.Drawing.Imaging.Encoder.Quality, 100L);
            var canvasWidth = maximumWidth;
            var canvasHeight = maximumHeight;
            var newImageWidth = maximumWidth;
            var newImageHeight = maximumHeight;
            var xPosition = 0;
            var yPosition = 0;


            //if (image.Width > image.Height)
            //{
            //    var newx = maximumWidth;
            //    var shrinkratio = maximumWidth / (double)image.Width;
            //    var xyratio = (double)image.Height / (double)image.Width;
            //    var newy = newx * (double)xyratio;
            //    newImageHeight = (int)newy;
            //}

            //else if (image.Height >= image.Width)
            //{
            //    var newy = maximumHeight;
            //    var shrinkratio = maximumHeight / (double)image.Height;
            //    var xyratio = (double)image.Width / (double)image.Height;

            //    var newx = newy * xyratio;
            //    newImageWidth = (int)newx;
            //}

            if (enforceRatio)
            {

                var ratioX = maximumWidth / (double)image.Width;
                var ratioY = maximumHeight / (double)image.Height;
                var ratio = ratioX < ratioY ? ratioX : ratioY;
                newImageHeight = (int)(image.Height * ratio);
                newImageWidth = (int)(image.Width * ratio);

                if (addPadding)
                {
                    xPosition = (int)((maximumWidth - (image.Width * ratio)) / 2);
                    yPosition = (int)((maximumHeight - (image.Height * ratio)) / 2);
                }
                else
                {
                    canvasWidth = newImageWidth;
                    canvasHeight = newImageHeight;
                }
            }



            var thumbnail = new Bitmap(canvasWidth, canvasHeight);
            var graphic = Graphics.FromImage(thumbnail);

            if (enforceRatio && addPadding)
            {
                graphic.Clear(Color.White);
            }

            graphic.InterpolationMode = InterpolationMode.HighQualityBicubic;
            graphic.SmoothingMode = SmoothingMode.HighQuality;
            graphic.PixelOffsetMode = PixelOffsetMode.HighQuality;
            graphic.CompositingQuality = CompositingQuality.HighQuality;
            graphic.DrawImage(image, xPosition, yPosition, newImageWidth, newImageHeight);

            thumbnail.Save(newPath + "\\" + newFileName, imageEncoders[1], encoderParameters);

            graphic.Dispose();
            image.Dispose();
            encoderParameters.Dispose();
        }

    }

}
