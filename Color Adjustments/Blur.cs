using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media;
using System.Windows;
namespace ImageEditor.Color_Adjustments
{
    public static class Blur
    {
        public static async Task<WriteableBitmap> ApplyBlur(WriteableBitmap source, double blurRadius)
        {
            if (source == null || blurRadius <= 0)
                return source;

            // Create a BlurEffect
            BlurEffect blurEffect = new BlurEffect
            {
                Radius = blurRadius
            };

            // Apply the blur effect to the image
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                drawingContext.DrawImage(source, new Rect(0, 0, source.PixelWidth, source.PixelHeight));
            }
            drawingVisual.Effect = blurEffect;

            // Render the DrawingVisual to a WriteableBitmap
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, PixelFormats.Pbgra32);
            rtb.Render(drawingVisual);

            // Convert RenderTargetBitmap to WriteableBitmap
            WriteableBitmap blurredBitmap = new WriteableBitmap(rtb);

            return blurredBitmap;
        }

    }
}
