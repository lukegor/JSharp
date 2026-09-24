using JSharp.Utility.Utility;
using OpenCvSharp;

namespace JSharp.Domain.Operations
{
    public sealed record ConvertColorParams(ColorSpaceType Target) : OperationParams;

    public sealed class ConvertColorOperation : IImageOperation<ConvertColorParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("color.convert", "color", "Convert Color Space", InputRequirement.Color, typeof(ConvertColorParams));

        public Mat Apply(Mat source, ConvertColorParams parameters, CancellationToken cancellationToken = default)
        {
            ColorConversionCodes conversion = parameters.Target switch
            {
                ColorSpaceType.Grayscale => ColorConversionCodes.BGR2GRAY,
                ColorSpaceType.RGB => ColorConversionCodes.BGR2RGB,
                ColorSpaceType.HSV => ColorConversionCodes.BGR2HSV,
                ColorSpaceType.LAB => ColorConversionCodes.BGR2Lab,
                _ => throw new OperationValidationException(ErrorCodes.Validation, $"Unknown target color space '{parameters.Target}'."),
            };

            Mat result = new();
            Cv2.CvtColor(source, result, conversion);
            return result;
        }
    }
}
