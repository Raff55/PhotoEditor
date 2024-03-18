using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;

namespace ImageEditor.Color_Adjustments
{
    public static class Vibrance
    {
        public static BitmapSource AdjustVibrance(BitmapSource source, double vibranceValue)
        {
            // Convert the BitmapSource to a WriteableBitmap for editing
            WriteableBitmap bitmap = new WriteableBitmap(source);

            int stride = (bitmap.PixelWidth * bitmap.Format.BitsPerPixel + 7) / 8;
            int size = stride * bitmap.PixelHeight;
            byte[] pixels = new byte[size];
            bitmap.CopyPixels(pixels, stride, 0);

            // Iterate through each pixel and adjust the vibrance
            for (int i = 0; i < pixels.Length; i += 4)
            {
                byte blue = pixels[i];
                byte green = pixels[i + 1];
                byte red = pixels[i + 2];

                // Convert RGB to HSV
                double h, s, v;
                RGBToHSV(red, green, blue, out h, out s, out v);

                // Adjust the vibrance value
                v = v + (vibranceValue / 100);

                // Clamp the value to the valid HSV range
                v = Math.Max(0, Math.Min(1, v));

                // Convert HSV back to RGB
                HSVToRGB(h, s, v, out red, out green, out blue);

                // Update the pixel values
                pixels[i] = blue;
                pixels[i + 1] = green;
                pixels[i + 2] = red;
            }

            // Create a new BitmapSource with the adjusted pixels
            WriteableBitmap adjustedBitmap = new WriteableBitmap(bitmap.PixelWidth, bitmap.PixelHeight, bitmap.DpiX, bitmap.DpiY, bitmap.Format, bitmap.Palette);
            adjustedBitmap.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), pixels, stride, 0);

            return adjustedBitmap;
        }

        private static void RGBToHSV(byte red, byte green, byte blue, out double h, out double s, out double v)
        {
            double r = red / 255.0;
            double g = green / 255.0;
            double b = blue / 255.0;

            double max = Math.Max(r, Math.Max(g, b));
            double min = Math.Min(r, Math.Min(g, b));
            double delta = max - min;

            // Calculate hue
            if (delta == 0)
                h = 0;
            else if (max == r)
                h = ((g - b) / delta) % 6;
            else if (max == g)
                h = ((b - r) / delta) + 2;
            else
                h = ((r - g) / delta) + 4;

            h *= 60;

            // Calculate saturation
            if (max == 0)
                s = 0;
            else
                s = delta / max;

            // Calculate value
            v = max;
        }

        private static void HSVToRGB(double h, double s, double v, out byte red, out byte green, out byte blue)
        {
            double c = v * s;
            double x = c * (1 - Math.Abs((h / 60) % 2 - 1));
            double m = v - c;

            double r, g, b;
            if (h >= 0 && h < 60)
            {
                r = c;
                g = x;
                b = 0;
            }
            else if (h >= 60 && h < 120)
            {
                r = x;
                g = c;
                b = 0;
            }
            else if (h >= 120 && h < 180)
            {
                r = 0;
                g = c;
                b = x;
            }
            else if (h >= 180 && h < 240)
            {
                r = 0;
                g = x;
                b = c;
            }
            else if (h >= 240 && h < 300)
            {
                r = x;
                g = 0;
                b = c;
            }
            else
            {
                r = c;
                g = 0;
                b = x;
            }

            red = (byte)((r + m) * 255);
            green = (byte)((g + m) * 255);
            blue = (byte)((b + m) * 255);
        }
    }
}
