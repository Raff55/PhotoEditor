using System.Windows;
using System.Windows.Controls;
using System.Windows.Media;

namespace ImageEditor.Tools;

public static class Text
{
    public static TextBlock AddTextToImage(string text, double fontSize, System.Windows.Media.Brush color, FontFamily fontFamily, bool isBold, bool isItalic, bool isUnderline)
    {
        TextBlock textBlock = new()
        {
            Text = text,
            FontSize = fontSize,
            FontWeight = isBold ? FontWeights.Bold : FontWeights.Normal,
            FontStyle = isItalic ? FontStyles.Italic : FontStyles.Normal,
            TextDecorations = isUnderline ? TextDecorations.Underline : null,
            TextAlignment = TextAlignment.Center,
            FontFamily = fontFamily,
            Foreground = color,
        };
        return textBlock;
    }

    public static string GetEnglishColorName(string localizedColorName)
    {
        var armenianColorNames = new Dictionary<string, string>
        {
            { "Սպիտակ", "White" },
            { "Սև", "Black" },
            { "Կարմիր", "Red" },
            { "Կանաչ", "Green" },
            { "Կապույտ", "Blue" },
            { "Դեղին", "Yellow" },
            { "Նարնջագույն", "Orange" },
            { "Մանուշակագույն", "Purple" },
            { "Մոխրագույն", "Gray" }
        };

        var russianColorNames = new Dictionary<string, string>
        {
            { "Белый", "White" },
            { "Черный", "Black" },
            { "Красный", "Red" },
            { "Зелёный", "Green" },
            { "Синий", "Blue" },
            { "Жёлтый", "Yellow" },
            { "Оранжевый", "Orange" },
            { "Фиолетовый", "Purple" },
            { "Серый", "Gray" }
        };

        // Try to find the English name in the Armenian dictionary first
        if (armenianColorNames.TryGetValue(localizedColorName, out string englishName))
        {
            return englishName;
        }

        // If not found in Armenian, try in the Russian dictionary
        if (russianColorNames.TryGetValue(localizedColorName, out englishName))
        {
            return englishName;
        }

        // Fallback to the original localized name if no match found
        return localizedColorName;
    }
}
