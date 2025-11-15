using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEditor.Adjustments;

public static class Filter
{
    public static FormatConvertedBitmap SetFilter(PixelFormat format, WriteableBitmap img)
    {
        // Check if the requested format is already the same as the input image format
        if (img.Format == format)
        {
            // No need to convert, return the original image
            return new FormatConvertedBitmap(img, format, null, 0);
        }

        // Convert the input image to the desired format
        return new FormatConvertedBitmap(img, format, null, 0);
    }
}
