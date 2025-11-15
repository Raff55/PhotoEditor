using System.Windows.Media.Imaging;

namespace ImageEditor.Adjustments.Tonal;

public static class Highlight
{
    public static async Task<WriteableBitmap> ApplyHighlightFilter(BitmapSource image, double highlightValue)
    {
        int width = image.PixelWidth;
        int height = image.PixelHeight;

        // Create a new WriteableBitmap with the same properties as the original image
        WriteableBitmap bitmap = new WriteableBitmap(image);

        // Calculate the stride (width of a single row of pixels in bytes)
        int stride = (width * bitmap.Format.BitsPerPixel + 7) / 8;

        // Create a byte array buffer to hold the modified pixel data
        byte[] pixelBuffer = new byte[stride * height];

        // Copy the original pixel data to the buffer
        image.CopyPixels(pixelBuffer, stride, 0);

        // Adjust the highlight of each pixel in the buffer
        for (int i = 0; i < pixelBuffer.Length; i += 4)
        {
            // Extract color components from the buffer
            byte blue = pixelBuffer[i];
            byte green = pixelBuffer[i + 1];
            byte red = pixelBuffer[i + 2];
            byte alpha = pixelBuffer[i + 3];

            // Adjust the highlight/brightness of the color
            byte newRed = AdjustHighlight(red, highlightValue);
            byte newGreen = AdjustHighlight(green, highlightValue);
            byte newBlue = AdjustHighlight(blue, highlightValue);

            // Write the modified color values back to the buffer
            pixelBuffer[i] = newBlue;
            pixelBuffer[i + 1] = newGreen;
            pixelBuffer[i + 2] = newRed;
        }

        // Write the modified pixel data from the buffer to the bitmap
        bitmap.WritePixels(new System.Windows.Int32Rect(0, 0, width, height), pixelBuffer, stride, 0);

        return bitmap;
    }

    private static byte AdjustHighlight(byte originalValue, double highlightValue)
    {
        // Adjust the brightness/highlight value based on the highlightValue parameter
        double adjustedValue = originalValue + highlightValue * 2.55;

        // Ensure the adjusted value is within the valid byte range (0-255)
        adjustedValue = Math.Max(0, Math.Min(255, adjustedValue));

        // Convert the adjusted value back to a byte
        return (byte)adjustedValue;
    }
}
