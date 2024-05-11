using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows;
using System.Windows.Media;

namespace ImageEditor.Tools
{
    public static class Text
    {
        public static TextBlock AddTextToImage(string text, double fontSize, System.Windows.Media.Brush color, FontFamily fontFamily, bool isBold, bool isItalic, bool isUnderline)
        {
            TextBlock textBlock = new TextBlock
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
    }
}
