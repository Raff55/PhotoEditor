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
        public static BitmapSource ApplyBlur(BitmapSource source, double blurRadius)
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
                drawingContext.DrawImage(source, new Rect(0, 0, source.Width, source.Height));
            }
            drawingVisual.Effect = blurEffect;

            // Render the DrawingVisual to a RenderTargetBitmap
            RenderTargetBitmap blurredBitmap = new RenderTargetBitmap(
                (int)source.PixelWidth, (int)source.PixelHeight,
                source.DpiX, source.DpiY, PixelFormats.Pbgra32);

            blurredBitmap.Render(drawingVisual);

            return blurredBitmap;
        }
    }
}
