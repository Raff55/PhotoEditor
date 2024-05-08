using System;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEditor.Functionals
{
    public static class Filter
    {
        private static readonly Dictionary<string, PixelFormat> pixelFormats = new Dictionary<string, PixelFormat>()
        {
            { "Bgr32", PixelFormats.Bgr32 },
            { "Bgra32", PixelFormats.Bgra32 },
            { "Pbgra32", PixelFormats.Pbgra32 },
            { "Gray8", PixelFormats.Gray8 },
            { "Indexed8", PixelFormats.Indexed8 },
            { "Rgb24", PixelFormats.Rgb24 },
            { "Prgba64", PixelFormats.Prgba64 },
            { "Gray16", PixelFormats.Gray16 },
            { "Rgb48", PixelFormats.Rgb48 },
            { "Rgba64", PixelFormats.Rgba64 },
            { "Default", PixelFormats.Default }
        };

        public static PixelFormat GetPixelFormat(string formatName)
        {
            if (pixelFormats.TryGetValue(formatName, out PixelFormat format))
            {
                return format;
            }

            throw new ArgumentException("Invalid pixel format name.");
        }

        public static string[] GetPixelFormatNames()
        {
            return pixelFormats.Keys.ToArray();
        }

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
}
