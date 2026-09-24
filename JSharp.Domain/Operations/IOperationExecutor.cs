namespace JSharp.Domain.Operations
{
    public interface IOperationExecutor
    {
        Task<OperationResult> ExecuteAsync(OperationRequest request, CancellationToken cancellationToken = default);
    }
}
