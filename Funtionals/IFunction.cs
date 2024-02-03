using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;

namespace ImageEditor.Funtionals
{
    public interface IFunction
    {
        //partial  BitmapImage Open();

        void Save(WriteableBitmap editedBitmap);

        WriteableBitmap Rotate(WriteableBitmap editedBitmap);

        BitmapSource AdjustBrightness(BitmapSource source, double brightness);

        FormatConvertedBitmap SetFilter(WriteableBitmap img);
    }
}
