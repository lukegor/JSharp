using Emgu.CV;
using Emgu.CV.Reg;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Interop;
using System.Windows;
using System.Windows.Media.Imaging;

namespace JSharp.Utility
{
    public static class MatExtensions
    {
        public static BitmapSource MatToBitmapSource(this Mat image)
        {
            Bitmap bitmap = image.ToBitmap();

            BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                bitmap.GetHbitmap(),
                IntPtr.Zero,
                Int32Rect.Empty,
                BitmapSizeOptions.FromEmptyOptions());

            return bitmapSource;
        }
    }
}
