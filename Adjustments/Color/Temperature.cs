using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEditor.Adjustments.Color;

public static class Temperature
{
    public static async Task<WriteableBitmap> AdjustTemperature(BitmapSource sourceImage, double temperatureValue)
    {
        // Convert the image to a writable bitmap
        WriteableBitmap bitmap = new WriteableBitmap(sourceImage);

        // Get the pixel buffer of the writable bitmap
        int width = bitmap.PixelWidth;
        int height = bitmap.PixelHeight;
        int stride = width * 4; // 4 bytes per pixel (BGR32 format)
        int pixelCount = width * height;
        byte[] pixels = new byte[pixelCount * 4];
        bitmap.CopyPixels(pixels, stride, 0);

        // Calculate the color adjustment based on the temperature value
        double adjustment = temperatureValue * 255 / 100;

        // Adjust the temperature of each pixel
        for (int i = 0; i < pixelCount; i++)
        {
            // Calculate the starting index of the current pixel
            int index = i * 4;

            // Adjust the blue channel
            int adjustedBlue = pixels[index] + (int)(-temperatureValue * 255 / 100);
            pixels[index] = (byte)Math.Min(Math.Max(adjustedBlue, 0), 255);

            // Adjust the red channel
            int adjustedRed = pixels[index + 2] + (int)(temperatureValue * 255 / 100);
            pixels[index + 2] = (byte)Math.Min(Math.Max(adjustedRed, 0), 255);
        }

        // Create a new bitmap with the adjusted pixels
        WriteableBitmap adjustedBitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight, bitmap.DpiX, bitmap.DpiY, PixelFormats.Bgr32, null);
        adjustedBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

        return adjustedBitmap;
    }
}
