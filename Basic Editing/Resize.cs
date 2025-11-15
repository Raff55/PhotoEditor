using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEditor.BasicEditing;

public static class Resize
{
    public static BitmapSource ResizeImage(BitmapSource image, double newWidth, double newHeight)
    {
        var scaleTransform = new ScaleTransform(newWidth / image.PixelWidth, newHeight / image.PixelHeight);
        var resizedBitmap = new TransformedBitmap(image, scaleTransform);
        return resizedBitmap;
    }
}
