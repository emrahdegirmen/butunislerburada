using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Drawing2D;
using System.Drawing.Imaging;
using System.IO;
using System.Web;

namespace butunislerburada.MVC.Areas.admin
{
    public static class FileHelper
    {
        public static string SaveResizedImage(HttpPostedFileBase file, string folderName, string filename, Dictionary<int, int> ImageWH, Dictionary<int, int> ThumbImageWH)
        {
            string returnValue = "";

            string extension = System.IO.Path.GetExtension(file.FileName);

            if (extension == ".jpg" || extension == ".JPG" || extension == ".jpeg" || extension == ".JPEG" || extension == ".png" || extension == ".PNG" || extension == ".bmp" || extension == ".BMP" || extension == ".gif" || extension == ".GIF")
            {
                filename = filename + extension;

                string baseRoot = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Uploads" + "\\" + folderName;
                string originalFilePath = baseRoot + "\\" + "original";
                string photoFilePath = baseRoot + "\\" + "photo";
                string thumbFilePath = baseRoot + "\\" + "thumb";

                if (!Directory.Exists(baseRoot))
                {
                    Directory.CreateDirectory(baseRoot);
                }

                if (!Directory.Exists(originalFilePath))
                {
                    Directory.CreateDirectory(originalFilePath);
                }

                if (!Directory.Exists(photoFilePath))
                {
                    Directory.CreateDirectory(photoFilePath);
                }

                if (!Directory.Exists(thumbFilePath))
                {
                    Directory.CreateDirectory(thumbFilePath);
                }

                var originalPath = Path.Combine(originalFilePath, filename);
                file.SaveAs(originalPath);

                if (ThumbImageWH != null)
                {
                    Image origImg = Image.FromFile(Path.Combine(originalFilePath, filename));

                    foreach (var item in ThumbImageWH)
                    {
                        int width = item.Key;
                        int height = item.Value;

                        Image resizedImg = FixedSize(origImg, width, height);
                        resizedImg.Save(Path.Combine(thumbFilePath, filename), ImageFormat.Jpeg);
                        resizedImg.Dispose();
                        origImg.Dispose();
                    }
                }



                if (ImageWH != null)
                {
                    Image origImg = Image.FromFile(Path.Combine(originalFilePath, filename));

                    foreach (var item in ImageWH)
                    {
                        int width = item.Key;
                        int height = item.Value;

                        Image resizedImg = FixedSize(origImg, width, height);
                        resizedImg.Save(Path.Combine(photoFilePath, filename), ImageFormat.Jpeg);
                        resizedImg.Dispose();
                        origImg.Dispose();
                    }

                    if (File.Exists(originalPath))
                    {
                        FileInfo fi = new FileInfo(originalPath);
                        fi.Delete();
                    }
                }

                returnValue = filename;
            }

            return returnValue;
        }

        public static Image FixedSize(Image imgPhoto, int Width, int Height)
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
            if (nPercentH < nPercentW)
            {
                nPercent = nPercentH;
                destX = System.Convert.ToInt16((Width - (sourceWidth * nPercent)) / 2);
            }
            else
            {
                nPercent = nPercentW;
                destY = System.Convert.ToInt16((Height - (sourceHeight * nPercent)) / 2);
            }

            int destWidth = (int)(sourceWidth * nPercent);
            int destHeight = (int)(sourceHeight * nPercent);

            Bitmap bmPhoto = new Bitmap(Width, Height, PixelFormat.Format24bppRgb);
            bmPhoto.SetResolution(imgPhoto.HorizontalResolution, imgPhoto.VerticalResolution);

            Graphics grPhoto = Graphics.FromImage(bmPhoto);
            grPhoto.Clear(Color.White);
            grPhoto.InterpolationMode = InterpolationMode.HighQualityBicubic;
            grPhoto.DrawImage(imgPhoto, new Rectangle(destX, destY, destWidth, destHeight), new Rectangle(sourceX,
            sourceY, sourceWidth, sourceHeight), GraphicsUnit.Pixel);
            grPhoto.Dispose();

            return bmPhoto;
        }


        public static void DeleteImage(string folderName, string fileName)
        {
            string baseRoot = AppDomain.CurrentDomain.BaseDirectory.ToString() + "Uploads\\" + folderName;
            string originalFilePath = baseRoot + "\\" + "original\\" + fileName;
            string photoFilePath = baseRoot + "\\" + "photo\\" + fileName;
            string thumbFilePath = baseRoot + "\\" + "thumb\\" + fileName;

            if (File.Exists(originalFilePath))
            {
                FileInfo fi = new FileInfo(originalFilePath);
                fi.Delete();
            }

            if (File.Exists(photoFilePath))
            {
                FileInfo fi = new FileInfo(photoFilePath);
                fi.Delete();
            }

            if (File.Exists(thumbFilePath))
            {
                FileInfo fi = new FileInfo(thumbFilePath);
                fi.Delete();
            }
        }
    }
}