using JSharp.Domain.Operations;
using JSharp.Utility.Utility;

namespace JSharp.Operations
{
    /// <summary>
    /// Everything the generic dispatcher needs to know after the user filled a dialog.
    /// </summary>
    public sealed record OperationExecutionPlan(
        OperationParams? Parameters,
        ColorSpaceType? NewColorSpace = null,
        bool ThenSplitChannels = false,
        int Repeat = 1,
        bool HandledByCollector = false,
        /// <summary>
        /// Executor operation id resolved by fan-out dialogs (convolver, calculator).
        /// Null means the menu id that opened the dialog.
        /// </summary>
        string? OperationId = null,
        /// <summary>
        /// True when the result belongs in a new image window (calculator checkbox),
        /// leaving the focused document untouched.
        /// </summary>
        bool CreateNewWindow = false,
        /// <summary>
        /// Serializable description of the morphology structuring element.
        /// Set by the morphology collector; the native element Mat is rebuilt at replay.
        /// </summary>
        MorphologySpec? Morphology = null);

    public interface IOperationDialogRouter
    {
        /// <summary>Shows the parameter dialog for the operation; null means the user cancelled.</summary>
        Task<OperationExecutionPlan?> TryCollectAsync(string operationId);
    }
}
