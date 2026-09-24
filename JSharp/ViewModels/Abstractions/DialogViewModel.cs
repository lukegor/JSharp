using CommunityToolkit.Mvvm.ComponentModel;

namespace JSharp.ViewModels.Abstractions
{
    /// <summary>
    /// Base for parameter dialogs. One concept: <see cref="Complete(TResult)"/> to
    /// accept, <see cref="Close"/> to dismiss. The dialog service subscribes
    /// <see cref="CloseRequested"/>, shows the window modally and returns <see cref="Result"/>
    /// afterwards (null = dismissed).
    /// </summary>
    internal abstract class DialogViewModel<TResult> : ObservableObject, IRequestClose
    {
        private bool _completed;

        public event EventHandler? CloseRequested;

        public bool HasResult => _completed;

        public TResult? Result { get; private set; }

        protected void Complete(TResult result)
        {
            _completed = true;
            Result = result;
            CloseRequested?.Invoke(this, EventArgs.Empty);
        }

        protected void Close() => CloseRequested?.Invoke(this, EventArgs.Empty);
    }
}
