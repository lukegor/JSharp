using JSharp.Domain.Imaging;
using JSharp.Shared.Imaging;
using OpenCvSharp;

namespace JSharp.Domain.Operations
{
    public sealed record MorphologyParams(Mat Element, BorderMode Border, Scalar BorderValue) : OperationParams;

    public sealed record DiamondParams(int Radius) : OperationParams;

    public sealed class ErodeOperation : IImageOperation<MorphologyParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("morph.erode", "morphology", "Erode", InputRequirement.Any, typeof(MorphologyParams));

        public Mat Apply(Mat source, MorphologyParams parameters, CancellationToken cancellationToken = default)
        {
            Mat result = new();
            Cv2.Erode(source, result, parameters.Element, new Point(-1, -1), 1, CvFacade.ToBorder(parameters.Border), parameters.BorderValue);
            return result;
        }
    }

    public sealed class DilateOperation : IImageOperation<MorphologyParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("morph.dilate", "morphology", "Dilate", InputRequirement.Any, typeof(MorphologyParams));

        public Mat Apply(Mat source, MorphologyParams parameters, CancellationToken cancellationToken = default)
        {
            Mat result = new();
            Cv2.Dilate(source, result, parameters.Element, new Point(-1, -1), 1, CvFacade.ToBorder(parameters.Border), parameters.BorderValue);
            return result;
        }
    }

    public sealed class MorphologicalOpenOperation : IImageOperation<MorphologyParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("morph.open", "morphology", "Morphological Opening", InputRequirement.Any, typeof(MorphologyParams));

        public Mat Apply(Mat source, MorphologyParams parameters, CancellationToken cancellationToken = default)
        {
            Mat result = new();
            Cv2.MorphologyEx(source, result, MorphTypes.Open, parameters.Element, new Point(-1, -1), 1, CvFacade.ToBorder(parameters.Border), parameters.BorderValue);
            return result;
        }
    }

    public sealed class MorphologicalCloseOperation : IImageOperation<MorphologyParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("morph.close", "morphology", "Morphological Closing", InputRequirement.Any, typeof(MorphologyParams));

        public Mat Apply(Mat source, MorphologyParams parameters, CancellationToken cancellationToken = default)
        {
            Mat result = new();
            Cv2.MorphologyEx(source, result, MorphTypes.Close, parameters.Element, new Point(-1, -1), 1, CvFacade.ToBorder(parameters.Border), parameters.BorderValue);
            return result;
        }
    }

    /// <summary>
    /// Builds the diamond structuring element; its source argument is unused (element factory).
    /// </summary>
    public sealed class DiamondOperation : IImageOperation<DiamondParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("morph.diamond", "morphology", "Diamond Element", InputRequirement.Any, typeof(DiamondParams));

        public Mat Apply(Mat source, DiamondParams parameters, CancellationToken cancellationToken = default) =>
            StructuringElements.Diamond(parameters.Radius);
    }
}
