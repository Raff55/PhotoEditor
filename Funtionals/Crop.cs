using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Drawing;

namespace ImageEditor.Funtionals
{
    public static partial class Funtions
    {
        public static CroppedBitmap Crop(BitmapImage bmpImage, Rectangle cropArea)
        {
            int stride = bmpImage.PixelWidth * (bmpImage.Format.BitsPerPixel / 8);
            byte[] pixelData = new byte[bmpImage.PixelHeight * stride];
            bmpImage.CopyPixels(pixelData, stride, 0);

            CroppedBitmap croppedBitmap = new CroppedBitmap(bmpImage, new Int32Rect(cropArea.X, cropArea.Y, cropArea.Width, cropArea.Height));
            return croppedBitmap;
        }
        public static WriteableBitmap ConvertCroppedBitmapToWriteableBitmap(CroppedBitmap croppedBitmap)
        {
            int stride = croppedBitmap.PixelWidth * ((croppedBitmap.Format.BitsPerPixel + 7) / 8);
            byte[] pixelData = new byte[croppedBitmap.PixelHeight * stride];
            croppedBitmap.CopyPixels(pixelData, stride, 0);

            WriteableBitmap writeableBitmap = new WriteableBitmap(croppedBitmap.PixelWidth, croppedBitmap.PixelHeight, croppedBitmap.DpiX, croppedBitmap.DpiY, croppedBitmap.Format, null);
            writeableBitmap.WritePixels(new Int32Rect(0, 0, croppedBitmap.PixelWidth, croppedBitmap.PixelHeight), pixelData, stride, 0);

            return writeableBitmap;
        }
    }
}
