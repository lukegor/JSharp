using OpenCvSharp;

namespace JSharp.Domain.Operations
{
    public interface IImageOperation<TParams>
        where TParams : OperationParams
    {
        /// <summary>Metadata for catalogs, validation and menus.</summary>
        OperationDescriptor Descriptor { get; }

        /// <summary>Pure transform: never mutates <paramref name="source"/>, returns a new Mat.</summary>
        Mat Apply(Mat source, TParams parameters, CancellationToken cancellationToken = default);
    }
}
