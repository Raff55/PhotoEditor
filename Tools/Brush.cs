using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ImageEditor.Tools
{
    public static class Brush
    {
        public static Ellipse Draw(Point position, System.Windows.Media.Brush currentBrush, double brushSize)
        {
            Ellipse ellipse = new Ellipse
            {
                Width = brushSize,
                Height = brushSize,
                Fill = currentBrush,
                RenderTransform = new TranslateTransform(position.X - brushSize/ 2, position.Y - brushSize/ 2)
            };
            return ellipse;
        }
    }
}
