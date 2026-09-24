using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using OpenCvSharp;
using System.Diagnostics;

namespace JSharp.Domain.Operations
{
    public sealed class OperationExecutor : IOperationExecutor
    {
        private readonly IOperationCatalog _catalog;
        private readonly ILogger<OperationExecutor> _logger;

        public OperationExecutor(IOperationCatalog catalog, ILogger<OperationExecutor>? logger = null)
        {
            _catalog = catalog;
            _logger = logger ?? NullLogger<OperationExecutor>.Instance;
        }

        public async Task<OperationResult> ExecuteAsync(OperationRequest request, CancellationToken cancellationToken = default)
        {
            // An unknown id is a programming error at the call site - let it propagate
            // instead of disguising it as an execution failure.
            OperationDescriptor descriptor = _catalog.GetDescriptor(request.OperationId);

            // Capture dimensions before execution: the caller may dispose the source
            // while we await (e.g. undo/redo swaps the document's Mat).
            Mat source = request.Source;
            int width = source.Width;
            int height = source.Height;
            int channels = source.Channels();

            long startedAt = Stopwatch.GetTimestamp();

            OperationResult result = await ExecuteCoreAsync(descriptor, request, cancellationToken)
                .ConfigureAwait(false);

            _logger.LogInformation(
                "Operation {OperationId} {Outcome} in {DurationMs:F1} ms on {Width}x{Height}x{Channels} image.",
                request.OperationId,
                result.Success ? "succeeded" : result.ErrorCode ?? ErrorCodes.Unexpected,
                Stopwatch.GetElapsedTime(startedAt).TotalMilliseconds,
                width,
                height,
                channels);

            return result;
        }

        private async Task<OperationResult> ExecuteCoreAsync(OperationDescriptor descriptor, OperationRequest request, CancellationToken cancellationToken)
        {
            try
            {
                ValidateInputKind(descriptor, request.Source);
                cancellationToken.ThrowIfCancellationRequested();

                Mat output = await _catalog
                    .InvokeAsync(request.OperationId, request.Source, request.Parameters, cancellationToken)
                    .ConfigureAwait(false);

                return OperationResult.Ok(output);
            }
            catch (OperationValidationException ex)
            {
                _logger.LogWarning("Validation failed for {OperationId}: {Detail}", request.OperationId, ex.Message);
                return OperationResult.Fail(ex.ErrorCode, ex.Message);
            }
            catch (OperationCanceledException)
            {
                _logger.LogInformation("Operation {OperationId} was cancelled.", request.OperationId);
                return OperationResult.Fail(ErrorCodes.Cancelled);
            }
            catch (Exception ex)
            {
                // Log the full exception (stack + inner chain) before reducing it to a result detail.
                _logger.LogError(ex, "Operation {OperationId} failed unexpectedly.", request.OperationId);
                return OperationResult.Fail(ErrorCodes.Unexpected, ex.Message);
            }
        }

        private static void ValidateInputKind(OperationDescriptor descriptor, Mat source)
        {
            ArgumentNullException.ThrowIfNull(source);

            if (source.Empty() || source.Width == 0 || source.Height == 0)
            {
                throw new OperationValidationException(
                    ErrorCodes.Validation,
                    $"Operation '{descriptor.Id}' received an empty image.");
            }

            if (descriptor.Input == InputRequirement.Grayscale && source.Channels() != 1)
            {
                throw new OperationValidationException(
                    ErrorCodes.InputMismatchGray,
                    $"Operation '{descriptor.Id}' requires a grayscale input.");
            }

            if (descriptor.Input == InputRequirement.Color && source.Channels() != 3)
            {
                throw new OperationValidationException(
                    ErrorCodes.InputMismatchColor,
                    $"Operation '{descriptor.Id}' requires a color input.");
            }
        }
    }
}
