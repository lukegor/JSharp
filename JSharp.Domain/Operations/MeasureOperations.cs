using OpenCvSharp;

namespace JSharp.Domain.Operations
{
    public sealed record MeasureHistogramParams : OperationParams
    {
        private MeasureHistogramParams()
        {
        }

        public static readonly MeasureHistogramParams Default = new();
    }

    public sealed record MeasureParticlesParams(int? MinArea, int? MaxArea) : OperationParams;

    public sealed record MeasureProfileParams(int X1, int Y1, int X2, int Y2) : OperationParams;

    public sealed class MeasureHistogramOperation : IImageOperation<MeasureHistogramParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("measure.histogram", "measure", "Measure Histogram", InputRequirement.Grayscale, typeof(MeasureHistogramParams));

        public Mat Apply(Mat source, MeasureHistogramParams parameters, CancellationToken cancellationToken = default) =>
            source.Clone();
    }

    public sealed class MeasureParticlesOperation : IImageOperation<MeasureParticlesParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("measure.particles", "measure", "Measure Particles", InputRequirement.Any, typeof(MeasureParticlesParams));

        public Mat Apply(Mat source, MeasureParticlesParams parameters, CancellationToken cancellationToken = default) =>
            source.Clone();
    }

    public sealed class MeasureProfileOperation : IImageOperation<MeasureProfileParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("measure.profile", "measure", "Measure Profile", InputRequirement.Grayscale, typeof(MeasureProfileParams));

        public Mat Apply(Mat source, MeasureProfileParams parameters, CancellationToken cancellationToken = default) =>
            source.Clone();
    }
}
