using JSharp.ViewModels;
using JSharp.ViewModels.Abstractions;

namespace JSharp.Services
{
    internal sealed class FocusedImageStore : IFocusedImageStore
    {
        public NewImageWindowViewModel? Current { get; private set; }

        public event EventHandler? Changed;

        public void Set(NewImageWindowViewModel? viewModel)
        {
            Current = viewModel;
            Changed?.Invoke(this, EventArgs.Empty);
        }
    }
}
