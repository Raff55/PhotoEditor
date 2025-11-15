namespace ImageEditor;

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

    public static void SetValues(Image1 imageValue) 
    {
        Image.Brightness = imageValue.Brightness;
        Image.Contrast = imageValue.Contrast;
        Image.Highlight = imageValue.Highlight;
        Image.Shadows = imageValue.Shadows;
        Image.Hue= imageValue.Hue;
        Image.Saturation = imageValue.Saturation;
        Image.Temperature = imageValue.Temperature;
        Image.Sharpen = imageValue.Sharpen;
        Image.Blur = imageValue.Blur;
        Image.Vibrance = imageValue.Vibrance;
    }
}

public class Image1
{
    public  double Brightness { get; set; } = 0;
    public  double Contrast { get; set; } = 0;
    public  double Highlight { get; set; } = 0;
    public  double Shadows { get; set; } = 0;
    public  double Hue { get; set; } = 0;
    public  double Saturation { get; set; } = 0;
    public  double Temperature { get; set; } = 0;
    public  double Sharpen { get; set; } = 0;
    public  double Blur { get; set; } = 0;
    public  double Vibrance { get; set; } = 0;
}
