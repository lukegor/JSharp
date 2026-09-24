using System.Runtime.InteropServices;
using OpenCvSharp;

namespace JSharp.Domain.Operations
{
    public sealed record PosterizeParams(int Levels) : OperationParams;

    public sealed record StretchContrastParams(int P1, int P2, int Q3, int Q4) : OperationParams;

    public sealed record EqualizeHistogramParams(int[] HistogramData) : OperationParams;

    public sealed class NegateOperation : IImageOperation<NoParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("point.negate", "point", "Negate", InputRequirement.Grayscale, typeof(NoParams));

        public Mat Apply(Mat source, NoParams parameters, CancellationToken cancellationToken = default)
        {
            Mat result = new();
            Cv2.BitwiseNot(source, result); // native, stride-safe: fixes F11
            return result;
        }
    }

    public sealed class PosterizeOperation : IImageOperation<PosterizeParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("point.posterize", "point", "Posterize", InputRequirement.Grayscale, typeof(PosterizeParams));

        public Mat Apply(Mat source, PosterizeParams parameters, CancellationToken cancellationToken = default)
        {
            if (parameters.Levels < 2)
            {
                throw new OperationValidationException(ErrorCodes.Validation, "Levels must be >= 2.");
            }

            Mat result = source.Clone();
            System.Diagnostics.Debug.Assert(result.IsContinuous(), "Dense layout assumed: rows*cols copy is only valid on continuous Mats.");
            int totalBytes = result.Rows * result.Cols;
            byte[] buffer = new byte[totalBytes];
            Marshal.Copy(result.Data, buffer, 0, totalBytes);

            int divisor = 255 / (parameters.Levels - 1);
            for (int i = 0; i < totalBytes; ++i)
            {
                buffer[i] = (byte)(Math.Round(buffer[i] / (double)divisor) * divisor);
            }

            Marshal.Copy(buffer, 0, result.Data, totalBytes);
            return result;
        }
    }

    public sealed class StretchHistogramOperation : IImageOperation<NoParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("point.stretch-histogram", "point", "Stretch Histogram", InputRequirement.Grayscale, typeof(NoParams));

        public Mat Apply(Mat source, NoParams parameters, CancellationToken cancellationToken = default)
        {
            Cv2.MinMaxIdx(source, out double min, out double max);
            return new StretchContrastOperation()
                .Apply(source, new StretchContrastParams((int)min, (int)max, 0, 255), cancellationToken);
        }
    }

    public sealed class StretchContrastOperation : IImageOperation<StretchContrastParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("point.stretch-contrast", "point", "Stretch Contrast", InputRequirement.Grayscale, typeof(StretchContrastParams));

        public Mat Apply(Mat source, StretchContrastParams parameters, CancellationToken cancellationToken = default)
        {
            int height = source.Height;
            int width = source.Width;
            int step = (int)source.Step();
            int totalBytes = height * step;

            byte[] buffer = new byte[totalBytes];
            Marshal.Copy(source.Data, buffer, 0, totalBytes);

            byte minValue = 255;
            byte maxValue = 0;
            bool anyInRange = false;
            for (int y = 0; y < height; y++)
            {
                int rowStart = y * step;
                for (int x = 0; x < width; x++)
                {
                    byte value = buffer[rowStart + x];
                    if (value >= parameters.P1 && value <= parameters.P2)
                    {
                        anyInRange = true;
                        if (value < minValue)
                        {
                            minValue = value;
                        }

                        if (value > maxValue)
                        {
                            maxValue = value;
                        }
                    }
                }
            }

            Mat result = source.Clone();
            if (!anyInRange || maxValue == minValue)
            {
                return result; // fixes F7: constant range stretches nothing instead of dividing by zero
            }

            double dynamic = parameters.Q4 - parameters.Q3;
            for (int y = 0; y < height; y++)
            {
                int rowStart = y * step;
                for (int x = 0; x < width; x++)
                {
                    int offset = rowStart + x;
                    byte value = buffer[offset];
                    if (value >= parameters.P1 && value <= parameters.P2)
                    {
                        buffer[offset] = (byte)Math.Round((value - minValue) / (double)(maxValue - minValue) * dynamic + parameters.Q3);
                    }
                }
            }

            Marshal.Copy(buffer, 0, result.Data, totalBytes);
            return result;
        }
    }

    public sealed class EqualizeHistogramOperation : IImageOperation<EqualizeHistogramParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("point.equalize-histogram", "point", "Equalize Histogram", InputRequirement.Grayscale, typeof(EqualizeHistogramParams));

        public Mat Apply(Mat source, EqualizeHistogramParams parameters, CancellationToken cancellationToken = default)
        {
            System.Diagnostics.Debug.Assert(source.IsContinuous(), "Dense layout assumed: rows*cols copy is only valid on continuous Mats.");
            int[] histogramData = parameters.HistogramData;
            if (histogramData.Length != 256)
            {
                throw new OperationValidationException(ErrorCodes.Validation, "Histogram data must contain exactly 256 bins.");
            }

            int total = source.Width * source.Height;
            float scale = 255.0f / total;
            byte[] lut = new byte[256];
            int sum = 0;
            for (int i = 0; i < 256; i++)
            {
                sum += histogramData[i];
                lut[i] = (byte)(sum * scale);
            }

            int totalBytes = source.Rows * source.Cols;
            byte[] buffer = new byte[totalBytes];
            Marshal.Copy(source.Data, buffer, 0, totalBytes);
            for (int i = 0; i < totalBytes; i++)
            {
                buffer[i] = lut[buffer[i]];
            }

            Mat result = new(source.Size(), MatType.CV_8UC1);
            Marshal.Copy(buffer, 0, result.Data, totalBytes);
            return result;
        }
    }
}
