using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageEditor.Exposure
{
    public static class Highlight
    {
        public static WriteableBitmap ApplyHighlightFilter(BitmapSource image, double highlightValue)
        {
            int width = image.PixelWidth;
            int height = image.PixelHeight;

            // Create a new WriteableBitmap with the same properties as the original image
            WriteableBitmap bitmap = new WriteableBitmap(image);

            // Lock the bitmap to manipulate the pixel data
            bitmap.Lock();

            // Get the address of the first pixel
            nint backBuffer = bitmap.BackBuffer;

            // Get the stride (width of a single row of pixels in bytes)
            int stride = bitmap.BackBufferStride;

            // Iterate over each row and column of pixels
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    // Compute the address of the current pixel
                    nint pixelAddress = backBuffer + y * stride + x * 4;

                    // Read the color values of the pixel
                    byte blue = System.Runtime.InteropServices.Marshal.ReadByte(pixelAddress);
                    byte green = System.Runtime.InteropServices.Marshal.ReadByte(pixelAddress + 1);
                    byte red = System.Runtime.InteropServices.Marshal.ReadByte(pixelAddress + 2);
                    byte alpha = System.Runtime.InteropServices.Marshal.ReadByte(pixelAddress + 3);

                    // Adjust the highlight/brightness of the color
                    byte newRed = AdjustHighlight(red, highlightValue);
                    byte newGreen = AdjustHighlight(green, highlightValue);
                    byte newBlue = AdjustHighlight(blue, highlightValue);

                    // Write the modified color values back to the pixel
                    System.Runtime.InteropServices.Marshal.WriteByte(pixelAddress, newBlue);
                    System.Runtime.InteropServices.Marshal.WriteByte(pixelAddress + 1, newGreen);
                    System.Runtime.InteropServices.Marshal.WriteByte(pixelAddress + 2, newRed);
                    System.Runtime.InteropServices.Marshal.WriteByte(pixelAddress + 3, alpha);
                }
            }

            // Unlock the bitmap to finish the manipulation
            bitmap.Unlock();

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
}
