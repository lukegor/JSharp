using System.Drawing;
using System.Windows;
using System.Windows.Interop;
using System.Windows.Media.Imaging;
using Emgu.CV;

namespace JSharp.Utility.Utility
{
    public static class MatExtensions
    {
        public static BitmapSource MatToBitmapSource(this Mat image)
        {
            Bitmap bitmap = image.ToBitmap();
            nint hBitmap = bitmap.GetHbitmap();

            try
            {
                BitmapSource bitmapSource = Imaging.CreateBitmapSourceFromHBitmap(
                    hBitmap,
                    nint.Zero,
                    Int32Rect.Empty,
                    BitmapSizeOptions.FromEmptyOptions());
                return bitmapSource;
            }
            finally
            {
                DeleteObject(hBitmap);
                bitmap.Dispose();
            }
        }

        /// <summary>
        /// Deletes a GDI object.
        /// </summary>
        /// <param name="hObject">A handle to the GDI object to be deleted. This handle is obtained from a GDI function.</param>
        /// <returns>
        /// <c>true</c> if the object was deleted; otherwise, <c>false</c>.
        /// </returns>
        /// <remarks>
        /// Effectively fixes huge unmanaged memory leaks.
        /// The DeleteObject function deletes a logical pen, brush, font, bitmap, region, or palette, freeing all system resources associated with the object.
        /// After the object is deleted, the specified handle is no longer valid.
        /// </remarks>
        [System.Runtime.InteropServices.DllImport("gdi32.dll")]
        [return: System.Runtime.InteropServices.MarshalAs(System.Runtime.InteropServices.UnmanagedType.Bool)]
        private static extern bool DeleteObject(nint hObject);
    }
}
