using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;

namespace ImageEditor.Funtionals
{
    public static class Sharpen
    {
        public static BitmapSource AdjustSharpen(WriteableBitmap originalImage, double sharpenValue)
        {
            // Convert the BitmapImage to a WriteableBitmap for editing
            WriteableBitmap writeableBitmap = new WriteableBitmap(originalImage);

            int width = writeableBitmap.PixelWidth;
            int height = writeableBitmap.PixelHeight;

            // Copy the pixel data from the original image
            int[] pixels = new int[width * height];
            writeableBitmap.CopyPixels(pixels, width * 4, 0);

            // Calculate the sharpening factor based on sharpenValue
            double factor = 0.4 * sharpenValue;

            // Apply sharpening to each pixel
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * width + x;

                    // Get the color components of the pixel
                    byte alpha = (byte)(pixels[index] >> 24);
                    byte red = (byte)(pixels[index] >> 16);
                    byte green = (byte)(pixels[index] >> 8);
                    byte blue = (byte)pixels[index];

                    // Calculate the sharpened color components
                    int newRed = (int)(red + factor * (red - CalculateAverageNeighbor(pixels, width, height, x, y, ColorComponent.Red)));
                    int newGreen = (int)(green + factor * (green - CalculateAverageNeighbor(pixels, width, height, x, y, ColorComponent.Green)));
                    int newBlue = (int)(blue + factor * (blue - CalculateAverageNeighbor(pixels, width, height, x, y, ColorComponent.Blue)));

                    // Clamp the color values to the valid range (0-255)
                    newRed = Clamp(newRed, 0, 255);
                    newGreen = Clamp(newGreen, 0, 255);
                    newBlue = Clamp(newBlue, 0, 255);

                    // Update the pixel with the sharpened color components
                    pixels[index] = (alpha << 24) | (newRed << 16) | (newGreen << 8) | newBlue;
                }
            }

            // Create a new WriteableBitmap with the modified pixel data
            WriteableBitmap sharpenedBitmap = new WriteableBitmap(width, height, originalImage.DpiX, originalImage.DpiY, originalImage.Format, null);
            sharpenedBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, width * 4, 0);

            return sharpenedBitmap;
        }

        private static byte CalculateAverageNeighbor(int[] pixels, int width, int height, int x, int y, ColorComponent component)
        {
            int sum = 0;
            int count = 0;

            for (int j = y - 1; j <= y + 1; j++)
            {
                for (int i = x - 1; i <= x + 1; i++)
                {
                    if (i >= 0 && i < width && j >= 0 && j < height)
                    {
                        int index = j * width + i;
                        byte colorComponent = 0;

                        switch (component)
                        {
                            case ColorComponent.Red:
                                colorComponent = (byte)(pixels[index] >> 16);
                                break;
                            case ColorComponent.Green:
                                colorComponent = (byte)(pixels[index] >> 8);
                                break;
                            case ColorComponent.Blue:
                                colorComponent = (byte)pixels[index];
                                break;
                        }

                        sum += colorComponent;
                        count++;
                    }
                }
            }

            return (byte)(sum / count);
        }

        private static int Clamp(int value, int min, int max)
        {
            return value < min ? min : (value > max ? max : value);
        }

        private enum ColorComponent
        {
            Red,
            Green,
            Blue
        }
    }
}
