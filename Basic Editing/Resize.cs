using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using System.Windows.Media;

namespace ImageEditor.Functionals
{
    public static class Resize
    {
        public static BitmapSource ResizeImage(BitmapSource image, double newWidth, double newHeight)
        {
            var scaleTransform = new ScaleTransform(newWidth / image.PixelWidth, newHeight / image.PixelHeight);
            var resizedBitmap = new TransformedBitmap(image, scaleTransform);
            return resizedBitmap;
        }
    }
}
