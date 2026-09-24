namespace JSharp.ViewModels.Abstractions
{
    /// <summary>
    /// Shows parameter dialogs modally and returns what the user confirmed.
    /// Confirmed is false when the dialog was dismissed without completing.
    /// </summary>
    internal interface IDialogService
    {
        (bool Confirmed, TResult Result) ShowDialog<TViewModel, TResult>(TViewModel viewModel)
            where TViewModel : DialogViewModel<TResult>;

        /// <summary>
        /// Shows a modeless live-preview window. When the window closes, Result (if the
        /// viewmodel completed) is handed to onConfirm; a dismiss without completing
        /// (cancel, title-bar X) runs onDismissed instead, so live-preview paint can be reverted.
        /// </summary>
        void Show<TViewModel, TResult>(TViewModel viewModel, Func<TResult, Task> onConfirm, Action? onDismissed = null)
            where TViewModel : DialogViewModel<TResult>;
    }
}
