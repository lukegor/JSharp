namespace JSharp.Domain.Operations
{
    public sealed class OperationValidationException : Exception
    {
        public string ErrorCode { get; }

        public OperationValidationException(string errorCode, string? message = null)
            : base(message ?? errorCode)
        {
            ErrorCode = errorCode;
        }
    }
}
