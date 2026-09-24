using System.Globalization;
using System.Text;
using JSharp.Domain.Operations;
using JSharp.Shared.Resources;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Maps stable domain error codes to localized, actionable user messages.
    /// The only place in the app where an ErrorCode becomes visible text.
    /// </summary>
    internal static class OperationResultPresenter
    {
        private static readonly CompositeFormat RequiresColorFormat =
            CompositeFormat.Parse(Errors.OperationRequiresColorImage);
        private static readonly CompositeFormat RequiresGrayscaleFormat =
            CompositeFormat.Parse(Errors.OperationRequiresGrayscaleImage);

        public static string ToLocalizedMessage(OperationResult result, string operationName)
        {
            return result.ErrorCode switch
            {
                ErrorCodes.InputMismatchGray => Format(RequiresGrayscaleFormat, operationName),
                ErrorCodes.InputMismatchColor => Format(RequiresColorFormat, operationName),
                ErrorCodes.Cancelled => string.Empty,
                _ => result.Detail ?? result.ErrorCode ?? "Unknown error."
            };
        }

        private static string Format(CompositeFormat format, string operationName) =>
            string.Format(CultureInfo.InvariantCulture, format, operationName);
    }
}
