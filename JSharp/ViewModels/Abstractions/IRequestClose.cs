namespace JSharp.ViewModels.Abstractions
{
    /// <summary>
    /// A viewmodel the window/dialog service can close on demand. The teardown contract
    /// from the view/viewmodel management patterns doc (step 5).
    /// </summary>
    internal interface IRequestClose
    {
        event EventHandler? CloseRequested;
    }
}
