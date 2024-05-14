using System;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageEditor.Functionals
{
    public static class Sharpen
    {
        public static async Task<WriteableBitmap> AdjustSharpen(WriteableBitmap originalImage, double sharpenValue)
        {
            // Convert the WriteableBitmap to a BitmapSource for editing
            BitmapSource bitmapSource = originalImage;

            // Create a new WriteableBitmap with the same dimensions and properties as the original image
            WriteableBitmap sharpenedBitmap = new WriteableBitmap(
                bitmapSource.PixelWidth, bitmapSource.PixelHeight,
                bitmapSource.DpiX, bitmapSource.DpiY,
                bitmapSource.Format, bitmapSource.Palette);

            // Lock the bitmap to manipulate the pixel data
            sharpenedBitmap.Lock();

            // Copy the pixel data from the original image to an array
            int width = sharpenedBitmap.PixelWidth;
            int height = sharpenedBitmap.PixelHeight;
            int stride = sharpenedBitmap.BackBufferStride;
            byte[] pixels = new byte[height * stride];
            bitmapSource.CopyPixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            // Apply sharpening to each pixel
            for (int y = 1; y < height - 1; y++)
            {
                for (int x = 1; x < width - 1; x++)
                {
                    int index = y * stride + 4 * x;

                    // Get the color components of the pixel and its neighbors
                    byte alpha = pixels[index + 3];
                    byte red = pixels[index + 2];
                    byte green = pixels[index + 1];
                    byte blue = pixels[index];

                    byte newRed = (byte)(CalculateSharpenedComponent(pixels, width, height, stride, x, y, red, sharpenValue));
                    byte newGreen = (byte)(CalculateSharpenedComponent(pixels, width, height, stride, x, y, green, sharpenValue));
                    byte newBlue = (byte)(CalculateSharpenedComponent(pixels, width, height, stride, x, y, blue, sharpenValue));

                    // Update the pixel with the sharpened color components
                    pixels[index + 2] = newRed;
                    pixels[index + 1] = newGreen;
                    pixels[index] = newBlue;
                }
            }

            // Write the modified pixel data back to the bitmap
            sharpenedBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            // Unlock the bitmap
            sharpenedBitmap.Unlock();

            return sharpenedBitmap;
        }

        private static double CalculateSharpenedComponent(byte[] pixels, int width, int height, int stride, int x, int y, byte component, double sharpenValue)
        {
            double sum = 5 * component;
            sum -= pixels[(y - 1) * stride + 4 * (x - 1) + 2];
            sum -= pixels[(y - 1) * stride + 4 * x + 2];
            sum -= pixels[(y - 1) * stride + 4 * (x + 1) + 2];
            sum -= pixels[y * stride + 4 * (x - 1) + 2];
            sum -= pixels[y * stride + 4 * (x + 1) + 2];
            sum -= pixels[(y + 1) * stride + 4 * (x - 1) + 2];
            sum -= pixels[(y + 1) * stride + 4 * x + 2];
            sum -= pixels[(y + 1) * stride + 4 * (x + 1) + 2];

            return Clamp(component + sharpenValue * (component - sum / 9), 0, 255);
        }

        private static byte Clamp(double value, int min, int max)
        {
            return (byte)(value < min ? min : (value > max ? max : value));
        }
    }
}
