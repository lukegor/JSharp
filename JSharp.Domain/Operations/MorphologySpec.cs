using JSharp.Shared.Imaging;
using JSharp.Utility.Utility;

namespace JSharp.Domain.Operations
{
    /// <summary>
    /// Serializable description of a morphology structuring element.
    /// The native element <see cref="OpenCvSharp.Mat"/> is rebuilt from this at replay;
    /// persistence never stores the materialized Mat.
    /// </summary>
    public sealed record MorphologySpec(ShapeType Shape, int ElementSize, BorderMode Border, int BorderValue);
}
