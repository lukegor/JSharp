using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.ML;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using Emgu.CV.XImgproc;
using JSharp.Models;
using JSharp.Resources;
using JSharp.Utility;
using System;
using System.Collections.Generic;
using System.Diagnostics.Metrics;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Effects;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace JSharp
{
    public static class ImageProcessingCore
    {
        /// <summary>
        /// Converts an RGB image to a specified color space.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="targetColorSpace"></param>
        /// <returns>The converted image in the target color space.</returns>
        /// <exception cref="InvalidOperationException"></exception>
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

        /// <summary>
        /// Negates a grayscale image by inverting its pixel values.
        /// </summary>
        /// <param name="image">The input grayscale image.</param>
        /// <returns>The negated grayscale image.</returns>
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

        /// <summary>
        /// Splits an RGB image into its three separate channels.
        /// </summary>
        /// <param name="rgbMat">The input RGB image.</param>
        /// <returns>A vector of matrices containing the separate channels.</returns>
        public static VectorOfMat SplitChannels(Mat rgbMat)
        {
            VectorOfMat channels = new VectorOfMat();
            CvInvoke.Split(rgbMat, channels);
            return channels;
        }

        /// <summary>
        /// Calculates the histogram values for a grayscale image.
        /// </summary>
        /// <param name="image">The input grayscale image.</param>
        /// <returns>A tuple containing the histogram data and the total pixel count.</returns>
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

        /// <summary>
        /// Gets the minimum and maximum pixel values in an image.
        /// </summary>
        /// <param name="image">The input image.</param>
        /// <returns>A tuple containing the minimum and maximum pixel values.</returns>
        private static (int min, int max) GetMinMaxPixelValuesInImage(Mat image)
        {
            CvInvoke.MinMaxIdx(image, out double min, out double max, null, null);
            return (Convert.ToInt32(min), Convert.ToInt32(max));
        }

        /// <summary>
        /// Stretches the histogram of a grayscale image.
        /// </summary>
        /// <param name="image">The input grayscale image.</param>
        /// <returns>The image with stretched histogram.</returns>
        public static Mat StretchHistogram(Mat image)
        {
            CvInvoke.MinMaxIdx(image, out double min, out double max, null, null);
            int p1 = Convert.ToInt32(min), p2 = Convert.ToInt32(max);
            return StretchContrast(image, p1, p2, 0, 255);
        }

        /// <summary>
        /// Stretches the contrast of an image within specified brightness bounds.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="p1">The lower bound of brightness within which contrast stretching will be applied</param>
        /// <param name="p2">The upper bound of brightness within which contrast stretching will be applied</param>
        /// <param name="q3">The lower bound of brightness into which stretching will be applied</param>
        /// <param name="q4">The upper bound of brightness into which stretching will be applied</param>
        /// <returns>The image with stretched contrast.</returns>
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

        /// <summary>
        /// Equalizes the histogram of a grayscale image.
        /// </summary>
        /// <param name="image">The input grayscale image.</param>
        /// <param name="histogramData">The histogram data of the image.</param>
        /// <returns>The image with equalized histogram.</returns>
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
                //int lightnessLevel = (int)data.GetType().GetProperty("LightnessLevel").GetValue(data);
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

        /// <summary>
        /// Posterizes a grayscale image by reducing the number of intensity levels.
        /// </summary>
        /// <param name="image">The input grayscale image.</param>
        /// <param name="levels">The number of intensity levels to reduce to.</param>
        /// <returns>The posterized image.</returns>
        public static Mat Posterize(Mat image, int levels)
        {
            Mat posterizedImage = image.Clone();

            int divisor = 255 / (levels-1);

            IntPtr dataPtr = posterizedImage.DataPointer;
            int totalBytes = posterizedImage.Rows * posterizedImage.Cols;

            for (int i = 0; i < totalBytes; ++i)
            {
                byte intensity = Marshal.ReadByte(dataPtr, i);

                // Quantize the intensity channel
                intensity = (byte)(Math.Round(intensity / (double)divisor) * divisor);

                // Write quantized value back to image data
                Marshal.WriteByte(dataPtr, i, intensity);
            }

            return posterizedImage;
        }

        /// <summary>
        /// Applies a blur filter to an image.
        /// </summary>
        /// <param name="inputImage">The input image.</param>
        /// <param name="borderType">The border type for handling image borders.</param>
        /// <param name="kernelSize">The size of the kernel to use for blurring.</param>
        /// <returns>The blurred image.</returns>
        public static Mat ApplyBlur(Mat inputImage, BorderType borderType, int kernelSize = 3)
        {
            Mat result = new Mat();
            CvInvoke.Blur(inputImage, result, new Size(kernelSize, kernelSize), new Point(-1, -1), borderType);
            return result;
        }

        /// <summary>
        /// Applies a Gaussian blur filter to an image.
        /// </summary>
        /// <param name="inputImage">The input image.</param>
        /// <param name="borderType">The border type for handling image borders.</param>
        /// <param name="sigmaX">The Gaussian kernel standard deviation in the X direction.</param>
        /// <param name="sigmaY">The Gaussian kernel standard deviation in the Y direction.</param>
        /// <param name="kernelSize">The size of the kernel to use for blurring.</param>
        /// <returns>The blurred image.</returns>
        public static Mat ApplyGaussianBlur(Mat inputImage, BorderType borderType, double sigmaX, double sigmaY, int kernelSize = 3)
        {
            Mat result = new Mat();
            CvInvoke.GaussianBlur(inputImage, result, new Size(kernelSize, kernelSize), sigmaX, sigmaY, borderType);
            return result;
        }

        public static Mat ApplyEdgeDetectionFilter(Mat inputImage, string currentKernel, ConvolutionInfo convolutionInfo)
        {
            BorderType borderType = convolutionInfo.BorderPixelsOption;

            // Apply the edge detection kernel
            Mat result = new Mat();
            if (currentKernel == Kernels.SobelNS)
            {
                CvInvoke.Sobel(inputImage, result, DepthType.Cv64F, xorder: 1, yorder: 0, 3, 1, 0, borderType);
            }
            else if (currentKernel == Kernels.SobelEW)
            {
                CvInvoke.Sobel(inputImage, result, DepthType.Cv64F, xorder: 0, yorder: 1, 3, 1, 0, borderType);
            }
            else if (currentKernel == Kernels.Canny)
            {
                CvInvoke.Canny(inputImage, result, (double)convolutionInfo.Min, (double)convolutionInfo.Max);
            }
            else if (currentKernel == Kernels.Laplacian)
            {
                CvInvoke.Laplacian(inputImage, result, DepthType.Cv64F, 1, 1, 0, borderType);
            }
            else throw new InvalidOperationException("Invalid kernel specified: " + currentKernel);

            // Convert to 8-bit unsigned integer
            Mat result8U = new Mat();
            CvInvoke.ConvertScaleAbs(result, result8U, 1.0, 1.0);

            return result8U;
        }

        /// <remarks>
        /// TODO: This method needs to be fixed.
        /// </remarks>
        public static Mat ApplyCustomKernel(Mat inputImage, float[,] kernel, BorderType borderType, int kernelSize = 3, double delta = 0)
        {
            Mat result = new Mat(inputImage.Size, inputImage.Depth, inputImage.NumberOfChannels);

            Matrix<float> kernelMatrix = new Matrix<float>(kernel);
            CvInvoke.Filter2D(inputImage, result, kernelMatrix, new Point(-1, -1), delta, borderType);

            // Normalization - probably doesn't entirely work
            int kernelSum = 0;
            for (int i = 0; i < kernelSize; i++)
            {
                for (int j = 0; j < kernelSize; j++)
                {
                    kernelSum += (int)kernel[i, j];
                }
            }
            if (kernelSum != 0)
            {
                result = result / kernelSum;
            }

            return result;
        }

        /// <remarks>
        /// TODO: This method needs to be fixed.
        /// </remarks>
        public static Mat FullConvolution(Mat inputImage, float[,] kernel1, float[,] kernel2, double delta = 0)
        {
            int paddingSize = kernel1.GetLength(0) / 2; // Założenie: oba jądra są kwadratowe i mają nieparzysty rozmiar

            // Dodaj padding do oryginalnego obrazu przed każdą filtracją
            Mat paddedImage = new Mat();
            CvInvoke.CopyMakeBorder(inputImage, paddedImage, paddingSize, paddingSize, paddingSize, paddingSize, BorderType.Constant, new MCvScalar(0));

            // Pierwsza filtracja
            Mat filteredImage1 = new Mat(paddedImage.Size, paddedImage.Depth, paddedImage.NumberOfChannels);
            Matrix<float> kernelMatrix1 = new Matrix<float>(kernel1);
            CvInvoke.Filter2D(paddedImage, filteredImage1, kernelMatrix1, new Point(-1, -1), delta);

            Mat paddedImage2 = new Mat();
            CvInvoke.CopyMakeBorder(filteredImage1, paddedImage2, paddingSize, paddingSize, paddingSize, paddingSize, BorderType.Constant, new MCvScalar(0));

            // Druga filtracja na wyniku pierwszej filtracji
            Mat filteredImage2 = new Mat(paddedImage2.Size, paddedImage2.Depth, paddedImage2.NumberOfChannels);
            Matrix<float> kernelMatrix2 = new Matrix<float>(kernel2);
            CvInvoke.Filter2D(paddedImage2, filteredImage2, kernelMatrix2, new Point(-1, -1), delta);

            return filteredImage2;
        }

        /// <summary>
        /// Applies a median filter to a grayscale image.
        /// </summary>
        /// <param name="inputGrayImage">The input grayscale image.</param>
        /// <param name="kernelSize">The size of the median filter kernel.</param>
        /// <param name="borderType">The border type for handling image borders.</param>
        /// <returns>The image after applying the median filter.</returns>
        public static Mat ApplyMedianFilter(Mat inputGrayImage, int kernelSize, BorderType borderType)
        {
            Mat result = new Mat(inputGrayImage.Size, DepthType.Cv8U, Constants.Grayscale_ChannelCount);
            Mat paddedImage = new Mat();

            CvInvoke.MedianBlur(paddedImage, result, kernelSize);

            return result;
        }

        public static Mat Add(Mat image1, Mat image2)
        {
            Mat resultMat = new Mat();
            CvInvoke.Add(image1, image2, resultMat);
            return resultMat;
        }

        public static Mat Subtract(Mat image1, Mat image2)
        {
            Mat resultMat = new Mat();
            CvInvoke.Subtract(image1, image2, resultMat);
            return resultMat;
        }

        public static Mat Blend(Mat image1, Mat image2, double weight1)
        {
            // Perform linear blending
            Mat resultMat = new Mat();
            CvInvoke.AddWeighted(image1, weight1, image2, (1-weight1), 0.0, resultMat);

            return resultMat;
        }

        /// <summary>
        /// Performs a bitwise AND operation between two images.
        /// </summary>
        /// <param name="image1">The first input image.</param>
        /// <param name="image2">The second input image.</param>
        /// <returns>The result of the bitwise AND operation.</returns>
        public static Mat BitwiseAnd(Mat image1, Mat image2)
        {
            Mat result = new Mat();
            CvInvoke.BitwiseAnd(image1, image2, result);
            return result;
        }

        /// <summary>
        /// Performs a bitwise OR operation between two images.
        /// </summary>
        /// <param name="image1">The first input image.</param>
        /// <param name="image2">The second input image.</param>
        /// <returns>The result of the bitwise OR operation.</returns>
        public static Mat BitwiseOr(Mat image1, Mat image2)
        {
            Mat result = new Mat();
            CvInvoke.BitwiseOr(image1, image2, result);
            return result;
        }

        /// <summary>
        /// Performs a bitwise NOT operation on an image.
        /// </summary>
        /// <param name="image1">The input image.</param>
        /// <returns>The result of the bitwise NOT operation.</returns>
        public static Mat BitwiseNot(Mat image1)
        {
            Mat result = new Mat();
            CvInvoke.BitwiseNot(image1, result);
            return result;
        }

        /// <summary>
        /// Performs a bitwise XOR operation between two images.
        /// </summary>
        /// <param name="image1">The first input image.</param>
        /// <param name="image2">The second input image.</param>
        /// <returns>The result of the bitwise XOR operation.</returns>
        public static Mat BitwiseXor(Mat image1, Mat image2)
        {
            Mat result = new Mat();
            CvInvoke.BitwiseXor(image1, image2, result);
            return result;
        }

        /// <summary>
        /// Applies morphological erosion to an image.
        /// </summary>
        /// <param name="image">The input image.</param>
        /// <param name="element">The structuring element for erosion.</param>
        /// <param name="borderType">The border type for handling image borders.</param>
        /// <param name="borderValue">The value to use for pixels beyond the image boundaries.</param>
        /// <returns>The image after applying morphological erosion.</returns>
        public static Mat Erode(Mat image, Mat element, BorderType borderType, MCvScalar borderValue)
        {
            Mat result = new Mat();
            CvInvoke.Erode(image, result, element, new Point(-1, -1), 1, borderType, borderValue);
            return result;
        }

        /// <summary>
        /// Creates Rhombus structuring element
        /// </summary>
        /// <param name="r">The radius of the diamond.</param>
        /// <returns>The diamond-shaped structuring element.</returns>
        public static Mat Diamond(int r)
        {
            Mat diamond = new Mat(new Size(r * 2 + 1, r * 2 + 1), DepthType.Cv8U, 1);
            byte[,] diamondData = new byte[r * 2 + 1, r * 2 + 1];

            for (int i = 0; i <= r; i++)
            {
                for (int j = r - i; j <= r + i; j++)
                {
                    diamondData[i, j] = 255;
                    diamondData[diamondData.GetLength(0) - 1 - i, j] = 255;
                }
            }

            Marshal.Copy(diamondData.Cast<byte>().ToArray(), 0, diamond.DataPointer, diamondData.Length);
            return diamond;
        }

        /// <summary>
        /// Applies morphological dilation to an image.
        /// </summary>
        /// <param name="image">The input image.</param>
        /// <param name="element">The structuring element for dilation.</param>
        /// <param name="borderType">The border type for handling image borders.</param>
        /// <param name="borderValue">The value to use for pixels beyond the image boundaries.</param>
        /// <returns>The image after applying morphological dilation.</returns>
        public static Mat Dilate(Mat image, Mat element, BorderType borderType, MCvScalar borderValue)
        {
            Mat result = new Mat();
            CvInvoke.Dilate(image, result, element, new Point(-1, -1), 1, borderType, borderValue);
            return result;
        }

        /// <summary>
        /// Performs morphological opening on an image.
        /// </summary>
        /// <param name="image">The input image.</param>
        /// <param name="element">The structuring element for the morphological operation.</param>
        /// <param name="borderType">Type of border to add.</param>
        /// <param name="borderValue">Value used in case of a constant border.</param>
        /// <returns>The result of morphological opening on the input image.</returns>
        public static Mat MorphologicalOpen(Mat image, Mat element, BorderType borderType, MCvScalar borderValue)
        {
            Mat result = new Mat();
            CvInvoke.MorphologyEx(image, result, MorphOp.Open, element, new Point(-1, -1), 1, borderType, borderValue);
            return result;
        }

        /// <summary>
        /// Performs morphological closing on an image.
        /// </summary>
        /// <param name="image">The input image.</param>
        /// <param name="element">The structuring element for the morphological operation.</param>
        /// <param name="borderType">Type of border to add.</param>
        /// <param name="borderValue">Value used in case of a constant border.</param>
        /// <returns>The result of morphological closing on the input image.</returns>
        public static Mat MorphologicalClose(Mat image, Mat element, BorderType borderType, MCvScalar borderValue)
        {
            Mat result = new Mat();
            CvInvoke.MorphologyEx(image, result, MorphOp.Close, element, new Point(-1, -1), 1, borderType, borderValue);
            return result;
        }

        /// <summary>
        /// Applies simple thresholding to the input image.
        /// </summary>
        /// <param name="image">The input image.</param>
        /// <param name="threshold">The threshold value.</param>
        /// <param name="thresholdingMethod">The method of thresholding to apply.</param>
        /// <returns>The result of thresholding applied to the input image.</returns>
        public static Mat SimpleThreshold(Mat image, int threshold, SimpleThresholdingMethod thresholdingMethod)
        {
            Mat result = new Mat();

            ThresholdType thresholdType;
            switch (thresholdingMethod)
            {
                case SimpleThresholdingMethod.Standard:
                    thresholdType = ThresholdType.Binary;
                    CvInvoke.Threshold(image, result, threshold, 255, thresholdType);
                    break;
                case SimpleThresholdingMethod.Otsu:
                    thresholdType = ThresholdType.Otsu;
                    double value = CvInvoke.Threshold(image, result, 0, 255, thresholdType);
                    break;
            };

            return result;
        }

        public static Mat AdaptiveThreshold(Mat image)
        {
            Mat result = new Mat();

            int odd = 11;
            int constantSubtracter = 5;
            CvInvoke.AdaptiveThreshold(image, result, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, odd, constantSubtracter);

            return result;
        }

        /// <summary>
        /// Applies thresholding to the input image based on the given thresholds and mode.
        /// </summary>
        /// <param name="image">Input image.</param>
        /// <param name="minThreshold">Lower threshold value T1.</param>
        /// <param name="maxThreshold">Upper threshold value T2.</param>
        /// <param name="mode">Thresholding mode to apply.</param>
        /// <param name="enableContrastMode">Whether to enable contrast mode.</param>
        /// <returns>Result of the applied thresholding to the input image.</returns>
        public static Mat Threshold(Mat image, int minThreshold, int maxThreshold, ThresholdingType mode, bool enableContrastMode)
        {
            // pointer to image data
            IntPtr imageDataPtr = image.DataPointer;

            // delegate Func stores a reference to the function
            // choosing the thresholding function
            Func<byte, byte> thresholdFunction = mode switch
            {
                ThresholdingType.Standard => enableContrastMode
                    ? new Func<byte, byte>((pixelValue) => StandardContrastThreshold(pixelValue, minThreshold, maxThreshold))
                    : new Func<byte, byte>((pixelValue) => StandardThreshold(pixelValue, minThreshold, maxThreshold)),
                ThresholdingType.Inverse => enableContrastMode
                    ? new Func<byte, byte>((pixelValue) => InverseContrastThreshold(pixelValue, minThreshold, maxThreshold))
                    : new Func<byte, byte>((pixelValue) => InverseThreshold(pixelValue, minThreshold, maxThreshold)),
                ThresholdingType.PreservingGrayscaleLevelsIdentity => enableContrastMode
                    ? new Func<byte, byte>((pixelValue) => PreserveIdentityContrastThreshold(pixelValue, minThreshold, maxThreshold))
                    : new Func<byte, byte>((pixelValue) => PreserveIdentityThreshold(pixelValue, minThreshold, maxThreshold)),
                ThresholdingType.PreservingGrayscaleLevelsNegation => enableContrastMode
                    ? new Func<byte, byte>((pixelValue) => PreserveNegationContrastThreshold(pixelValue, minThreshold, maxThreshold))
                    : new Func<byte, byte>((pixelValue) => PreserveNegationThreshold(pixelValue, minThreshold, maxThreshold)),
                // throw an exception if the selected thresholding mode is not supported
                _ => throw new NotSupportedException($"Invalid threshold mode: {mode}"),
            };

            // iterate through each pixel in the image
            for (int i = 0; i < image.Rows * image.Cols; i++)
            {
                // read the pixel value at the current position
                byte pixelValue = Marshal.ReadByte(imageDataPtr, i);

                // apply thresholding using the selected method to the specific pixel
                byte newPixelValue = thresholdFunction(pixelValue);
                Marshal.WriteByte(imageDataPtr, i, newPixelValue);
            }

            return image;
        }

        /// <summary>
        /// Applies standard thresholding with two thresholds to a single pixel.
        /// </summary>
        /// <param name="pixelValue">Pixel value to be thresholded.</param>
        /// <param name="minThreshold">Lower threshold value T1.</param>
        /// <param name="maxThreshold">Upper threshold value T2.</param>
        /// <returns>Pixel value after thresholding.</returns>
        private static byte StandardThreshold(byte pixelValue, int minThreshold, int maxThreshold)
        {
            return (pixelValue >= minThreshold && pixelValue <= maxThreshold) ? (byte)255 : (byte)0;
        }

        /// <summary>
        /// Applies inverse thresholding with two thresholds to a single pixel.
        /// </summary>
        /// <param name="pixelValue">Pixel value to be thresholded.</param>
        /// <param name="minThreshold">Lower threshold value T1.</param>
        /// <param name="maxThreshold">Upper threshold value T2.</param>
        /// <returns>Pixel value after thresholding.</returns>
        private static byte InverseThreshold(byte pixelValue, int minThreshold, int maxThreshold)
        {
            return (pixelValue >= minThreshold && pixelValue <= maxThreshold) ? (byte)0 : (byte)255;
        }

        /// <summary>
        /// Applies thresholding with two thresholds while preserving grayscale levels (identity preservation) to a single pixel.
        /// </summary>
        /// <param name="pixelValue">Pixel value to be thresholded.</param>
        /// <param name="minThreshold">Lower threshold value T1.</param>
        /// <param name="maxThreshold">Upper threshold value T2.</param>
        /// <returns>Pixel value after thresholding.</returns>
        private static byte PreserveIdentityThreshold(byte pixelValue, int minThreshold, int maxThreshold)
        {
            return (pixelValue >= minThreshold && pixelValue <= maxThreshold) ? pixelValue : (byte)0;
        }

        /// <summary>
        /// Applies thresholding with two thresholds while preserving grayscale levels and negation to a single pixel.
        /// </summary>
        /// <param name="pixelValue">Pixel value to be thresholded.</param>
        /// <param name="minThreshold">Lower threshold value T1.</param>
        /// <param name="maxThreshold">Upper threshold value T2.</param>
        /// <returns>Pixel value after thresholding.</returns>
        private static byte PreserveNegationThreshold(byte pixelValue, int minThreshold, int maxThreshold)
        {
            return (pixelValue >= minThreshold && pixelValue <= maxThreshold) ? (byte)(255 - pixelValue) : (byte)0;
        }

        /// <summary>
        /// Applies contrast thresholding with two thresholds in standard mode to a single pixel.
        /// </summary>
        /// <param name="pixelValue">Pixel value to be thresholded.</param>
        /// <param name="minThreshold">Lower threshold value T1.</param>
        /// <param name="maxThreshold">Upper threshold value T2.</param>
        /// <returns>Pixel value after thresholding.</returns>
        private static byte StandardContrastThreshold(byte pixelValue, int minThreshold, int maxThreshold)
        {
            if (pixelValue < minThreshold)
                return 0;
            if (pixelValue <= maxThreshold)
                return 127;
            return 255;
        }

        /// <summary>
        /// Applies inverse thresholding with two thresholds in contrast mode to a single pixel.
        /// </summary>
        /// <param name="pixelValue">Pixel value to be thresholded.</param>
        /// <param name="minThreshold">Lower threshold value T1.</param>
        /// <param name="maxThreshold">Upper threshold value T2.</param>
        /// <returns>Pixel value after thresholding.</returns>
        private static byte InverseContrastThreshold(byte pixelValue, int minThreshold, int maxThreshold)
        {
            if (pixelValue < minThreshold)
                return 255;
            if (pixelValue <= maxThreshold)
                return 127;
            return 0;
        }

        /// <summary>
        /// Applies thresholding with two thresholds while preserving grayscale levels (identity preservation) in contrast mode to a single pixel.
        /// </summary>
        /// <param name="pixelValue">Pixel value to be thresholded.</param>
        /// <param name="minThreshold">Lower threshold value T1.</param>
        /// <param name="maxThreshold">Upper threshold value T2.</param>
        /// <returns>Pixel value after thresholding.</returns>
        private static byte PreserveIdentityContrastThreshold(byte pixelValue, int minThreshold, int maxThreshold)
        {
            if (pixelValue < minThreshold)
                return 0;
            if (pixelValue <= maxThreshold)
                return pixelValue;
            return 255;
        }

        /// <summary>
        /// Applies thresholding with two thresholds in negation mode (preserving grayscale levels with negation) in contrast mode to a single pixel.
        /// </summary>
        /// <param name="pixelValue">Pixel value to be thresholded.</param>
        /// <param name="minThreshold">Lower threshold value T1.</param>
        /// <param name="maxThreshold">Upper threshold value T2.</param>
        /// <returns>Pixel value after thresholding.</returns>
        private static byte PreserveNegationContrastThreshold(byte pixelValue, int minThreshold, int maxThreshold)
        {
            if (pixelValue < minThreshold)
                return 0;
            if (pixelValue <= maxThreshold)
                return (byte)(255 - pixelValue);
            return 255;
        }


        /// <summary>
        /// Counts the number of connected components (objects) in the given image.
        /// </summary>
        /// <param name="image">Input image on which objects are counted.</param>
        /// <returns>Number of connected components (objects) in the image, excluding the background.</returns>
        public static int CountObjectsInImage(Mat image)
        {
            // labeled image
            Mat labels = new Mat();

            // applying the ConnectedComponents method from EmguCv, which refers to the method from OpenCv
            int nLabels = CvInvoke.ConnectedComponents(image, labels);

            return nLabels - 1; // number of objects excluding the background
        }

        /// <summary>
        /// Counts the number of connected components (objects) in the given image that meet specified criteria.
        /// </summary>
        /// <param name="image">Input image on which objects are counted.</param>
        /// <param name="minSize">Minimum size of objects to be counted. If null, no minimum size is applied.</param>
        /// <param name="maxSize">Maximum size of objects to be counted. If null, no maximum size is applied.</param>
        /// <returns>Number of connected components (objects) that meet the specified criteria, excluding the background.</returns>
        public static int CountObjectsInImage(Mat image, int? minSize, int? maxSize)
        {
            // labeled image
            Mat labels = new Mat();
            // object statistics
            Mat stats = new Mat();
            // object centroids
            Mat centroids = new Mat();
            // number of objects
            int nLabels = CvInvoke.ConnectedComponentsWithStats(image, labels, stats, centroids);

            int countInRange = 0;
            int[,] areaData = (int[,])stats.GetData(); // Get all data from the stats matrix

            for (int label = 1; label < nLabels; label++) // Start from 1 to exclude the background
            {
                // Get the size of the current object
                int objSize = areaData[label, 4]; // 4th column represents the area

                // Check if the object size is within the specified range
                bool withinRange = true;

                if (minSize.HasValue && objSize < minSize)
                    withinRange = false;
                else if (maxSize.HasValue && objSize > maxSize)
                    withinRange = false;

                // Count objects that meet the size criteria
                if (withinRange)
                    countInRange++;
            }

            return countInRange;
        }

        /// <summary>
        /// Analyzes the input image and detects contours.
        /// </summary>
        /// <param name="image">The input image.</param>
        /// <param name="retrType">The retrieval mode.</param>
        /// <param name="chainApproxMethod">The method for approximating contour chains.</param>
        /// <returns>A vector of vector of points representing contours found in the image.</returns>
        public static VectorOfVectorOfPoint AnalyseImage(Mat image, RetrType retrType, ChainApproxMethod chainApproxMethod)
        {
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(image, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxNone);

            return contours;
        }

        /// <summary>
        /// Detects lines in the input image using the probabilistic Hough transform.
        /// </summary>
        /// <param name="image">The input image.</param>
        /// <returns>An image with detected lines drawn on it.</returns>
        public static Mat Hough(Mat image)
        {
            Mat img = image.Clone();

            Mat edges = new Mat();

            CvInvoke.Canny(img, edges, 50, 100);

            double rho = 1; // Distance resolution of the accumulator in pixels
            double theta = Math.PI / 180; // Angle resolution of the accumulator in radians
            int threshold = 100;
            double minLineLength = 50; // Minimalna długość prostej
            double maxLineGap = 10; // Maksymalna przerwa między odcinkami, aby były uznane za jedną linię

            LineSegment2D[] linesArray = CvInvoke.HoughLinesP(edges, rho, theta, threshold, minLineLength, maxLineGap);

            // Draw detected lines on original image
            foreach (LineSegment2D line in linesArray)
            {
                CvInvoke.Line(image, line.P1, line.P2, new MCvScalar(0, 0, 255), 2);
            }

            return image;
        }

        /// <summary>
        /// Skeletonize the input binary image using the Zhang-Suen algorithm.
        /// </summary>
        /// <param name="inputMat">The input binary image to be skeletonized.</param>
        /// <returns>The skeletonized binary image.</returns>
        public static Mat Skeletonize(Mat inputMat)
        {
            // Krok 1: Utworzenie pustego obrazu do przechowania szkieletu
            Mat skeleton = new Mat(inputMat.Size, DepthType.Cv8U, 1);
            Mat imCopy = inputMat.Clone();

            Mat element = CvInvoke.GetStructuringElement(ElementShape.Cross, new Size(3, 3), new Point(-1, -1));
            BorderType borderType = BorderType.Default; // Ustawienie typu brzegu na domyślny

            // Krok 4: Wykonanie operacji erozji na oryginalnym obrazie oraz poprawienie szkieletu
            while (true)
            {
                // Krok 2: Wykonanie operacji otwarcia morfologicznego na obrazie oryginalnym
                Mat imOpen = new Mat();
                CvInvoke.MorphologyEx(imCopy, imOpen, MorphOp.Open, element, new Point(1, 1), 1, borderType, new MCvScalar());

                // Krok 3: Odjęcie im_open od obrazu oryginalnego
                Mat imTemp = new Mat();
                CvInvoke.Subtract(imCopy, imOpen, imTemp);

                // Erozja morfologiczna
                Mat imEroded = new Mat();
                CvInvoke.Erode(imCopy, imEroded, element, new Point(1, 1), 1, borderType, new MCvScalar());

                // Aktualizacja szkieletu
                CvInvoke.BitwiseOr(skeleton, imTemp, skeleton);

                // Aktualizacja obrazu przetwarzanego
                imCopy = imEroded.Clone();

                // Krok 5: Przerwij pętlę jeśli nie ma już obiektów w obrazie
                if (CvInvoke.CountNonZero(imCopy) == 0)
                    break;
            }

            return skeleton;
        }

        public static Mat Watershed(Mat img)
        {
            // Convert to grayscale
            Image<Gray, byte> gray = img.ToImage<Gray, byte>();
            Image<Gray, byte> thresh = gray.CopyBlank();
            CvInvoke.Threshold(gray, thresh, 0, 255, ThresholdType.BinaryInv | ThresholdType.Otsu);

            // Noise removal
            Matrix<byte> kernel = new Matrix<byte>(new Byte[3, 3] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } });
            Image<Gray, Byte> opening = thresh.MorphologyEx(MorphOp.Open, kernel, new Point(-1, -1), 2, BorderType.Default, new MCvScalar());
            Image<Gray, Byte> sureBg = opening.Dilate(3);

            // Finding sure foreground area
            Mat distanceTransform = new Mat();
            CvInvoke.DistanceTransform(opening, distanceTransform, null, DistType.L2, 5);
            double minVal = 0, maxVal = 0;
            Point minLoc = new Point(), maxLoc = new Point();
            CvInvoke.MinMaxLoc(distanceTransform, ref minVal, ref maxVal, ref minLoc, ref maxLoc); // Find distanceTransform.max()

            Mat sureFg0 = new Mat();
            CvInvoke.Threshold(distanceTransform, sureFg0, 0.7 * maxVal, 255, ThresholdType.Binary);
            Mat sureFg = new Mat();
            sureFg0.ConvertTo(sureFg, DepthType.Cv8U); // Convert from float to Byte

            Mat unknown = new Mat();
            CvInvoke.Subtract(sureBg, sureFg, unknown);

            // Marker labelling
            Mat markers = new Mat();
            CvInvoke.ConnectedComponents(sureFg, markers);
            markers = markers + 1;

            Mat zeros = markers - markers; // Create a matrix of zeros (with same type as markers).
            zeros.CopyTo(markers, unknown); // markers[unknown==255] = 0

            // Apply watershed
            CvInvoke.Watershed(img, markers);

            // Create mask for watershed result
            Mat mask = new Mat();
            zeros.SetTo(new MCvScalar(-1)); // Reuse zeros matrix - fill with (-1) values.
            CvInvoke.Compare(markers, zeros, mask, CmpType.Equal);
            mask.ConvertTo(mask, DepthType.Cv8U); // Convert from mask to Byte
            Mat blue = new Mat(img.Rows, img.Cols, DepthType.Cv8U, 3);
            blue.SetTo(new MCvScalar(255, 0, 0)); // Make blue image
            blue.CopyTo(img, mask);

            return img;
        }

        /// <summary>
        /// Perform inpainting on the input image using the provided mask.
        /// </summary>
        /// <param name="image">The input image.</param>
        /// <param name="mask">The mask specifying the areas to be inpainted.</param>
        /// <returns>The inpainted image.</returns>
        public static Mat Inpainting(Mat image, Mat mask)
        {
            Mat result = new Mat(image.Size, image.Depth, image.NumberOfChannels);
            CvInvoke.Inpaint(image, mask, result, inpaintRadius: 3, InpaintType.Telea);
            return result;
        }

        public static Mat GrabCut(Mat inputImage, System.Drawing.Rectangle rectangle)
        {
            Mat mask = new Mat(inputImage.Size, DepthType.Cv8U, 1);
            mask.SetTo(new MCvScalar(0));

            // Create backgroundModel and foregroundModel arrays
            Mat backgroundModel = new Mat(1, 65, DepthType.Cv64F, 1);
            Mat foregroundModel = new Mat(1, 65, DepthType.Cv64F, 1);

            // Fill arrays with zeros
            backgroundModel.SetTo(new MCvScalar(0));
            foregroundModel.SetTo(new MCvScalar(0));

            CvInvoke.GrabCut(inputImage, mask, rectangle, backgroundModel, foregroundModel, 3, GrabcutInitType.InitWithRect);

            // Create Mat objects for comparison
            Mat value2 = new Mat(mask.Size, DepthType.Cv8U, 1);
            value2.SetTo(new MCvScalar(2));
            Mat value0 = new Mat(mask.Size, DepthType.Cv8U, 1);
            value0.SetTo(new MCvScalar(0));

            // Create mask2
            Mat mask2 = new Mat();
            Mat temp1 = new Mat();
            Mat temp2 = new Mat();
            CvInvoke.Compare(mask, value2, dst: temp1, CmpType.Equal);
            CvInvoke.Compare(mask, value0, dst: temp2, CmpType.Equal);
            CvInvoke.BitwiseOr(temp1, temp2, mask2);

            // Convert mask2 to uint8
            mask2.ConvertTo(mask2, DepthType.Cv8U);

            // Create image_segmented
            Mat image_segmented = new Mat();
            CvInvoke.Multiply(inputImage, mask2, image_segmented);

            return image_segmented;
        }
    }
}
