using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace ImageEditor.Funtionals
{
    public static partial class Functions
    {
        private static BitmapImage originalImage;
        public static BitmapImage OpenImage()
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            if (openFileDialog.ShowDialog() == true)
            {
                originalImage = new BitmapImage(new Uri(openFileDialog.FileName));

                BitmapFrame bitmapFrame = BitmapFrame.Create(originalImage.UriSource);
                BitmapMetadata metadata = (BitmapMetadata)bitmapFrame.Metadata;
                if (metadata != null)
                {
                    const string descriptionQuery = "/app1/ifd/exif:{uint=270}";
                    if (metadata.ContainsQuery(descriptionQuery))
                    {
                        metadata.SetQuery(descriptionQuery, "New Image Description");
                    }
                }
            }
            return originalImage;
        }
    }
}
