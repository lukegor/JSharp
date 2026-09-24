using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.ViewModels.Abstractions;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="PosterizeWindow"/>.
    /// </summary>
    internal partial class PosterizeWindowViewModel : DialogViewModel<int>
    {
        [ObservableProperty]
        public partial int LevelsNumber { get; set; } = 2;

        public PosterizeWindowViewModel()
        {
        }

        [RelayCommand]
        private void Confirm() => Complete(LevelsNumber);
    }
}
