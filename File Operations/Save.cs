using Microsoft.Win32;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageEditor.Funtionals
{
    public static class Save
    {
        public static void SaveImage(WriteableBitmap editedBitmap) 
        {
            if (editedBitmap != null)
            {
                SaveFileDialog saveFileDialog = new SaveFileDialog();
                saveFileDialog.Filter = "PNG Image|*.png";
                if (saveFileDialog.ShowDialog() == true)
                {
                    using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                    {
                        PngBitmapEncoder encoder = new PngBitmapEncoder();
                        encoder.Frames.Add(BitmapFrame.Create(editedBitmap));
                        encoder.Save(fileStream);
                    }
                }
            }
        }
    }
}
