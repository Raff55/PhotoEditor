using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageEditor.Tools
{
    public static class Brush
    {
        public static WriteableBitmap Draw (WriteableBitmap bitmap, System.Windows.Point currentPosition, double brushSize, System.Windows.Media.Brush currentBrush, double actualWidth, double actualHeight)
        {
            double brushSizeValue = brushSize;

            // Calculate the position in the bitmap
            int x = (int)(currentPosition.X * (bitmap.PixelWidth / actualWidth));
            int y = (int)(currentPosition.Y * (bitmap.PixelHeight / actualHeight));

            // Get the bitmap's pixel data
            int bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
            int stride = bytesPerPixel * bitmap.PixelWidth;
            byte[] pixelData = new byte[stride * bitmap.PixelHeight];
            bitmap.CopyPixels(pixelData, stride, 0);

            // Set brush size
            int brushSizeHalf = (int)(brushSizeValue / 2);

            // Cast the brush to a SolidColorBrush to access its color
            SolidColorBrush solidColorBrush = currentBrush as SolidColorBrush;
            //if (solidColorBrush == null)
            //    return new WriteableBitmap(); // Handle if the brush is not a SolidColorBrush

            // Get the color
            System.Windows.Media.Color brushColor = solidColorBrush.Color;

            // Draw on the pixel data
            for (int i = x - brushSizeHalf; i <= x + brushSizeHalf; i++)
            {
                for (int j = y - brushSizeHalf; j <= y + brushSizeHalf; j++)
                {
                    if (i >= 0 && i < bitmap.PixelWidth && j >= 0 && j < bitmap.PixelHeight)
                    {
                        int pixelIndex = j * stride + i * bytesPerPixel;
                        pixelData[pixelIndex] = brushColor.B;
                        pixelData[pixelIndex + 1] = brushColor.G;
                        pixelData[pixelIndex + 2] = brushColor.R;
                        pixelData[pixelIndex + 3] = brushColor.A;
                    }
                }
            }

            // Copy modified pixel data back to the bitmap
            bitmap.WritePixels(new Int32Rect(0, 0, bitmap.PixelWidth, bitmap.PixelHeight), pixelData, stride, 0);

            return bitmap;
        }
    }
}
