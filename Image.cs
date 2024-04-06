using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ImageEditor
{
    public static class Image
    {
        public static double Brightness { get; set; } = 0;
        public static double Contrast { get; set; } = 0;
        public static double Highlight { get; set; } = 0;
        public static double Shadows { get; set; } = 0;
        public static double Hue { get; set; } = 0;
        public static double Saturation { get; set; } = 0;
        public static double Temperature { get; set; } = 0;
        public static double Sharpen { get; set; } = 0;
        public static double Blur { get; set; } = 0;
        public static double Vibrance { get; set; } = 0;
    }
}
