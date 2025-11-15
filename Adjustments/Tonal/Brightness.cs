using System.Windows;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEditor.Adjustments.Tonal;

public static class Brightness
{
    public static async Task<WriteableBitmap> AdjustBrightness(ImageSource source, double brightness)
    {
        // Convert the ImageSource to a WriteableBitmap
        WriteableBitmap sourceBitmap = new WriteableBitmap((BitmapSource)source);

        // Create a new WriteableBitmap for the adjusted image
        WriteableBitmap adjustedBitmap = new WriteableBitmap(sourceBitmap.PixelWidth, sourceBitmap.PixelHeight, sourceBitmap.DpiX, sourceBitmap.DpiY, sourceBitmap.Format, null);

        // Calculate brightness factor
        double brightnessFactor = (brightness + 100) / 100.0;

        // Create a buffer to hold pixel data
        int bytesPerPixel = (sourceBitmap.Format.BitsPerPixel + 7) / 8;
        int stride = sourceBitmap.PixelWidth * bytesPerPixel;
        byte[] pixelData = new byte[stride * sourceBitmap.PixelHeight];
        sourceBitmap.CopyPixels(pixelData, stride, 0);

        // Adjust pixel brightness
        for (int i = 0; i < pixelData.Length; i += bytesPerPixel)
        {
            // Apply brightness adjustment to each color component
            pixelData[i] = AdjustBrightnessComponent(pixelData[i], brightnessFactor);       // Blue
            pixelData[i + 1] = AdjustBrightnessComponent(pixelData[i + 1], brightnessFactor); // Green
            pixelData[i + 2] = AdjustBrightnessComponent(pixelData[i + 2], brightnessFactor); // Red
        }

        // Update UI on the UI thread
        await Application.Current.Dispatcher.InvokeAsync(() =>
        {
            Int32Rect adjustedRect = new Int32Rect(0, 0, adjustedBitmap.PixelWidth, adjustedBitmap.PixelHeight);
            adjustedBitmap.WritePixels(adjustedRect, pixelData, stride, 0);
        });

        return adjustedBitmap;
    }

    private static byte AdjustBrightnessComponent(byte component, double brightnessFactor)
    {
        // Adjust brightness of a single color component
        double adjustedComponent = component * brightnessFactor;
        adjustedComponent = Math.Max(0, Math.Min(255, adjustedComponent));
        return (byte)adjustedComponent;
    }
}
