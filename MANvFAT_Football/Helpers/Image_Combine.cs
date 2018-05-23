using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Linq;
using System.Web;

namespace MANvFAT_Football.Helpers
{
    public class Image_Combine
    {

        public string CombineBitmap_WatermarkImage(string WorkingDirectory, string FirstImageName, string SecondImagePath)
        {
            string FirstImagePath = System.IO.Path.Combine(WorkingDirectory, FirstImageName);

            string FinalImageName = Guid.NewGuid().ToString() + ".png";

            //read all images into memory

            System.Drawing.Bitmap finalImage = null;
            System.Drawing.Bitmap First_Image = null;
            System.Drawing.Bitmap Second_Image = null;
            try
            {
                int width = 0;
                int height = 0;

                First_Image = new System.Drawing.Bitmap(FirstImagePath);
                Second_Image = new System.Drawing.Bitmap(SecondImagePath);

                width = First_Image.Width;
                height = First_Image.Height + Second_Image.Height;

                //create a bitmap to hold the combined image
                finalImage = new System.Drawing.Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(System.Drawing.Color.Black);

                    //go through each image and draw it on the final image
                    int offset = 0;

                    g.DrawImage(First_Image,
                      new System.Drawing.Rectangle(0, 0, First_Image.Width, First_Image.Height));

                    g.DrawImage(Second_Image,
                        new System.Drawing.Rectangle(First_Image.Width - Second_Image.Width, First_Image.Height, Second_Image.Width, Second_Image.Height));
                }

                finalImage.Save(WorkingDirectory + "\\" + FinalImageName, ImageFormat.Png);
                //   imgPhoto.Dispose();
                finalImage.Dispose();

                return FinalImageName;
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
                if (finalImage != null)
                    finalImage.Dispose();
                //throw ex;
                throw ex;
            }
            finally
            {
                //clean up memory

                First_Image.Dispose();
                Second_Image.Dispose();
            }
        }

        public string CombineBitmap_BeforeAfter(string WorkingDirectory, string FirstImagePath, string SecondImagePath)
        {
            //string FirstImagePath = System.IO.Path.Combine(WorkingDirectory, FirstImageName);

            string FinalImageName = Guid.NewGuid().ToString() + ".png";

            //read all images into memory

            System.Drawing.Bitmap finalImage = null;
            System.Drawing.Bitmap First_Image = null;
            System.Drawing.Bitmap Second_Image = null;
            try
            {
                int width = 0;
                int height = 0;
                int Seperation = 20;//px
                First_Image = new System.Drawing.Bitmap(FirstImagePath);
                Second_Image = new System.Drawing.Bitmap(SecondImagePath);

                //We need to combine both Images so width should be first + second image, Plus the 20px separation between images
                width = First_Image.Width + Second_Image.Width + Seperation;

                //height should be of the maximum image height
                if (First_Image.Height > Second_Image.Height)
                {
                    height = First_Image.Height;
                }
                else
                {
                    height = Second_Image.Height;
                }

                //create a bitmap to hold the combined image
                finalImage = new System.Drawing.Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(System.Drawing.Color.Black);

                    //go through each image and draw it on the final image
                    int offset = 0;

                    g.DrawImage(First_Image,
                      new System.Drawing.Rectangle(0, 0, First_Image.Width, First_Image.Height));

                    //Start to Draw Second Image after First Image Width + the seperation px required between both images
                    g.DrawImage(Second_Image,
                        new System.Drawing.Rectangle(First_Image.Width + Seperation, 0, Second_Image.Width, Second_Image.Height));
                }

                finalImage.Save(WorkingDirectory + "\\" + FinalImageName, ImageFormat.Png);
                //   imgPhoto.Dispose();
                finalImage.Dispose();

                return FinalImageName;
            }
            catch (Exception ex)
            {
                ErrorHandling.HandleException(ex);
                if (finalImage != null)
                    finalImage.Dispose();
                //throw ex;
                throw ex;
            }
            finally
            {
                //clean up memory

                First_Image.Dispose();
                Second_Image.Dispose();
            }
        }

        public System.Drawing.Bitmap CombineBitmap(string[] files)
        {
            //read all images into memory
            List<System.Drawing.Bitmap> images = new List<System.Drawing.Bitmap>();
            System.Drawing.Bitmap finalImage = null;

            try
            {
                int width = 0;
                int height = 0;

                foreach (string image in files)
                {
                    //create a Bitmap from the file and add it to the list
                    System.Drawing.Bitmap bitmap = new System.Drawing.Bitmap(image);

                    //update the size of the final bitmap
                    //width += bitmap.Width;
                    //height = bitmap.Height > height ? bitmap.Height : height;

                    height += bitmap.Height;
                    width = bitmap.Width > width ? bitmap.Width : width;

                    images.Add(bitmap);
                }

                //create a bitmap to hold the combined image
                finalImage = new System.Drawing.Bitmap(width, height);

                //get a graphics object from the image so we can draw on it
                using (System.Drawing.Graphics g = System.Drawing.Graphics.FromImage(finalImage))
                {
                    //set background color
                    g.Clear(System.Drawing.Color.Black);

                    //go through each image and draw it on the final image
                    int offset = 0;
                    foreach (System.Drawing.Bitmap image in images)
                    {
                        g.DrawImage(image,
                          new System.Drawing.Rectangle(0, offset, image.Width, image.Height));
                        offset += image.Height;
                    }
                }

                return finalImage;
            }
            catch (Exception)
            {
                if (finalImage != null)
                    finalImage.Dispose();
                //throw ex;
                throw;
            }
            finally
            {
                //clean up memory
                foreach (System.Drawing.Bitmap image in images)
                {
                    image.Dispose();
                }
            }
        }

    }
}