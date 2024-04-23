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
        public static ColorSpaceType OnLoadingDetermineColorspace(int NumberOfChannels)
        {
            return NumberOfChannels switch
            {
                1 => ColorSpaceType.Grayscale,
                3 => ColorSpaceType.RGB,
                _ => throw new NotImplementedException()
            };
        }

        public static ElementShape GetStructuringElementType(ShapeType shapeType)
        {
            return shapeType switch
            {
                ShapeType.Rhombus => ElementShape.Cross,
                ShapeType.Rectangle => ElementShape.Rectangle,
                _ => throw new NotImplementedException()
            };
        }

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
    }
}
