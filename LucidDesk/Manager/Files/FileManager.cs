using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace LucidDesk.Manager.Files
{
    public static class FileManager
    {
        public static string ImageToString(string FilePath)
        {
            byte[] imageBytes = ResizeImage(FilePath, 60000);
            string imageString = Convert.ToBase64String(imageBytes);
            return imageString;
        }
        public static string ImageToString(BitmapImage Image)
        {
            if (Image == null)
                return "";
            byte[] imageBytes = ResizeImage(Image, 60000);
            string imageString = Convert.ToBase64String(imageBytes);
            return imageString;
        }

        public static BitmapImage ConvertBase64ToBitmapImage(string base64String)
        {
            BitmapImage bitmapImage = new BitmapImage();

            try
            {
                byte[] imageBytes = Convert.FromBase64String(base64String);


                using (MemoryStream memoryStream = new MemoryStream(imageBytes))
                {
                    bitmapImage.BeginInit();
                    bitmapImage.CacheOption = BitmapCacheOption.OnLoad; // Load the bitmapImage immediately
                    bitmapImage.StreamSource = memoryStream;
                    bitmapImage.EndInit();
                }
            }
            catch
            {
                return null;
            }

            return bitmapImage;
        }
        private static byte[] ResizeImage(string filePath, int maxFileSizeInBytes)
        {
            BitmapImage bitmap = new BitmapImage(new Uri(filePath));
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;

            double scale = Math.Sqrt(maxFileSizeInBytes / (double)new FileInfo(filePath).Length);
            // No need to resize if already small enough
            if (scale >= 1)
                return File.ReadAllBytes(filePath);
            int newWidth = (int)(width * scale);
            int newHeight = (int)(height * scale);
            BitmapSource resizedBitmap = new TransformedBitmap(bitmap, new System.Windows.Media.ScaleTransform(scale, scale));

            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder encoder = new JpegBitmapEncoder(); // You can change to PngBitmapEncoder for PNG
                encoder.Frames.Add(BitmapFrame.Create(resizedBitmap));
                encoder.Save(ms);
                return ms.ToArray();
            }
        }
        public static byte[] ResizeImage(BitmapImage bitmap, int maxFileSizeInBytes)
        {
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;

            // Calculate scale based on desired file size and current dimensions
            double scale = Math.Sqrt(maxFileSizeInBytes / (double)(width * height * 4)); // Assuming 4 bytes per pixel

            // No need to resize if already small enough
            if (scale >= 1)
            {
                using (MemoryStream ms = new MemoryStream())
                {
                    BitmapEncoder encoder = new JpegBitmapEncoder(); // You can change to PngBitmapEncoder for PNG
                    encoder.Frames.Add(BitmapFrame.Create(bitmap));
                    encoder.Save(ms);
                    return ms.ToArray();
                }
            }

            int newWidth = (int)(width * scale);
            int newHeight = (int)(height * scale);

            // Create the transformed (resized) bitmap
            TransformedBitmap resizedBitmap = new TransformedBitmap(bitmap, new System.Windows.Media.ScaleTransform(scale, scale));

            using (MemoryStream ms = new MemoryStream())
            {
                BitmapEncoder encoder = new JpegBitmapEncoder(); // You can change to PngBitmapEncoder for PNG
                encoder.Frames.Add(BitmapFrame.Create(resizedBitmap));
                encoder.Save(ms);
                return ms.ToArray();
            }
        }
        public static BitmapImage ConvertBitmapToBitmapImage(System.Drawing.Bitmap bitmap)
        {
            BitmapImage bitmapImage = new BitmapImage();

            using (System.IO.MemoryStream memoryStream = new System.IO.MemoryStream())
            {
                // Save the Bitmap to the MemoryStream
                bitmap.Save(memoryStream, System.Drawing.Imaging.ImageFormat.Png);

                // Rewind the stream ready to read from it later
                memoryStream.Seek(0, System.IO.SeekOrigin.Begin);

                // Tell the BitmapImage to use this stream
                bitmapImage.BeginInit();
                bitmapImage.StreamSource = memoryStream;
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.EndInit();
            }

            return bitmapImage;
        }
    }
}
