using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows;
using System.Windows.Media.Effects;

namespace ImageEditor.Color_Adjustments
{
    public static class Blur
    {
        public static async Task<WriteableBitmap> ApplyBlur(WriteableBitmap source, double blurRadius)
        {
            if (source == null || blurRadius <= 0)
                return source;

            // Create a BlurEffect with the specified radius
            BlurEffect blurEffect = new BlurEffect
            {
                Radius = blurRadius
            };

            // Create a DrawingVisual to hold the drawing context
            DrawingVisual drawingVisual = new DrawingVisual();
            using (DrawingContext drawingContext = drawingVisual.RenderOpen())
            {
                // Draw the image into the drawing context
                drawingContext.DrawImage(source, new Rect(0, 0, source.PixelWidth, source.PixelHeight));
            }

            // Apply the blur effect to the DrawingVisual
            drawingVisual.Effect = blurEffect;

            // Create a RenderTargetBitmap to render the visual
            RenderTargetBitmap rtb = new RenderTargetBitmap(
                source.PixelWidth, source.PixelHeight, source.DpiX, source.DpiY, PixelFormats.Pbgra32);
            rtb.Render(drawingVisual);

            // Convert RenderTargetBitmap to WriteableBitmap
            WriteableBitmap blurredBitmap = new WriteableBitmap(rtb);

            return blurredBitmap;
        }
    }
}
