using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.ML;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using JSharp.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;

namespace JSharp
{
    public static class ImageProcessingCore
    {
        public static Mat ConvertRgb(Mat image, ColorSpaceType targetColorSpace)
        {
            Mat convertedImage = new Mat();

            switch (targetColorSpace)
            {
                case ColorSpaceType.Grayscale:
                    CvInvoke.CvtColor(image, convertedImage, ColorConversion.Bgr2Gray);
                    break;
                case ColorSpaceType.HSV:
                    CvInvoke.CvtColor(image, convertedImage, ColorConversion.Bgr2Hsv);
                    break;
                case ColorSpaceType.LAB:
                    CvInvoke.CvtColor(image, convertedImage, ColorConversion.Bgr2Lab);
                    break;
                default:
                    throw new InvalidOperationException();
            }

            return convertedImage;
        }

        public static Mat Negate(Mat image)
        {
            Image<Gray, byte> gray = image.ToImage<Gray, byte>();
            IntPtr dataPtr = gray.MIplImage.ImageData;

            int rows = gray.Rows;
            int cols = gray.Cols;

            for (int y = 0; y < rows; ++y)
            {
                for (int x = 0; x < cols; ++x)
                {
                    byte pixelValue = Marshal.ReadByte(dataPtr, y * cols + x);

                    byte negatedValue = (byte)(255 - pixelValue);

                    Marshal.WriteByte(dataPtr, y * cols + x, negatedValue);
                }
            }
            return gray.Mat;
        }

        public static VectorOfMat SplitChannels(Mat rgbMat)
        {
            VectorOfMat channels = new VectorOfMat();
            CvInvoke.Split(rgbMat, channels);
            return channels;
        }

        public static (int[] histogramData, int pixelCount) CalculateHistogramValues(Mat image)
        {
            int[] histogramData = new int[256];
            int sum = 0;

            //Calculate pixels for each brightness level
            for (int y = 0; y < image.Rows; ++y)
            {
                for (int x = 0; x < image.Cols; ++x)
                {
                    IntPtr dataPtr = image.GetDataPointer(y, x);
                    byte pixelValue = Marshal.ReadByte(dataPtr);
                    histogramData[pixelValue]++;
                    ++sum;
                }
            }

            return (histogramData, sum);
        }

        private static (int min, int max) GetMinMaxPixelValuesInImage(Mat image)
        {
            CvInvoke.MinMaxIdx(image, out double min, out double max, null, null);
            return (Convert.ToInt32(min), Convert.ToInt32(max));
        }

        public static Mat StretchHistogram(Mat image)
        {
            CvInvoke.MinMaxIdx(image, out double min, out double max, null, null);
            int p1 = Convert.ToInt32(min), p2 = Convert.ToInt32(max);
            return StretchContrast(image, p1, p2, 0, 255);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="image"></param>
        /// <param name="p1">The lower bound of brightness within which contrast stretching will be applied</param>
        /// <param name="p2">The upper bound of brightness within which contrast stretching will be applied</param>
        /// <param name="q3">The lower bound of brightness into which stretching will be applied</param>
        /// <param name="q4">The upper bound of brightness into which stretching will be applied</param>
        /// <returns></returns>
        public static Mat StretchContrast(Mat image, int p1, int p2, int q3, int q4)
        {
            IntPtr dataPtr = image.DataPointer;

            byte minValue = 255;
            byte maxValue = 0;

            int height = image.Height;
            int width = image.Width;
            int step = image.Step;

            // Find the minimum and maximum pixel values in the image
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int offset = y * step + x;
                    byte pixelValue = Marshal.ReadByte(dataPtr, offset);
                    if (pixelValue >= p1 && pixelValue <= p2)
                    {
                        if (pixelValue < minValue) minValue = pixelValue;
                        if (pixelValue > maxValue) maxValue = pixelValue;
                    }
                }
            }

            // Stretch the histogram
            double dynamic = q4 - q3;
            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int offset = y * step + x;
                    double pixelValue = Marshal.ReadByte(dataPtr, offset);
                    if (pixelValue >= p1 && pixelValue <= p2)
                    {
                        double newValue = ((pixelValue - minValue) / (maxValue - minValue)) * dynamic + q3;
                        byte newByteValue = (byte)Math.Round(newValue);
                        Marshal.WriteByte(dataPtr, offset, newByteValue);
                    }
                }
            }

            return image;
        }

        //public static Mat EqualizeHistogram(Mat image, List<object> histogramData)
        //{
        //    int[] cdf = new int[256];
        //    int sum = 0;

        //    // Calculate cumulative distribution function (CDF)
        //    foreach (var data in histogramData)
        //    {
        //        int pixelCount = (int)data.GetType().GetProperty("PixelCount").GetValue(data);
        //        sum += pixelCount;
        //        int lightnessLevel = (int)data.GetType().GetProperty("LightnessLevel").GetValue(data);
        //        cdf[lightnessLevel] = sum;
        //    }

        //    // Normalize CDF
        //    double[] cdfNormalized = new double[256];
        //    for (int i = 0; i < 256; i++)
        //    {
        //        cdfNormalized[i] = (double)cdf[i] / sum * 255;
        //    }

        //    // Map intensity values using normalized CDF
        //    Mat outputImage = new Mat(image.Size, image.Depth, image.NumberOfChannels);
        //    for (int y = 0; y < image.Rows; ++y)
        //    {
        //        for (int x = 0; x < image.Cols; ++x)
        //        {
        //            IntPtr dataPtr = image.GetDataPointer(y, x);
        //            byte pixelValue = Marshal.ReadByte(dataPtr);
        //            byte newIntensity = (byte)cdfNormalized[pixelValue];
        //            Marshal.WriteByte(dataPtr, newIntensity);
        //        }
        //    }

        //    return outputImage;
        //}

        public static Mat EqualizeHistogram(Mat image, List<object> histogramData)
        {
            int total = image.Width * image.Height;

            int sum = 0;
            float scale = 255.0f / total;
            byte[] lut = new byte[256];
            int i = 0;
            foreach (var data in histogramData)
            {
                int pixelCount = (int)data.GetType().GetProperty("PixelCount").GetValue(data);
                sum += pixelCount;
                int lightnessLevel = (int)data.GetType().GetProperty("LightnessLevel").GetValue(data);
                lut[i] = (byte)(sum * scale);
                ++i;
            }

            Mat equalizedImage = new Mat(image.Size, DepthType.Cv8U, 1);
            IntPtr srcDataPtr = image.DataPointer;
            IntPtr dstDataPtr = equalizedImage.DataPointer;
            int width = image.Width;
            int height = image.Height;
            int step = image.Step;

            for (int y = 0; y < height; y++)
            {
                for (int x = 0; x < width; x++)
                {
                    int offset = y * step + x;
                    byte val = Marshal.ReadByte(srcDataPtr, offset);
                    Marshal.WriteByte(dstDataPtr, offset, lut[val]);
                }
            }

            return equalizedImage;
        }

        public static Mat Posterize(Mat image, int levels)
        {
            Mat posterizedImage = image.Clone();

            int divisor = 256 / levels;

            IntPtr dataPtr = posterizedImage.DataPointer;
            int totalBytes = posterizedImage.Rows * posterizedImage.Cols * posterizedImage.NumberOfChannels;

            for (int i = 0; i < totalBytes; i += posterizedImage.NumberOfChannels)
            {
                byte intensity = Marshal.ReadByte(dataPtr, i);

                // Quantize the intensity channel
                intensity = (byte)(Math.Round(intensity / (double)divisor) * divisor);

                // Write quantized value back to image data
                Marshal.WriteByte(dataPtr, i, intensity);
            }

            return posterizedImage;
        }

        public static Mat ApplyBlur(Mat inputImage, BorderType borderType = BorderType.Isolated, int kernelSize = 3)
        {
            Mat result = new Mat();
            CvInvoke.Blur(inputImage, result, new Size(kernelSize, kernelSize), new Point(-1, -1), borderType);
            return result;
        }

        public static Mat ApplyGaussianBlur(Mat inputImage, BorderType borderType, double sigmaX, double sigmaY, int kernelSize = 3)
        {
            Mat result = new Mat();
            CvInvoke.GaussianBlur(inputImage, result, new Size(kernelSize, kernelSize), sigmaX, sigmaY, borderType);
            return result;
        }

        public static Mat ApplyCustomKernel(Mat inputImage, float[,] kernel, BorderType borderType, int kernelSize = 3, double delta = 0)
        {
            Mat result = new Mat(inputImage.Size, inputImage.Depth, inputImage.NumberOfChannels);

            Matrix<float> kernelMatrix = new Matrix<float>(kernel);
            CvInvoke.Filter2D(inputImage, result, kernelMatrix, new Point(-1, -1), delta, borderType);
            return result;
        }

        public static Mat ApplyMedianFilter(Mat inputGrayImage, int kernelSize, BorderType borderType)
        {
            Mat result = new Mat(inputGrayImage.Size, DepthType.Cv8U, Constants.Grayscale_ChannelCount);
            Mat paddedImage = new Mat();

            int padding = kernelSize / 2;

            CvInvoke.CopyMakeBorder(inputGrayImage, paddedImage, padding, padding, padding, padding, borderType, new MCvScalar());

            int height = paddedImage.Height;
            int width = paddedImage.Width;
            int paddedStep = paddedImage.Step;
            int resultStep = result.Step;

            for (int y = padding; y < height - padding; y++)
            {
                for (int x = padding; x < width - padding; x++)
                {
                    List<byte> values = new List<byte>();

                    for (int i = -padding; i <= padding; i++)
                    {
                        for (int j = -padding; j <= padding; j++)
                        {
                            IntPtr pixelPtr = paddedImage.DataPointer + (y + i) * paddedStep + (x + j) * paddedImage.ElementSize;
                            byte pixelValue = Marshal.ReadByte(pixelPtr);
                            values.Add(pixelValue);
                        }
                    }

                    values.Sort();
                    byte medianValue = values[values.Count / 2];

                    IntPtr outputPtr = result.DataPointer + (y - padding) * resultStep + (x - padding) * result.ElementSize;
                    Marshal.WriteByte(outputPtr, medianValue);
                }
            }

            return result;
        }

        public static Mat Add(Mat image1, Mat image2, PixelOverflowHandlingType overflowHandling, double weight = 1.0)
        {
            Mat resultMat = new Mat(image1.Rows, image1.Cols, image1.Depth, image1.NumberOfChannels);

            IntPtr ptr1 = image1.DataPointer;
            IntPtr ptr2 = image2.DataPointer;
            IntPtr resultPtr = resultMat.DataPointer;

            for (int y = 0; y < image1.Rows; y++)
            {
                for (int x = 0; x < image1.Cols; x++)
                {
                    int offset1 = y * image1.Step + x;
                    int offset2 = y * image2.Step + x;
                    int resultOffset = y * resultMat.Step + x;

                    IntPtr currentPtr1 = IntPtr.Add(ptr1, offset1);
                    IntPtr currentPtr2 = IntPtr.Add(ptr2, offset2);
                    IntPtr currentResultPtr = IntPtr.Add(resultPtr, resultOffset);

                    byte pixel1 = Marshal.ReadByte(currentPtr1);
                    byte pixel2 = Marshal.ReadByte(currentPtr2);
                    byte newPixel = overflowHandling switch
                    {
                        PixelOverflowHandlingType.None_Clipping => (byte)Math.Min(255, pixel1 + pixel2),
                        PixelOverflowHandlingType.Weights => (byte)Math.Min(255, pixel1 + weight * pixel2),
                        PixelOverflowHandlingType.Modulo => (byte)((pixel1 + pixel2) % 256),
                        PixelOverflowHandlingType.LinearScaling => (byte)((pixel1 + pixel2) / 2),
                        PixelOverflowHandlingType.AdaptiveScaling => CalculateAdaptiveScaling(pixel1, pixel2),
                        _ => throw new ArgumentException("Invalid overflow handling option."),
                    };
                    Marshal.WriteByte(currentResultPtr, newPixel);
                }
            }

            return resultMat;
        }

        private static byte CalculateAdaptiveScalingSubtraction(byte pixel1, byte pixel2)
        {
            int sum = pixel1 + pixel2;
            double divisor = Math.Max(1, sum / 255.0);
            return (byte)Math.Floor((sum / divisor));
        }

        public static Mat Sub(Mat image1, Mat image2, PixelOverflowHandlingType overflowHandling, double weight = 1.0)
        {
            Mat resultMat = new Mat(image1.Rows, image1.Cols, image1.Depth, image1.NumberOfChannels);

            IntPtr ptr1 = image1.DataPointer;
            IntPtr ptr2 = image2.DataPointer;
            IntPtr resultPtr = resultMat.DataPointer;

            for (int y = 0; y < image1.Rows; y++)
            {
                for (int x = 0; x < image1.Cols; x++)
                {
                    int offset1 = y * image1.Step + x;
                    int offset2 = y * image2.Step + x;
                    int resultOffset = y * resultMat.Step + x;

                    IntPtr currentPtr1 = IntPtr.Add(ptr1, offset1);
                    IntPtr currentPtr2 = IntPtr.Add(ptr2, offset2);
                    IntPtr currentResultPtr = IntPtr.Add(resultPtr, resultOffset);

                    byte pixel1 = Marshal.ReadByte(currentPtr1);
                    byte pixel2 = Marshal.ReadByte(currentPtr2);
                    byte newPixel = overflowHandling switch
                    {
                        PixelOverflowHandlingType.None_Clipping => (byte)Math.Min(0, pixel1 - pixel2),
                        //PixelOverflowHandlingType.Weights => (byte)Math.Min(0, pixel1 + weight * pixel2),
                        PixelOverflowHandlingType.Modulo => (byte)((pixel1 - pixel2 + 256) % 256),
                        PixelOverflowHandlingType.LinearScaling => (byte)Math.Abs(pixel1 - pixel2),
                        PixelOverflowHandlingType.AdaptiveScaling => CalculateAdaptiveScaling(pixel1, pixel2),
                        _ => throw new ArgumentException("Invalid overflow handling option."),
                    };
                    Marshal.WriteByte(currentResultPtr, newPixel);
                }
            }

            return resultMat;
        }

        private static byte CalculateAdaptiveScaling(byte pixel1, byte pixel2)
        {
            int difference = pixel1 - pixel2;
            int sum = pixel1 + pixel2;

            // Adjust the divisor to prevent division by zero and handle extremes.
            double divisor = Math.Max(1, sum / 255.0);

            // Scale the difference based on the sum.
            double scaledDifference = difference / divisor;

            // Ensure the scaled difference falls within the valid intensity range.
            return (byte)Math.Max(0, Math.Min(255, 128 + scaledDifference));
        }
    }
}
