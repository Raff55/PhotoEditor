namespace ImageEditor.Adjustments.Color;

public static class ColorUtils
{
    public static void RgbToHsl(byte r, byte g, byte b, out double h, out double s, out double l)
    {
        double red = r / 255.0;
        double green = g / 255.0;
        double blue = b / 255.0;

        double max = Math.Max(red, Math.Max(green, blue));
        double min = Math.Min(red, Math.Min(green, blue));

        double hue = 0.0;
        if (max == min)
        {
            hue = 0.0;
        }
        else if (max == red)
        {
            hue = ((green - blue) / (max - min) + 6.0) % 6.0;
        }
        else if (max == green)
        {
            hue = (blue - red) / (max - min) + 2.0;
        }
        else if (max == blue)
        {
            hue = (red - green) / (max - min) + 4.0;
        }

        h = hue * 60.0;
        l = (max + min) / 2.0;

        if (max == min)
        {
            s = 0.0;
        }
        else if (l <= 0.5)
        {
            s = (max - min) / (2.0 * l);
        }
        else
        {
            s = (max - min) / (2.0 - 2.0 * l);
        }
    }

    public static void HslToRgb(double h, double s, double l, out byte r, out byte g, out byte b)
    {
        if (s == 0.0)
        {
            r = g = b = (byte)(l * 255);
        }
        else
        {
            double q = l < 0.5 ? l * (1.0 + s) : l + s - l * s;
            double p = 2.0 * l - q;

            double hk = h / 360.0;

            double tr = hk + 1.0 / 3.0;
            double tg = hk;
            double tb = hk - 1.0 / 3.0;

            r = ToRgbComponent(p, q, tr);
            g = ToRgbComponent(p, q, tg);
            b = ToRgbComponent(p, q, tb);
        }
    }

    private static byte ToRgbComponent(double p, double q, double tc)
    {
        if (tc < 0.0)
        {
            tc += 1.0;
        }
        else if (tc > 1.0)
        {
            tc -= 1.0;
        }

        if (tc < 1.0 / 6.0)
        {
            return (byte)((p + (q - p) * 6.0 * tc) * 255);
        }
        else if (tc < 0.5)
        {
            return (byte)(q * 255);
        }
        else if (tc < 2.0 / 3.0)
        {
            return (byte)((p + (q - p) * (2.0 / 3.0 - tc) * 6.0) * 255);
        }
        else
        {
            return (byte)(p * 255);
        }
    }
}
