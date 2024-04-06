using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageEditor.Color
{
    public static class Temperature
    {
        public static async Task<WriteableBitmap> AdjustTemperature(BitmapSource sourceImage, double temperatureValue)
        {
            // Convert the image to a writable bitmap
            WriteableBitmap bitmap = new WriteableBitmap(sourceImage);

            // Get the pixel buffer of the writable bitmap
            byte[] pixels = new byte[4 * bitmap.PixelWidth * bitmap.PixelHeight];
            bitmap.CopyPixels(pixels, 4 * bitmap.PixelWidth, 0);

            // Adjust the temperature of each pixel
            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte blue = pixels[i];
                byte green = pixels[i + 1];
                byte red = pixels[i + 2];

                // Adjust the color temperature
                red = AdjustColor(red, temperatureValue);
                blue = AdjustColor(blue, -temperatureValue);

                // Update the pixel values
                pixels[i] = blue;
                pixels[i + 1] = green;
                pixels[i + 2] = red;
            }

            // Create a new bitmap with the adjusted pixels
            WriteableBitmap adjustedBitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight, bitmap.DpiX, bitmap.DpiY, PixelFormats.Bgr32, null);
            adjustedBitmap.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), pixels, 4 * bitmap.PixelWidth, 0);

            return adjustedBitmap;
        }

        private static byte AdjustColor(byte color, double temperatureValue)
        {
            // Calculate the color adjustment based on the temperature value
            double adjustment = temperatureValue * 255 / 100;

            // Apply the adjustment to the color
            int adjustedColor = color + (int)adjustment;

            // Ensure the adjusted color is within the valid range of 0-255
            adjustedColor = Math.Min(Math.Max(adjustedColor, 0), 255);

            return (byte)adjustedColor;
        }
    }
}