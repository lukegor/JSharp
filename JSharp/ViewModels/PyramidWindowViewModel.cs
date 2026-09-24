using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.ViewModels.Abstractions;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="PyramidWindow"/>.
    /// </summary>
    internal partial class PyramidWindowViewModel : DialogViewModel<int>
    {
        [ObservableProperty]
        public partial int EffectSize { get; set; } = 2;

        [RelayCommand]
        private void Confirm() => Complete(EffectSize);
    }
}
