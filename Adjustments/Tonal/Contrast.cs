using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageEditor.Adjustments.Tonal;

public static class Contrast
{
    public static async Task<WriteableBitmap> ApplyContrastFilter(BitmapSource source, double contrastValue)
    {
        // Create a new WriteableBitmap with the same parameters as the source image
        WriteableBitmap result = new WriteableBitmap(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format, source.Palette);

        // Calculate the stride (bytes per row) of the source image
        int stride = (source.PixelWidth * source.Format.BitsPerPixel + 7) / 8;

        // Create a byte array buffer with enough capacity to hold the pixel data
        byte[] pixelBuffer = new byte[stride * source.PixelHeight];

        // Copy the pixels from the source image to the buffer
        source.CopyPixels(new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight), pixelBuffer, stride, 0);

        // Calculate the contrast adjustment factor
        double contrastFactor = (100 + contrastValue) / 100;

        for (int i = 0; i < pixelBuffer.Length - 2; i += source.Format.BitsPerPixel / 8)
        {
            // Adjust the contrast of each color channel separately
            pixelBuffer[i] = AdjustContrast(pixelBuffer[i], contrastFactor);         // Blue channel
            pixelBuffer[i + 1] = AdjustContrast(pixelBuffer[i + 1], contrastFactor); // Green channel
            pixelBuffer[i + 2] = AdjustContrast(pixelBuffer[i + 2], contrastFactor); // Red channel
        }

        // Write the modified pixel values from the buffer to the result image
        result.WritePixels(new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight), pixelBuffer, stride, 0);

        return result;
    }

    private static byte AdjustContrast(byte value, double contrastFactor)
    {
        // Ensure contrastFactor is greater than zero
        if (contrastFactor <= 0)
            contrastFactor = 0.01;

        // Adjust the contrast of a single color channel
        double adjustedValue = (value / 255.0 - 0.5) * contrastFactor + 0.5;
        adjustedValue = Math.Min(Math.Max(adjustedValue, 0), 1);
        return (byte)(adjustedValue * 255);
    }
}
