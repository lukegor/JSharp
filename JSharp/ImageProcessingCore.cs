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

        public static Mat ApplyBlur(Mat inputImage, BorderType borderType, int kernelSize = 3)
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

        public static Mat ApplyMedianFilter(Mat inputGrayImage, int kernelSize, BorderType borderType)
        {
            Mat result = new Mat(inputGrayImage.Size, DepthType.Cv8U, Constants.Grayscale_ChannelCount);
            Mat paddedImage = new Mat();

            int padding = kernelSize / 2;

            CvInvoke.CopyMakeBorder(inputGrayImage, paddedImage, padding, padding, padding, padding, borderType, new MCvScalar());
            CvInvoke.MedianBlur(paddedImage, result, kernelSize);
            //int height = paddedImage.Height;
            //int width = paddedImage.Width;
            //int paddedStep = paddedImage.Step;
            //int resultStep = result.Step;

            //for (int y = padding; y < height - padding; y++)
            //{
            //    for (int x = padding; x < width - padding; x++)
            //    {
            //        List<byte> values = new List<byte>();

            //        for (int i = -padding; i <= padding; i++)
            //        {
            //            for (int j = -padding; j <= padding; j++)
            //            {
            //                IntPtr pixelPtr = paddedImage.DataPointer + (y + i) * paddedStep + (x + j) * paddedImage.ElementSize;
            //                byte pixelValue = Marshal.ReadByte(pixelPtr);
            //                values.Add(pixelValue);
            //            }
            //        }

            //        values.Sort();
            //        byte medianValue = values[values.Count / 2];

            //        IntPtr outputPtr = result.DataPointer + (y - padding) * resultStep + (x - padding) * result.ElementSize;
            //        Marshal.WriteByte(outputPtr, medianValue);
            //    }
            //}

            return result;
        }

        //public static Mat Add(Mat image1, Mat image2, PixelOverflowHandlingType overflowHandling, double weight = 1.0)
        //{
        //    Mat resultMat = new Mat(image1.Rows, image1.Cols, image1.Depth, image1.NumberOfChannels);

        //    IntPtr ptr1 = image1.DataPointer;
        //    IntPtr ptr2 = image2.DataPointer;
        //    IntPtr resultPtr = resultMat.DataPointer;

        //    for (int y = 0; y < image1.Rows; y++)
        //    {
        //        for (int x = 0; x < image1.Cols; x++)
        //        {
        //            int offset1 = y * image1.Step + x;
        //            int offset2 = y * image2.Step + x;
        //            int resultOffset = y * resultMat.Step + x;

        //            IntPtr currentPtr1 = IntPtr.Add(ptr1, offset1);
        //            IntPtr currentPtr2 = IntPtr.Add(ptr2, offset2);
        //            IntPtr currentResultPtr = IntPtr.Add(resultPtr, resultOffset);

        //            byte pixel1 = Marshal.ReadByte(currentPtr1);
        //            byte pixel2 = Marshal.ReadByte(currentPtr2);
        //            byte newPixel = overflowHandling switch
        //            {
        //                PixelOverflowHandlingType.None_Clipping => (byte)Math.Min(255, pixel1 + pixel2),
        //                PixelOverflowHandlingType.Weights => (byte)Math.Min(255, pixel1 + weight * pixel2),
        //                PixelOverflowHandlingType.Modulo => (byte)((pixel1 + pixel2) % 256),
        //                PixelOverflowHandlingType.LinearScaling => (byte)((pixel1 + pixel2) / 2),
        //                PixelOverflowHandlingType.AdaptiveScaling => CalculateAdaptiveScaling(pixel1, pixel2),
        //                _ => throw new ArgumentException("Invalid overflow handling option."),
        //            };
        //            Marshal.WriteByte(currentResultPtr, newPixel);
        //        }
        //    }

        //    return resultMat;
        //}

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

        public static Mat BitwiseAnd(Mat image1, Mat image2)
        {
            Mat result = new Mat();
            CvInvoke.BitwiseAnd(image1, image2, result);
            return result;
        }

        public static Mat BitwiseOr(Mat image1, Mat image2)
        {
            Mat result = new Mat();
            CvInvoke.BitwiseOr(image1, image2, result);
            return result;
        }

        public static Mat BitwiseNot(Mat image1)
        {
            Mat result = new Mat();
            CvInvoke.BitwiseNot(image1, result);
            return result;
        }

        public static Mat BitwiseXor(Mat image1, Mat image2)
        {
            Mat result = new Mat();
            CvInvoke.BitwiseXor(image1, image2, result);
            return result;
        }

        // Morphological erosion
        public static Mat Erode(Mat image, Mat element, BorderType borderType, MCvScalar borderValue)
        {
            Mat result = new Mat();
            CvInvoke.Erode(image, result, element, new Point(-1, -1), 1, borderType, borderValue);
            return result;
        }

        /// <summary>
        /// Creates Rhombus structuring element
        /// </summary>
        /// <param name="r">promień</param>
        /// <returns></returns>
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

        // Morphological dilation
        public static Mat Dilate(Mat image, Mat element, BorderType borderType, MCvScalar borderValue)
        {
            Mat result = new Mat();
            CvInvoke.Dilate(image, result, element, new Point(-1, -1), 1, borderType, borderValue);
            return result;
        }

        // Morphological opening
        public static Mat MorphologicalOpen(Mat image, Mat element, BorderType borderType, MCvScalar borderValue)
        {
            Mat result = new Mat();
            CvInvoke.MorphologyEx(image, result, MorphOp.Open, element, new Point(-1, -1), 1, borderType, borderValue);
            return result;
        }

        // Morphological closing
        public static Mat MorphologicalClose(Mat image, Mat element, BorderType borderType, MCvScalar borderValue)
        {
            Mat result = new Mat();
            CvInvoke.MorphologyEx(image, result, MorphOp.Close, element, new Point(-1, -1), 1, borderType, borderValue);
            return result;
        }

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
                case SimpleThresholdingMethod.Adaptive:
                    int odd = 11;
                    int constantSubtracter = 5;
                    CvInvoke.AdaptiveThreshold(image, result, 255, AdaptiveThresholdType.MeanC, ThresholdType.Binary, odd, constantSubtracter);
                    break;
                case SimpleThresholdingMethod.Otsu:
                    thresholdType = ThresholdType.Otsu;
                    CvInvoke.Threshold(image, result, 0, 255, thresholdType);
                    break;
            };

            return result;
        }

        public static Mat Threshold(Mat image, int minThreshold, int maxThreshold, ThresholdingType mode)
        {
            IntPtr imageDataPtr = image.DataPointer;

            // Iterate through each pixel in the image
            for (int i = 0; i < image.Rows * image.Cols; i++)
            {
                // Read the pixel value at the current position
                byte pixelValue = Marshal.ReadByte(imageDataPtr, i);

                // Apply thresholding
                switch (mode)
                {
                    case ThresholdingType.Standard:
                        Marshal.WriteByte(imageDataPtr, i, (pixelValue >= minThreshold && pixelValue <= maxThreshold) ? (byte)255 : (byte)0);
                        break;
                    case ThresholdingType.Inverse:
                        Marshal.WriteByte(imageDataPtr, i, (pixelValue >= minThreshold && pixelValue <= maxThreshold) ? (byte)0 : (byte)255);
                        break;
                    case ThresholdingType.PreservingGrayscaleLevelsIdentity:
                        if (!(pixelValue >= minThreshold && pixelValue <= maxThreshold))
                            Marshal.WriteByte(imageDataPtr, i, 0); // Set pixel to black
                        break;
                    case ThresholdingType.PreservingGrayscaleLevelsNegation:
                        if (pixelValue >= minThreshold && pixelValue <= maxThreshold)
                        {
                            Marshal.WriteByte(imageDataPtr, i, (byte)((byte)255 - pixelValue));
                        }
                        else
                        {
                            Marshal.WriteByte(imageDataPtr, i, 0);
                        }
                        break;
                    default:
                        throw new ArgumentOutOfRangeException(nameof(mode), mode, "Invalid threshold mode.");
                }
            }

            return image;
        }

        public static VectorOfVectorOfPoint AnalyseImage(Mat image, RetrType retrType, ChainApproxMethod chainApproxMethod)
        {
            VectorOfVectorOfPoint contours = new VectorOfVectorOfPoint();
            Mat hierarchy = new Mat();
            CvInvoke.FindContours(image, contours, hierarchy, RetrType.List, ChainApproxMethod.ChainApproxNone);

            return contours;
        }

        //public static (double[,] moments, double[] area, double[] perimeter, double[] aspectRatio, double[] extent) AnalyzeContours(VectorOfVectorOfPoint contours)
        //{
        //    // Initialize lists to store features
        //    List<double[]> momentsList = new List<double[]>();
        //    List<double> areaList = new List<double>();
        //    List<double> perimeterList = new List<double>();
        //    List<double> aspectRatioList = new List<double>();
        //    List<double> extentList = new List<double>();

        //    foreach (var contour in contours.ToArrayOfArray())
        //    {
        //        // Moments
        //        Moments m = CvInvoke.Moments(contour);

        //        // Calculate Hu moments from central moments
        //        //double[] huMoments = new double[7];
        //        //CvInvoke.HuMoments(m, huMoments);

        //        // Store Hu moments
        //        //momentsList.Add(huMoments);

        //        // Area and perimeter
        //        double area = CvInvoke.ContourArea(contour as VectorOfPoint);
        //        double perimeter = CvInvoke.ArcLength(contour, true);

        //        areaList.Add(area);
        //        perimeterList.Add(perimeter);

        //        // Bounding box
        //        Rectangle boundingBox = CvInvoke.BoundingRectangle(contour);
        //        double boundingBoxArea = boundingBox.Width * boundingBox.Height;

        //        // Aspect ratio
        //        double aspectRatio = boundingBox.Width / (double)boundingBox.Height;
        //        aspectRatioList.Add(aspectRatio);

        //        // Extent
        //        double extent = area / boundingBoxArea;
        //        extentList.Add(extent);
        //    }

            // Convert lists to arrays
        //    double[,] moments = momentsList.ToArray();
        //    double[] area = areaList.ToArray();
        //    double[] perimeter = perimeterList.ToArray();
        //    double[] aspectRatio = aspectRatioList.ToArray();
        //    double[] extent = extentList.ToArray();

        //    return (moments, area, perimeter, aspectRatio, extent);
        //}

        public static int CountObjectsInImage(Mat image)
        {
            Mat labels = new Mat();
            Mat stats = new Mat();
            Mat centroids = new Mat();
            int nLabels = CvInvoke.ConnectedComponentsWithStats(image, labels, stats, centroids);

            return nLabels - 1;
        }

        public static int CountObjectsInImage(Mat image, int? minSize, int? maxSize)
        {
            Mat labels = new Mat();
            Mat stats = new Mat();
            Mat centroids = new Mat();
            int nLabels = CvInvoke.ConnectedComponentsWithStats(image, labels, stats, centroids);

            int countInRange = 0;
            int[,] areaData = (int[,])stats.GetData(); // Get all data from stats matrix

            for (int label = 1; label < nLabels; label++) // Start from 1 to exclude background
            {
                // Retrieve the size of the current object
                int objSize = areaData[label, 4]; // 4th column represents the area

                // Check if the object size falls within the specified range
                bool withinRange = true;

                if (minSize.HasValue && objSize < minSize)
                    withinRange = false;

                if (maxSize.HasValue && objSize > maxSize)
                    withinRange = false;

                if (withinRange)
                    countInRange++;
            }

            return countInRange;
        }

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

        public static int MyWatershed(Mat image)
        {
            Mat imgGray = new Mat();
            //CvInvoke.CvtColor(image, imgGray, ColorConversion.Bgr2Gray);

            // Progowanie obrazu
            Mat thresh = new Mat(image.Size, image.Depth, image.NumberOfChannels);
            CvInvoke.Threshold(image, thresh, 0, 255, ThresholdType.BinaryInv | ThresholdType.Otsu);

            // Odszumianie obrazu przez operacje morfologiczne
            Mat kernel = CvInvoke.GetStructuringElement(ElementShape.Rectangle, new Size(3, 3), new Point(-1, -1));
            Mat opening = new Mat();
            CvInvoke.MorphologyEx(thresh, opening, MorphOp.Open, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            // Wyznaczenie jednoznacznych obszarów tła
            Mat sureBg = new Mat();
            CvInvoke.Dilate(opening, sureBg, kernel, new Point(-1, -1), 1, BorderType.Default, new MCvScalar());

            // Transformata odległościowa
            Mat distTransform = new Mat();
            CvInvoke.DistanceTransform(opening, distTransform, null, DistType.L2, 5);

            // Określenie jednoznacznych obszarów obiektów przez progowanie obrazu transformaty odległościowej
            double maxDist = 0, minDist = 0;
            Point maxLoc = new Point(), minLoc = new Point();
            CvInvoke.MinMaxLoc(distTransform, ref minDist, ref maxDist, ref minLoc, ref maxLoc);
            double threshValue = 0.5 * maxDist;
            Mat sureFg = new Mat(distTransform.Size, DepthType.Cv8U, 1);
            CvInvoke.Threshold(distTransform, sureFg, distTransform, 255, ThresholdType.Binary);

            // Wyznaczenie obszarów "niepewnych"
            Mat unknown = new Mat(sureBg.Size, sureBg.Depth, sureBg.NumberOfChannels);
            CvInvoke.Subtract(sureBg, sureFg, unknown, null, DepthType.Cv8U);

            // Etykietowanie obiektów
            Mat markers = new Mat(sureFg.Size, DepthType.Cv8U, 1);
            int nLabels = CvInvoke.ConnectedComponents(sureFg, markers);

            // Dodanie wartości 1 do etykiet
            markers += 1;

            // Oznaczenie obszarów "niepewnych" jako zero
            CvInvoke.BitwiseAnd(unknown, new ScalarArray(new MCvScalar(255)), unknown);
            markers.SetTo(new MCvScalar(0), unknown);

            // Algorytm watershed
            CvInvoke.Watershed(image, markers);

            Mat mask = new Mat();
            CvInvoke.Compare(markers, new ScalarArray(new MCvScalar(-1)), mask, CmpType.Equal);

            // Wstawienie linii krawędzi obiektów do obrazu szaroodcieniowego
            Mat imgGrayData = new Mat();
            image.ConvertTo(imgGrayData, DepthType.Cv8U);
            imgGrayData.SetTo(new MCvScalar(255), mask);

            // Wstawienie linii krawędzi obiektów do oryginalnego obrazu kolorowego
            image.SetTo(new MCvScalar(255, 0, 0), mask);

            Console.WriteLine("Znaleziono " + (nLabels - 1) + " obiektów.");

            // Wizualizacja
            Mat markersColor = new Mat();
            CvInvoke.ApplyColorMap(markers * 10, markersColor, Emgu.CV.CvEnum.ColorMapType.Jet);
            Mat frame = new Mat();
            CvInvoke.HConcat(image, markersColor, frame);
            CvInvoke.Imshow("Watershed Result", frame);

            return nLabels;
        }

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

        public static (List<(char, int)>, int) RleEncode(string inputString)
        {
            int count = 1;
            char prev = '\0'; // Initialize to null character
            var list = new List<(char, int)>();

            foreach (char character in inputString)
            {
                if (character != prev)
                {
                    if (prev != '\0')
                    {
                        var entry = (prev, count);
                        list.Add(entry);
                    }
                    count = 1;
                    prev = character;
                }
                else
                {
                    count++;
                }
            }

            try
            {
                var entry = (prev, count);
                list.Add(entry);
                return (list, 0);
            }
            catch (Exception e)
            {
                Console.WriteLine($"Exception encountered: {e.Message}");
                return (null, 1);
            }
        }

        //public static Mat CompressMat(Mat inputMat)
        //{
        //    // Convert Mat to string
        //    string matAsString = MatToString(inputMat);

        //    // Compress the string using RLE
        //    (List<(char, int)>, int) compressedData = RleEncode(matAsString);

        //    // Store the compressed data back into a Mat or any other suitable format
        //    // For demonstration purposes, let's just store it as a string
        //    string compressedString = CompressedDataToString(compressedData);

        //    // Convert the compressed string back to a Mat if needed
        //    Mat compressedMat = StringToMat(compressedString);

        //    return compressedMat;
        //}

        //public static string MatToString(Mat inputMat)
        //{
        //    if (inputMat.NumberOfChannels != 1)
        //        throw new ArgumentException("Input Mat must have a single channel (grayscale)");

        //    if (inputMat.Depth != DepthType.Cv8U)
        //        throw new ArgumentException("Input Mat depth must be 8-bit unsigned");

        //    Image<Gray, byte> image = inputMat.ToImage<Gray, byte>();

        //    StringBuilder stringBuilder = new StringBuilder();

        //    // Assuming inputMat is a grayscale image
        //    for (int row = 0; row < image.Rows; row++)
        //    {
        //        for (int col = 0; col < image.Cols; col++)
        //        {
        //            byte pixelValue = (byte)image[row, col].Intensity; // Grayscale pixel value
        //            stringBuilder.Append(pixelValue.ToString() + " ");
        //        }
        //        stringBuilder.AppendLine(); // Move to the next row in the string
        //    }

        //    return stringBuilder.ToString();
        //}
    }
}
