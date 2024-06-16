using Emgu.CV;
using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Utility
{
    public static class ImageProcessingUtility
    {
        /// <summary>
        /// Determines the color space based on the number of channels.
        /// </summary>
        /// <param name="NumberOfChannels">The number of channels in the image.</param>
        /// <returns>The detected color space.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static ColorSpaceType OnLoadingDetermineColorspace(int NumberOfChannels)
        {
            return NumberOfChannels switch
            {
                1 => ColorSpaceType.Grayscale,
                3 => ColorSpaceType.RGB,
                _ => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Gets the corresponding element shape for the given shape type.
        /// </summary>
        /// <param name="shapeType">The type of shape.</param>
        /// <returns>The corresponding element shape.</returns>
        /// <exception cref="NotImplementedException"></exception>
        public static ElementShape GetStructuringElementType(ShapeType shapeType)
        {
            return shapeType switch
            {
                ShapeType.Rhombus => ElementShape.Cross,
                ShapeType.Rectangle => ElementShape.Rectangle,
                _ => throw new NotImplementedException()
            };
        }

        /// <summary>
        /// Calculates the percentage of pixels within the specified threshold range in the image.
        /// </summary>
        /// <param name="image">The input image.</param>
        /// <param name="minThreshold">The minimum threshold value.</param>
        /// <param name="maxThreshold">The maximum threshold value.</param>
        /// <returns>The percentage of selected pixels.</returns>
        public static double GetSelectedPixelPercentage(Mat image, int minThreshold, int maxThreshold)
        {
            int totalPixels = image.Rows * image.Cols;
            int selectedPixels = 0;

            // Get data pointer of the image
            IntPtr imageDataPtr = image.DataPointer;

            // Iterate through each pixel in the image
            for (int i = 0; i < totalPixels; i++)
            {
                // Read the pixel value at the current position
                byte pixelValue = Marshal.ReadByte(imageDataPtr, i);

                // Check if the pixel value falls within the specified range
                if (pixelValue >= minThreshold && pixelValue <= maxThreshold)
                {
                    selectedPixels++;
                }
            }

            // Calculate the percentage of selected pixels
            double percentage = (double)selectedPixels / totalPixels * 100.0;

            return percentage;
        }

        /// <summary>
        /// Calculates the Euclidean distance between two System.Windows.Point instances.
        /// </summary>
        /// <param name="point1"></param>
        /// <param name="point2"></param>
        /// <returns>The Euclidean distance between the two points.</returns>
        public static double GetDistance(System.Windows.Point point1, System.Windows.Point point2)
        {
            double dx = point2.X - point1.X;
            double dy = point2.Y - point1.Y;

            // Use Pythagorean theorem to calculate distance
            return Math.Sqrt(dx * dx + dy * dy);
        }

        public static bool IsBinaryImage(int[] histogramData)
        {
            int nonZeroCount = 0;

            // Check the specific brightness levels 0 and 255
            if (histogramData[0] > 0) nonZeroCount++; // Check brightness level 0
            if (histogramData[255] > 0) nonZeroCount++; // Check brightness level 255

            // Return true if exactly two brightness levels are non-zero
            return nonZeroCount == 2;
        }
    }
}
