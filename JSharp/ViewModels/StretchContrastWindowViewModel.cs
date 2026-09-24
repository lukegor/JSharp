using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Domain.Operations;
using JSharp.ViewModels.Abstractions;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="JSharp.UI.Views.StretchContrastWindow"/>.
    /// </summary>
    internal partial class StretchContrastWindowViewModel : DialogViewModel<StretchContrastParams>
    {
        [ObservableProperty]
        public partial int Q3 { get; set; }

        [ObservableProperty]
        public partial int Q4 { get; set; } = 255;

        [ObservableProperty]
        public partial int P1 { get; set; }

        [ObservableProperty]
        public partial int P2 { get; set; } = 255;

        [RelayCommand]
        private void Confirm() => Complete(new StretchContrastParams(P1, P2, Q3, Q4));
    }
}
