using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEditor.Color
{
    public static class Hue
    {
        public static BitmapSource AdjustHue(BitmapSource source, double hueValue)
        {
            // Create a new WriteableBitmap based on the source image
            WriteableBitmap writableBitmap = new WriteableBitmap(source);

            // Get the pixel buffer of the writable bitmap
            int width = writableBitmap.PixelWidth;
            int height = writableBitmap.PixelHeight;
            int stride = width * 4; // 4 bytes per pixel (ARGB)
            byte[] pixels = new byte[height * stride];
            writableBitmap.CopyPixels(pixels, stride, 0);

            // Adjust the hue of each pixel in the image
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int index = y * stride + 4 * x;

                    // Get the current pixel values (ARGB)
                    byte alpha = pixels[index + 3];
                    byte red = pixels[index + 2];
                    byte green = pixels[index + 1];
                    byte blue = pixels[index];

                    // Convert RGB to HSL (Hue, Saturation, Lightness)
                    double h, s, l;
                    HslColor.RgbToHsl(red, green, blue, out h, out s, out l);

                    // Adjust the hue value
                    h += hueValue;

                    // Wrap the hue value within the valid range (0-360)
                    h = h < 0 ? h + 360 : h > 360 ? h - 360 : h;

                    // Convert HSL back to RGB
                    HslColor.HslToRgb(h, s, l, out red, out green, out blue);

                    // Update the pixel values in the writable bitmap
                    pixels[index + 3] = alpha;
                    pixels[index + 2] = red;
                    pixels[index + 1] = green;
                    pixels[index] = blue;
                }
            }

            // Create a new bitmap source with the adjusted pixels
            BitmapSource adjustedBitmap = BitmapSource.Create(width, height, source.DpiX, source.DpiY, source.Format, source.Palette, pixels, stride);

            return adjustedBitmap;
        }
    }
}