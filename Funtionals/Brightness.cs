using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;

namespace ImageEditor.Funtionals
{
    public static class Brightness
    {
        public static BitmapSource AdjustBrightness(BitmapSource source, double brightness)
        {
            WriteableBitmap adjustedBitmap = new WriteableBitmap(source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, source.Format, null);

            Int32Rect sourceRect = new Int32Rect(0, 0, source.PixelWidth, source.PixelHeight);
            byte[] pixelData = new byte[source.PixelWidth * source.PixelHeight * (source.Format.BitsPerPixel / 8)];
            source.CopyPixels(sourceRect, pixelData, source.PixelWidth * (source.Format.BitsPerPixel / 8), 0);

            double brightnessFactor = (brightness + 100) / 100.0;
            for (int i = 0; i < pixelData.Length; i += (source.Format.BitsPerPixel / 8))
            {
                byte blue = pixelData[i];
                byte green = pixelData[i + 1];
                byte red = pixelData[i + 2];

                blue = AdjustBrightnessComponent(blue, brightnessFactor);
                green = AdjustBrightnessComponent(green, brightnessFactor);
                red = AdjustBrightnessComponent(red, brightnessFactor);

                pixelData[i] = blue;
                pixelData[i + 1] = green;
                pixelData[i + 2] = red;
            }

            Int32Rect adjustedRect = new Int32Rect(0, 0, adjustedBitmap.PixelWidth, adjustedBitmap.PixelHeight);
            adjustedBitmap.WritePixels(adjustedRect, pixelData, adjustedBitmap.PixelWidth * (adjustedBitmap.Format.BitsPerPixel / 8), 0);

            return adjustedBitmap;
        }

        private static byte AdjustBrightnessComponent(byte component, double brightnessFactor)
        {
            double adjustedComponent = component * brightnessFactor;
            adjustedComponent = Math.Max(0, Math.Min(255, adjustedComponent));
            return (byte)adjustedComponent;
        }
    }
}
