using OpenCvSharp;

namespace JSharp.Domain.Operations
{
    public sealed record OperationResult(bool Success, Mat? Output, string? ErrorCode, string? Detail)
    {
        public static OperationResult Ok(Mat output) => new(true, output, null, null);

        public static OperationResult Fail(string errorCode, string? detail = null) => new(false, null, errorCode, detail);
    }
}
