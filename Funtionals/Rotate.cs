﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace ImageEditor.Funtionals
{
    public static partial class Functions
    {
        public static WriteableBitmap Rotate(WriteableBitmap editedBitmap) 
        {
            if (editedBitmap != null)
            {
                double angle = 90;

                TransformedBitmap rotatedImage = new TransformedBitmap(editedBitmap, new RotateTransform(angle));
                return new WriteableBitmap(rotatedImage);
            }
            return null;
        }
    }
}