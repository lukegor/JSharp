using JSharp.Domain.Services;
using JSharp.Services;
using OpenCvSharp;
using System.Windows.Media.Imaging;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Renders the 256x256 histogram thumbnail shown by the thresholder dialogs.
    /// </summary>
    internal static class HistogramThumbnail
    {
        public static BitmapSource CreateFor(Mat image)
        {
            using Mat canvas = new(new Size(256, 256), image.Type(), Scalar.All(0));

            (int[] histogramData, _) = ImageAnalysis.CalculateHistogramValues(image);
            DrawBars(canvas, histogramData);
            return MatToBitmapSource.Convert(canvas);
        }

        private static void DrawBars(Mat canvas, int[] histogramData)
        {
            int maxCount = histogramData.Max();
            int histogramWidth = 256;
            int histogramHeight = 256;
            int bottomOffset = 159;

            for (int i = 0; i < histogramData.Length; i++)
            {
                int barHeight = (int)Math.Round((double)histogramData[i] / maxCount * (histogramHeight - bottomOffset));
                barHeight = Math.Max(barHeight, 1);

                int barX = i * (histogramWidth / histogramData.Length);
                int barY = histogramHeight - barHeight - bottomOffset;

                var rect = new Rect(barX, barY, histogramWidth / histogramData.Length, barHeight);
                Cv2.Rectangle(canvas, rect, new Scalar(255, 0, 0), 1);
            }
        }
    }
}
