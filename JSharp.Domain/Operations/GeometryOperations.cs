using OpenCvSharp;

namespace JSharp.Domain.Operations
{
    public sealed class Rotate90ClockwiseOperation : IImageOperation<NoParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("geometry.rotate-90", "geometry", "Rotate 90 CW", InputRequirement.Any, typeof(NoParams));

        public Mat Apply(Mat source, NoParams parameters, CancellationToken cancellationToken = default)
        {
            Mat result = new();
            Cv2.Rotate(source, result, RotateFlags.Rotate90Clockwise);
            return result;
        }
    }

    public sealed class Flip180Operation : IImageOperation<NoParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("geometry.flip-180", "geometry", "Flip 180", InputRequirement.Any, typeof(NoParams));

        public Mat Apply(Mat source, NoParams parameters, CancellationToken cancellationToken = default)
        {
            Mat result = new();
            Cv2.Rotate(source, result, RotateFlags.Rotate180);
            return result;
        }
    }

    public sealed class PyramidUpOperation : IImageOperation<NoParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("geometry.pyramid-up", "geometry", "Pyramid Up", InputRequirement.Any, typeof(NoParams));

        public Mat Apply(Mat source, NoParams parameters, CancellationToken cancellationToken = default)
        {
            Mat result = new();
            Cv2.PyrUp(source, result, borderType: BorderTypes.Default);
            return result;
        }
    }

    public sealed class PyramidDownOperation : IImageOperation<NoParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("geometry.pyramid-down", "geometry", "Pyramid Down", InputRequirement.Any, typeof(NoParams));

        public Mat Apply(Mat source, NoParams parameters, CancellationToken cancellationToken = default)
        {
            Mat result = new();
            Cv2.PyrDown(source, result, borderType: BorderTypes.Default);
            return result;
        }
    }
}
