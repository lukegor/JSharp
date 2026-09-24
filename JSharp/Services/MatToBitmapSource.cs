using System.Windows.Media.Imaging;
using OpenCvSharp;

namespace JSharp.Services
{
    public static class MatToBitmapSource
    {
        public static WriteableBitmap Convert(Mat mat)
        {
            System.Diagnostics.Debug.Assert(mat.IsContinuous(), "Dense layout assumed: rows*cols copy is only valid on continuous Mats.");
            int stride = mat.Width * (int)mat.ElemSize();
            byte[] buffer = new byte[stride * mat.Height];
            System.Runtime.InteropServices.Marshal.Copy(mat.Data, buffer, 0, buffer.Length);
            var wb = new WriteableBitmap(mat.Width, mat.Height, 96, 96,
                mat.Channels() == 1 ? System.Windows.Media.PixelFormats.Gray8 : System.Windows.Media.PixelFormats.Bgr24, null);
            wb.WritePixels(new System.Windows.Int32Rect(0, 0, mat.Width, mat.Height), buffer, stride, 0);
            return wb;
        }
    }
}
