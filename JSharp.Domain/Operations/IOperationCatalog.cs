using System.Diagnostics.CodeAnalysis;
using OpenCvSharp;

namespace JSharp.Domain.Operations
{
    public interface IOperationCatalog
    {
        IReadOnlyCollection<OperationDescriptor> Descriptors { get; }

        OperationDescriptor GetDescriptor(string operationId);

        bool TryGetDescriptor(string operationId, [NotNullWhen(true)] out OperationDescriptor? descriptor);

        Task<Mat> InvokeAsync(string operationId, Mat source, OperationParams? parameters, CancellationToken cancellationToken);
    }
}
