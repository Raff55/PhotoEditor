using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEditor.Funtionals
{
    public static class Filter
    {
        private static Dictionary<string, PixelFormat> pixelFormats = new Dictionary<string, PixelFormat>()
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
            if (pixelFormats.ContainsKey(formatName))
            {
                return pixelFormats[formatName];
            }

            throw new ArgumentException("Invalid pixel format name.");
        }

        public static string[] GetPixelFormatNames()
        {
            string[] formatNames = new string[pixelFormats.Count];
            pixelFormats.Keys.CopyTo(formatNames, 0);
            return formatNames;
        }

        public static FormatConvertedBitmap SetFilter(PixelFormat format, WriteableBitmap img)
        {
            FormatConvertedBitmap filteredImage = new FormatConvertedBitmap(img, PixelFormats.Gray8, null, 0);
            return filteredImage;
        }
    }
}
