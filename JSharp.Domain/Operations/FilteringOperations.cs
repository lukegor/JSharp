using JSharp.Domain.Imaging;
using JSharp.Shared.Imaging;
using OpenCvSharp;

namespace JSharp.Domain.Operations
{
    public sealed record BlurParams(int KernelSize, BorderMode Border) : OperationParams;

    public sealed record GaussianBlurParams(int KernelSize, double SigmaX, double SigmaY, BorderMode Border) : OperationParams;

    public sealed record CustomKernelParams(float[,] Kernel, BorderMode Border, double Delta) : OperationParams;

    public sealed record DoubleConvolutionParams(float[,] Kernel1, float[,] Kernel2, double Delta) : OperationParams;

    public sealed record EdgeDetectionParams(string KernelName, BorderMode Border, double? MinThreshold, double? MaxThreshold) : OperationParams;

    public sealed record MedianFilterParams(int KernelSize) : OperationParams;

    public sealed class BlurOperation : IImageOperation<BlurParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("filter.blur", "filter", "Box Blur", InputRequirement.Any, typeof(BlurParams));

        public Mat Apply(Mat source, BlurParams parameters, CancellationToken cancellationToken = default)
        {
            Mat result = new();
            Cv2.Blur(source, result, new Size(parameters.KernelSize, parameters.KernelSize),
                new Point(-1, -1), CvFacade.ToBorder(parameters.Border));
            return result;
        }
    }

    public sealed class GaussianBlurOperation : IImageOperation<GaussianBlurParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("filter.gaussian-blur", "filter", "Gaussian Blur", InputRequirement.Any, typeof(GaussianBlurParams));

        public Mat Apply(Mat source, GaussianBlurParams parameters, CancellationToken cancellationToken = default)
        {
            Mat result = new();
            Cv2.GaussianBlur(source, result, new Size(parameters.KernelSize, parameters.KernelSize),
                parameters.SigmaX, parameters.SigmaY, CvFacade.ToBorder(parameters.Border));
            return result;
        }
    }

    public sealed class MedianFilterOperation : IImageOperation<MedianFilterParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("filter.median", "filter", "Median Filter", InputRequirement.Grayscale, typeof(MedianFilterParams));

        public Mat Apply(Mat source, MedianFilterParams parameters, CancellationToken cancellationToken = default)
        {
            // Fixes F6: blur the actual source, not an empty padded Mat.
            Mat result = new();
            Cv2.MedianBlur(source, result, parameters.KernelSize);
            return result;
        }
    }

    public sealed class CustomKernelOperation : IImageOperation<CustomKernelParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("filter.custom-kernel", "filter", "Custom Convolution", InputRequirement.Any, typeof(CustomKernelParams));

        public Mat Apply(Mat source, CustomKernelParams parameters, CancellationToken cancellationToken = default)
        {
            float sum = 0f;
            foreach (float value in parameters.Kernel)
            {
                sum += value;
            }

            float[,] effective = parameters.Kernel;
            if (Math.Abs(sum) > 1e-6f)
            {
                effective = new float[parameters.Kernel.GetLength(0), parameters.Kernel.GetLength(1)];
                for (int y = 0; y < parameters.Kernel.GetLength(0); y++)
                {
                    for (int x = 0; x < parameters.Kernel.GetLength(1); x++)
                    {
                        effective[y, x] = parameters.Kernel[y, x] / sum;
                    }
                }
            }

            Mat result = new(source.Size(), source.Type());
            using Mat kernelMatrix = Mat.FromArray(effective);
            Cv2.Filter2D(source, result, -1, kernelMatrix, new Point(-1, -1), parameters.Delta, CvFacade.ToBorder(parameters.Border));
            return result;
        }
    }

    public sealed class DoubleConvolutionOperation : IImageOperation<DoubleConvolutionParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("filter.double-convolution", "filter", "Double Convolution", InputRequirement.Any, typeof(DoubleConvolutionParams));

        public Mat Apply(Mat source, DoubleConvolutionParams parameters, CancellationToken cancellationToken = default)
        {
            int padding = parameters.Kernel1.GetLength(0) / 2;
            int cropOffset = padding * 2; // the image was padded twice before filtering
            int width = source.Cols;
            int height = source.Rows;

            using Mat padded = PadBy(source, padding);
            using Mat firstPass = new(padded.Size(), padded.Type());
            using (Mat kernel1 = Mat.FromArray(parameters.Kernel1))
            {
                Cv2.Filter2D(padded, firstPass, -1, kernel1, new Point(-1, -1), parameters.Delta, BorderTypes.Default);
            }

            using Mat paddedAgain = PadBy(firstPass, padding);
            Mat secondPass = new(paddedAgain.Size(), paddedAgain.Type());
            using (Mat kernel2 = Mat.FromArray(parameters.Kernel2))
            {
                Cv2.Filter2D(paddedAgain, secondPass, -1, kernel2, new Point(-1, -1), parameters.Delta, BorderTypes.Default);
            }

            // Crop back to the original geometry so callers keep a same-size image.
            using (Mat region = new(secondPass, new Rect(cropOffset, cropOffset, width, height)))
            {
                Mat cropped = region.Clone();
                secondPass.Dispose();
                return cropped;
            }
        }

        private static Mat PadBy(Mat image, int padding)
        {
            Mat padded = new();
            Cv2.CopyMakeBorder(image, padded, padding, padding, padding, padding, BorderTypes.Constant, new Scalar());
            return padded;
        }
    }

    public sealed class EdgeDetectionOperation : IImageOperation<EdgeDetectionParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("filter.edge-detection", "filter", "Edge Detection", InputRequirement.Grayscale, typeof(EdgeDetectionParams));

        public Mat Apply(Mat source, EdgeDetectionParams parameters, CancellationToken cancellationToken = default)
        {
            Mat raw = new();
            if (parameters.KernelName == EdgeKernelNames.SobelNS)
            {
                Cv2.Sobel(source, raw, MatType.CV_64FC1, 1, 0, 3, 1, 0, CvFacade.ToBorder(parameters.Border));
            }
            else if (parameters.KernelName == EdgeKernelNames.SobelEW)
            {
                Cv2.Sobel(source, raw, MatType.CV_64FC1, 0, 1, 3, 1, 0, CvFacade.ToBorder(parameters.Border));
            }
            else if (parameters.KernelName == EdgeKernelNames.Canny)
            {
                if (parameters.MinThreshold is not double min || parameters.MaxThreshold is not double max)
                {
                    throw new OperationValidationException(ErrorCodes.Validation,
                        "The Canny kernel requires the 'min' and 'max' thresholds to be set.");
                }

                Cv2.Canny(source, raw, min, max);
            }
            else
            {
                throw new OperationValidationException(ErrorCodes.Validation, "Invalid kernel specified: " + parameters.KernelName);
            }

            // beta = 0 keeps true black backgrounds (the old beta = 1 lifted every pixel by one).
            Mat result = new();
            Cv2.ConvertScaleAbs(raw, result, 1.0, 0.0);
            return result;
        }
    }
}
