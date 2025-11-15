using System.Windows.Media.Imaging;

namespace ImageEditor.Adjustments.Tonal;

public static class Shadow
{
    public static async Task<WriteableBitmap> AdjustShadows(WriteableBitmap originalImage, double shadowsValue)
    {
        int width = originalImage.PixelWidth;
        int height = originalImage.PixelHeight;

        // Calculate shadow adjustment factor
        double shadowFactor = (100 + shadowsValue) / 100;

        // Create a new WriteableBitmap with the same properties as the original image
        WriteableBitmap adjustedImage = new WriteableBitmap(width, height, originalImage.DpiX, originalImage.DpiY, originalImage.Format, originalImage.Palette);

        // Calculate stride (width of a single row of pixels in bytes)
        int stride = width * ((originalImage.Format.BitsPerPixel + 7) / 8);

        // Create byte array to hold pixel data
        byte[] pixelBuffer = new byte[stride * height];

        // Copy pixel data from the original image to the buffer
        originalImage.CopyPixels(pixelBuffer, stride, 0);

        // Adjust pixel intensity for each channel (RGBA) separately
        for (int i = 0; i < pixelBuffer.Length; i += 4)
        {
            byte red = pixelBuffer[i];
            byte green = pixelBuffer[i + 1];
            byte blue = pixelBuffer[i + 2];

            // Adjust shadow intensity for each channel
            red = AdjustChannel(red, shadowFactor);
            green = AdjustChannel(green, shadowFactor);
            blue = AdjustChannel(blue, shadowFactor);

            // Update pixel values in the buffer
            pixelBuffer[i] = red;
            pixelBuffer[i + 1] = green;
            pixelBuffer[i + 2] = blue;
        }

        // Write modified pixel data back to the adjusted image
        adjustedImage.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixelBuffer, stride, 0);

        return adjustedImage;
    }

    private static byte AdjustChannel(byte originalValue, double shadowFactor)
    {
        // Adjust pixel intensity for a single channel
        double adjustedValue = originalValue * shadowFactor;
        return (byte)Math.Min(255, adjustedValue);
    }
}
