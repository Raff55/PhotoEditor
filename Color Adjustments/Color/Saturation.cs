using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageEditor.Color
{
    public static class Saturation
    {
        public static async Task<WriteableBitmap> AdjustSaturation(WriteableBitmap source, double saturationValue)
        {
            // Create a new WriteableBitmap based on the source image
            WriteableBitmap writableBitmap = new WriteableBitmap(source);

            // Get the pixel buffer of the writable bitmap
            int width = writableBitmap.PixelWidth;
            int height = writableBitmap.PixelHeight;
            int stride = width * 4; // 4 bytes per pixel (ARGB)
            int pixelCount = width * height;
            byte[] pixels = new byte[pixelCount * 4];
            writableBitmap.CopyPixels(pixels, stride, 0);

            // Adjust the saturation of each pixel in the image
            for (int i = 0; i < pixelCount; i++)
            {
                // Get the starting index of the current pixel
                int index = i * 4;

                // Get the current pixel values (ARGB)
                byte alpha = pixels[index + 3];
                byte red = pixels[index + 2];
                byte green = pixels[index + 1];
                byte blue = pixels[index];

                // Convert RGB to HSL (Hue, Saturation, Lightness)
                double h, s, l;
                ColorUtils.RgbToHsl(red, green, blue, out h, out s, out l);

                // Adjust the saturation value
                s += saturationValue;

                // Clamp the saturation value within the valid range (0-1)
                s = Math.Max(0, Math.Min(1, s));

                // Convert HSL back to RGB
                ColorUtils.HslToRgb(h, s, l, out red, out green, out blue);

                // Update the pixel values in the writable bitmap
                pixels[index + 3] = alpha;
                pixels[index + 2] = red;
                pixels[index + 1] = green;
                pixels[index] = blue;
            }

            // Update the writable bitmap with the adjusted pixels
            writableBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            // Return the updated WriteableBitmap
            return writableBitmap;
        }
    }
}
