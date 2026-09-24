using System.Runtime.InteropServices;
using JSharp.Utility.Utility;
using OpenCvSharp;

namespace JSharp.Domain.Operations
{
    public sealed record SimpleThresholdParams(int Threshold, SimpleThresholdingMethod Method) : OperationParams;

    public sealed record DualThresholdParams(int MinThreshold, int MaxThreshold, ThresholdingType Mode, bool EnableContrastMode) : OperationParams;

    public sealed class SimpleThresholdOperation : IImageOperation<SimpleThresholdParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("threshold.simple", "threshold", "Simple Threshold", InputRequirement.Grayscale, typeof(SimpleThresholdParams));

        public Mat Apply(Mat source, SimpleThresholdParams parameters, CancellationToken cancellationToken = default)
        {
            Mat result = new();
            switch (parameters.Method)
            {
                case SimpleThresholdingMethod.Standard:
                    Cv2.Threshold(source, result, parameters.Threshold, 255, ThresholdTypes.Binary);
                    break;
                case SimpleThresholdingMethod.Otsu:
                    Cv2.Threshold(source, result, 0, 255, ThresholdTypes.Otsu);
                    break;
                default:
                    throw new OperationValidationException(ErrorCodes.Validation, $"Unknown simple thresholding method '{parameters.Method}'.");
            }

            return result;
        }
    }

    public sealed class AdaptiveThresholdOperation : IImageOperation<NoParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("threshold.adaptive", "threshold", "Adaptive Threshold", InputRequirement.Grayscale, typeof(NoParams));

        public Mat Apply(Mat source, NoParams parameters, CancellationToken cancellationToken = default)
        {
            Mat result = new();
            Cv2.AdaptiveThreshold(source, result, 255, AdaptiveThresholdTypes.MeanC, ThresholdTypes.Binary, 11, 5);
            return result;
        }
    }

    public sealed class DualThresholdOperation : IImageOperation<DualThresholdParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("threshold.dual", "threshold", "Dual Threshold", InputRequirement.Grayscale, typeof(DualThresholdParams));

        public Mat Apply(Mat source, DualThresholdParams parameters, CancellationToken cancellationToken = default)
        {
            int min = parameters.MinThreshold;
            int max = parameters.MaxThreshold;

            Func<byte, byte> mapper = parameters.Mode switch
            {
                ThresholdingType.Standard when parameters.EnableContrastMode => v => v < min ? (byte)0 : v <= max ? (byte)127 : (byte)255,
                ThresholdingType.Standard => v => v >= min && v <= max ? (byte)255 : (byte)0,
                ThresholdingType.Inverse when parameters.EnableContrastMode => v => v < min ? (byte)255 : v <= max ? (byte)127 : (byte)0,
                ThresholdingType.Inverse => v => v >= min && v <= max ? (byte)0 : (byte)255,
                ThresholdingType.PreservingGrayscaleLevelsIdentity when parameters.EnableContrastMode => v => v < min ? (byte)0 : v <= max ? v : (byte)255,
                ThresholdingType.PreservingGrayscaleLevelsIdentity => v => v >= min && v <= max ? v : (byte)0,
                ThresholdingType.PreservingGrayscaleLevelsNegation when parameters.EnableContrastMode => v => v < min ? (byte)0 : v <= max ? (byte)(255 - v) : (byte)255,
                ThresholdingType.PreservingGrayscaleLevelsNegation => v => v >= min && v <= max ? (byte)(255 - v) : (byte)0,
                _ => throw new OperationValidationException(ErrorCodes.Validation, $"Invalid threshold mode: {parameters.Mode}"),
            };

            int totalBytes = source.Rows * source.Cols;
            byte[] buffer = new byte[totalBytes];
            Marshal.Copy(source.Data, buffer, 0, totalBytes);
            for (int i = 0; i < totalBytes; i++)
            {
                buffer[i] = mapper(buffer[i]);
            }

            Mat result = source.Clone();
            Marshal.Copy(buffer, 0, result.Data, totalBytes);
            return result;
        }
    }
}
