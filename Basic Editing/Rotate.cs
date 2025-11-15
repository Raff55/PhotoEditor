using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEditor.BasicEditing;

public static class Rotate
{
    public static WriteableBitmap? RotateImage(WriteableBitmap editedBitmap)
    {
        if (editedBitmap != null)
        {
            double angle = 90;

            TransformedBitmap rotatedImage = new TransformedBitmap(editedBitmap, new RotateTransform(angle));
            return new WriteableBitmap(rotatedImage);
        }
        return null;
    }
}
