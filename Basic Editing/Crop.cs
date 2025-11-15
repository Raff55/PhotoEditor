using System.Drawing;
using System.Windows;
using System.Windows.Media.Imaging;

namespace ImageEditor.BasicEditing;

public static class Crop
{
    public static WriteableBitmap CropImage(WriteableBitmap bmpImage, Rectangle cropArea)
    {
        if (cropArea.X < 0 || cropArea.Y < 0 || (cropArea.X + cropArea.Width) > bmpImage.PixelWidth || (cropArea.Y + cropArea.Height) > bmpImage.PixelHeight)
        {
            // Adjust the crop area to ensure it fits within the bounds of the original image
            cropArea.X = Math.Max(0, cropArea.X);
            cropArea.Y = Math.Max(0, cropArea.Y);
            cropArea.Width = Math.Min(bmpImage.PixelWidth - cropArea.X, cropArea.Width);
            cropArea.Height = Math.Min(bmpImage.PixelHeight - cropArea.Y, cropArea.Height);
        }

        CroppedBitmap croppedBitmap = new CroppedBitmap(bmpImage, new Int32Rect(cropArea.X, cropArea.Y, cropArea.Width, cropArea.Height));

        int stride = croppedBitmap.PixelWidth * ((croppedBitmap.Format.BitsPerPixel + 7) / 8);
        byte[] pixelData = new byte[croppedBitmap.PixelHeight * stride];
        croppedBitmap.CopyPixels(pixelData, stride, 0);

        WriteableBitmap writeableBitmap = new WriteableBitmap(croppedBitmap.PixelWidth, croppedBitmap.PixelHeight, croppedBitmap.DpiX, croppedBitmap.DpiY, croppedBitmap.Format, null);
        writeableBitmap.WritePixels(new Int32Rect(0, 0, croppedBitmap.PixelWidth, croppedBitmap.PixelHeight), pixelData, stride, 0);

        return writeableBitmap;
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