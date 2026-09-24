using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Shared.Imaging;
using JSharp.Utility.Utility;
using JSharp.ViewModels.Abstractions;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="MedianWindow"/>.
    /// </summary>
    internal partial class MedianWindowViewModel : DialogViewModel<int>
    {
        public IList<string> BorderTypes { get; } = GetEdgePixelsHandlingOptions().ToList();

        [ObservableProperty]
        public partial int MatrixSize { get; set; } = 3;

        [ObservableProperty]
        public partial BorderMode BorderPixelsOption { get; set; } = BorderMode.Isolated;

        [RelayCommand]
        private void Confirm() => Complete(MatrixSize);

        private static IEnumerable<string> GetEdgePixelsHandlingOptions()
        {
            IEnumerable<BorderMode> borderTypes = [BorderMode.Isolated, BorderMode.Reflect, BorderMode.Replicate];
            return BorderTypeHelper.GetLocalizedEdgePixelsHandlingOptions(borderTypes);
        }
    }
}
