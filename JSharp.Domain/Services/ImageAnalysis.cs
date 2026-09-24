using System.Runtime.InteropServices;
using OpenCvSharp;

namespace JSharp.Domain.Services
{
    /// <summary>
    /// Analysis and structural helpers that are not Mat-to-Mat transforms and therefore
    /// live outside the operation layer.
    /// </summary>
    public static class ImageAnalysis
    {
        public static Mat[] SplitChannels(Mat rgbMat)
        {
            Cv2.Split(rgbMat, out Mat[] channels);
            return channels;
        }

        public static (int[] histogramData, int pixelCount) CalculateHistogramValues(Mat image)
        {
            int totalBytes = image.Rows * image.Cols;
            byte[] buffer = new byte[totalBytes];
            Marshal.Copy(image.Data, buffer, 0, totalBytes);

            int[] histogramData = new int[256];
            for (int i = 0; i < totalBytes; i++)
            {
                histogramData[buffer[i]]++;
            }

            return (histogramData, totalBytes);
        }

        /// <summary>Counts connected components excluding the background.</summary>
        public static int CountObjectsInImage(Mat image)
        {
            using Mat labels = new();
            int nLabels = Cv2.ConnectedComponents(image, labels, PixelConnectivity.Connectivity8);
            return nLabels - 1;
        }

        public static int CountObjectsInImage(Mat image, int? minSize, int? maxSize)
        {
            using Mat labels = new();
            using Mat stats = new();
            using Mat centroids = new();
            int nLabels = Cv2.ConnectedComponentsWithStats(image, labels, stats, centroids,
                PixelConnectivity.Connectivity8, MatType.CV_32S);
            {
                int countInRange = 0;
                for (int label = 1; label < nLabels; label++)
                {
                    int objSize = stats.At<int>(label, 4);
                    if (minSize.HasValue && objSize < minSize)
                    {
                        continue;
                    }

                    if (maxSize.HasValue && objSize > maxSize)
                    {
                        continue;
                    }

                    countInRange++;
                }

                return countInRange;
            }
        }

        public static void AnalyseImage(Mat image, out Point[][] contours, out HierarchyIndex[] hierarchy)
        {
            Cv2.FindContours(image, out contours, out hierarchy, RetrievalModes.List, ContourApproximationModes.ApproxNone);
        }
    }
}
