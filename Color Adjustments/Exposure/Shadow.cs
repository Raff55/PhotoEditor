using System;
using System.Collections.Generic;
using System.Drawing.Imaging;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageEditor.Exposure
{
    public static class Shadow
    {
        public static async Task<WriteableBitmap> AdjustShadows(WriteableBitmap originalImage, double shadowsValue)
        {
            Bitmap bitmap = ConvertToBitmap(originalImage);

            // Apply shadow adjustment to the bitmap
            int width = bitmap.Width;
            int height = bitmap.Height;
            Bitmap adjustedBitmap = new Bitmap(width, height);

            using (Graphics graphics = Graphics.FromImage(adjustedBitmap))
            {
                // Create an ImageAttributes object for adjusting image attributes
                ImageAttributes imageAttributes = new ImageAttributes();

                // Set the shadows value in the ColorMatrix
                float shadowValue = (float)(shadowsValue / 100.0);
                ColorMatrix colorMatrix = new ColorMatrix(
                [
                    [1, 0, 0, 0, 0],
                    [0, 1, 0, 0, 0],
                    [0, 0, 1, 0, 0],
                    [0, 0, 0, 1, 0],
                    [shadowValue, shadowValue, shadowValue, 0, 1]
                ]);

                imageAttributes.SetColorMatrix(colorMatrix);

                // Draw the original bitmap onto the adjusted bitmap using the image attributes
                graphics.DrawImage(bitmap, new Rectangle(0, 0, width, height), 0, 0, width, height, GraphicsUnit.Pixel, imageAttributes);
            }

            return ConvertToBitmapSource(adjustedBitmap);
        }

        private static Bitmap ConvertToBitmap(WriteableBitmap bitmapImage)
        {
            Bitmap bitmap;
            using (MemoryStream stream = new MemoryStream())
            {
                BitmapEncoder encoder = new BmpBitmapEncoder();
                encoder.Frames.Add(BitmapFrame.Create(bitmapImage));
                encoder.Save(stream);
                bitmap = new Bitmap(stream);
            }
            return bitmap;
        }

        private static WriteableBitmap ConvertToBitmapSource(Bitmap bitmap)
        {
            WriteableBitmap writableBitmap;
            using (MemoryStream stream = new MemoryStream())
            {
                bitmap.Save(stream, ImageFormat.Png);
                stream.Position = 0;
                BitmapImage bitmapImage = new BitmapImage();
                bitmapImage.BeginInit();
                bitmapImage.CacheOption = BitmapCacheOption.OnLoad;
                bitmapImage.StreamSource = stream;
                bitmapImage.EndInit();
                writableBitmap = new WriteableBitmap(bitmapImage);
            }
            return writableBitmap;
        }
    }
}
