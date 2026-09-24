using OpenCvSharp;
using System.Windows;
using System.Windows.Media.Imaging;

namespace JSharp.Utility.Utility
{
    public static class MatExtensions
    {
        public static BitmapSource MatToBitmapSource(this Mat image)
        {
            ArgumentNullException.ThrowIfNull(image);
            System.Diagnostics.Debug.Assert(image.IsContinuous(), "Dense layout assumed: rows*cols copy is only valid on continuous Mats.");
            int stride = image.Width * (int)image.ElemSize();
            byte[] buffer = new byte[stride * image.Height];
            System.Runtime.InteropServices.Marshal.Copy(image.Data, buffer, 0, buffer.Length);
            var bitmap = new WriteableBitmap(image.Width, image.Height, 96, 96,
                image.Channels() == 1 ? System.Windows.Media.PixelFormats.Gray8 : System.Windows.Media.PixelFormats.Bgr24, null);
            bitmap.WritePixels(new Int32Rect(0, 0, image.Width, image.Height), buffer, stride, 0);
            return bitmap;
        }
    }
}
