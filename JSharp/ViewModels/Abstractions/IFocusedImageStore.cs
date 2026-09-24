namespace JSharp.ViewModels.Abstractions
{
    /// <summary>
    /// Tracks the currently focused image viewmodel. Written by the shell, read by
    /// features that need the focused document (replaces the former static bridge).
    /// </summary>
    internal interface IFocusedImageStore
    {
        NewImageWindowViewModel? Current { get; }

        event EventHandler? Changed;

        void Set(NewImageWindowViewModel? viewModel);
    }
}
