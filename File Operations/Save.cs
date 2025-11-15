using Microsoft.Win32;
using System.IO;
using System.Windows.Controls;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEditor.FileOperations;

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

    public static void SaveCollage(Canvas canvas)
    {
        if (canvas != null)
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "PNG Image|*.png";

            RenderTargetBitmap renderTargetBitmap = new RenderTargetBitmap(
                (int)canvas.ActualWidth, (int)canvas.ActualHeight,
                96, 96, PixelFormats.Pbgra32);
            renderTargetBitmap.Render(canvas);
            BitmapEncoder bitmapEncoder = new PngBitmapEncoder();
            bitmapEncoder.Frames.Add(BitmapFrame.Create(renderTargetBitmap));

            if (saveFileDialog.ShowDialog() == true)
            {
                using (var fileStream = new FileStream(saveFileDialog.FileName, FileMode.Create))
                {
                    bitmapEncoder.Save(fileStream);
                }
            }
        }
    }
}
