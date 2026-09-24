using OpenCvSharp;

namespace JSharp.Domain.Operations
{
    public sealed record OperationRequest(string OperationId, Mat Source, OperationParams? Parameters);
}
