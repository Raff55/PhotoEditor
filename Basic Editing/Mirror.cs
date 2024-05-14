using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;

namespace ImageEditor.Transformation
{
    public static class Mirror
    {
        public static WriteableBitmap MirrorImageHorizontal(WriteableBitmap bitmap)
        {
            int width = bitmap.PixelWidth;
            int height = bitmap.PixelHeight;
            int bytesPerPixel = (bitmap.Format.BitsPerPixel + 7) / 8;
            int stride = width * bytesPerPixel;

            byte[] pixels = new byte[height * stride];
            bitmap.CopyPixels(pixels, stride, 0);

            for (int y = 0; y < height / 2; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index1 = y * stride + x * bytesPerPixel;
                    int index2 = (height - y - 1) * stride + x * bytesPerPixel;

                    for (int i = 0; i < bytesPerPixel; i++)
                    {
                        byte temp = pixels[index1 + i];
                        pixels[index1 + i] = pixels[index2 + i];
                        pixels[index2 + i] = temp;
                    }
                }
            }

            WriteableBitmap mirroredBitmap = new WriteableBitmap(width, height, bitmap.DpiX, bitmap.DpiY, bitmap.Format, bitmap.Palette);
            mirroredBitmap.WritePixels(new Int32Rect(0, 0, width, height), pixels, stride, 0);

            return mirroredBitmap;
        }
    }
}
