using Emgu.CV;
using Emgu.CV.CvEnum;
using JSharp;
using JSharp.Utility;
using System.Runtime.InteropServices;

namespace JSharp_Tests.Algorithms_Tests
{
    public class ThresholdingTests
    {
        [Theory]
        [InlineData(50, 200, false, new byte[] { 10, 50, 100, 150, 200, 250, 0, 128, 255 }, 3, 3, new byte[] { 0, 255, 255, 255, 255, 0, 0, 255, 0 })]
        [InlineData(50, 200, true, new byte[] { 10, 50, 100, 150, 200, 250, 0, 128, 255 }, 3, 3, new byte[] { 0, 127, 127, 127, 127, 255, 0, 127, 255 })]
        public void StandardThreshold_Test(int minThreshold, int maxThreshold, bool enableContrastMode, byte[] input, int rows, int cols, byte[] expected)
        {
            RunThresholdTest(minThreshold, maxThreshold, ThresholdingType.Standard, enableContrastMode, input, rows, cols, expected);
        }

        [Theory]
        [InlineData(50, 200, false, new byte[] { 10, 50, 100, 150, 200, 250, 0, 128, 255 }, 3, 3, new byte[] { 255, 0, 0, 0, 0, 255, 255, 0, 255 })]
        [InlineData(50, 200, true, new byte[] { 10, 50, 100, 150, 200, 250, 0, 128, 255 }, 3, 3, new byte[] { 255, 127, 127, 127, 127, 0, 255, 127, 0 })]
        public void InverseThreshold_Test(int minThreshold, int maxThreshold, bool enableContrastMode, byte[] input, int rows, int cols, byte[] expected)
        {
            RunThresholdTest(minThreshold, maxThreshold, ThresholdingType.Inverse, enableContrastMode, input, rows, cols, expected);
        }

        [Theory]
        [InlineData(50, 200, false, new byte[] { 10, 50, 100, 150, 200, 250, 0, 128, 255 }, 3, 3, new byte[] { 0, 50, 100, 150, 200, 0, 0, 128, 0 })]
        [InlineData(50, 200, true, new byte[] { 10, 50, 100, 150, 200, 250, 0, 128, 255 }, 3, 3, new byte[] { 0, 50, 100, 150, 200, 255, 0, 128, 255 })]
        public void PreservingGrayscaleLevelsIdentityThreshold_Test(int minThreshold, int maxThreshold, bool enableContrastMode, byte[] input, int rows, int cols, byte[] expected)
        {
            RunThresholdTest(minThreshold, maxThreshold, ThresholdingType.PreservingGrayscaleLevelsIdentity, enableContrastMode, input, rows, cols, expected);
        }

        [Theory]
        [InlineData(50, 200, false, new byte[] { 10, 50, 100, 150, 200, 250, 0, 128, 255 }, 3, 3, new byte[] { 0, 205, 155, 105, 55, 0, 0, 127, 0 })]
        [InlineData(50, 200, true, new byte[] { 10, 50, 100, 150, 200, 250, 0, 128, 255 }, 3, 3, new byte[] { 0, 205, 155, 105, 55, 255, 0, 127, 255 })]
        public void PreservingGrayscaleLevelsNegationThreshold_Test(int minThreshold, int maxThreshold, bool enableContrastMode, byte[] input, int rows, int cols, byte[] expected)
        {
            RunThresholdTest(minThreshold, maxThreshold, ThresholdingType.PreservingGrayscaleLevelsNegation, enableContrastMode, input, rows, cols, expected);
        }

        private void RunThresholdTest(int minThreshold, int maxThreshold, ThresholdingType mode, bool enableContrastMode, byte[] input, int rows, int cols, byte[] expected)
        {
            // Convert the one-dimensional arrays to two-dimensional arrays
            byte[,] inputImage = ConvertTo2D(input, rows, cols);
            byte[,] expectedImage = ConvertTo2D(expected, rows, cols);

            // Create a Mat from the inputImage
            Mat image = CreateMatFrom2DArray(inputImage);

            // Call the Threshold function
            Mat result = ImageProcessingCore.Threshold(image, minThreshold, maxThreshold, mode, enableContrastMode);

            // Convert result Mat back to 2D array
            byte[,] resultImage = ConvertMatTo2DArray(result, rows, cols);

            // Assert equality
            Assert.Equal(expectedImage, resultImage);
        }

        private byte[,] ConvertTo2D(byte[] input, int rows, int cols)
        {
            byte[,] result = new byte[rows, cols];
            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    result[i, j] = input[i * cols + j];
                }
            }
            return result;
        }

        private Mat CreateMatFrom2DArray(byte[,] array)
        {
            int rows = array.GetLength(0);
            int cols = array.GetLength(1);
            Mat mat = new Mat(rows, cols, DepthType.Cv8U, 1);
            IntPtr imageDataPtr = mat.DataPointer;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    Marshal.WriteByte(imageDataPtr, i * cols + j, array[i, j]);
                }
            }
            return mat;
        }

        private byte[,] ConvertMatTo2DArray(Mat mat, int rows, int cols)
        {
            byte[,] array = new byte[rows, cols];
            IntPtr imageDataPtr = mat.DataPointer;

            for (int i = 0; i < rows; i++)
            {
                for (int j = 0; j < cols; j++)
                {
                    array[i, j] = Marshal.ReadByte(imageDataPtr, i * cols + j);
                }
            }
            return array;
        }
    }
}