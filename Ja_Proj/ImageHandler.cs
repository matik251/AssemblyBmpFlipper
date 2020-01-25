using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace Ja_Proj
{
    public static class ImageHandler
    {
        //todo split image due to pixelcount

        public static BitmapSource BitmapToBitmapSource(Bitmap source)
        {
            return System.Windows.Interop.Imaging.CreateBitmapSourceFromHBitmap(
                          source.GetHbitmap(),
                          IntPtr.Zero,
                          Int32Rect.Empty,
                          BitmapSizeOptions.FromEmptyOptions());
        }

        public static BitmapSource LoadBMP(string UserImagePath)
        {
            try
            {
                Bitmap bitmap = (Bitmap)Bitmap.FromFile(UserImagePath, true);
                return BitmapToBitmapSource(bitmap);
            }
            catch (Exception e)
            {
                return null;
            }
        }

        public static Image LoadImage(string UserImagePAth)
        {
            try
            {
                Image image1 = Image.FromFile(UserImagePAth);
                return image1;

            }
            catch(Exception e)
            {
                MessageBox.Show(e.Message);
            }
            return null;
        }

        public static void SaveImage(Image img, string UserImagePAth)
        {
            try
            {
                img.Save(UserImagePAth, ImageFormat.Bmp);
                img.Dispose();
            }
            catch 
            {

            }
        }

       
        public static byte[] ToByteArray(this Image image, ImageFormat format)
        {
            using (MemoryStream ms = new MemoryStream())
            {
                image.Save(ms, format);
                return ms.ToArray();
            }
        }

        public static string ByteArrayToBitmap(byte[] bytesArr)
        {
            var name = DateTime.Now.ToString("MM_dd_yyyy_HH_mm_ss");
            using (MemoryStream memstr = new MemoryStream(bytesArr))
            {
                Image img = Image.FromStream(memstr);
                SaveImage(img, $"{name}.bmp");
            }
            return name + ".bmp";
        }


        public static Bitmap BitmapFromSource(BitmapSource bitmapsource)
        {
            System.Drawing.Bitmap bitmap;
            using (MemoryStream outStream = new MemoryStream())
            {
                BitmapEncoder enc = new BmpBitmapEncoder();
                enc.Frames.Add(BitmapFrame.Create(bitmapsource));
                enc.Save(outStream);
                bitmap = new System.Drawing.Bitmap(outStream);
            }
            return bitmap;
        }


    }
}
