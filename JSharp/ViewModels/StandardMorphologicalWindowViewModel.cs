using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Shared.Imaging;
using JSharp.Utility.Utility;
using JSharp.ViewModels.Abstractions;

namespace JSharp.ViewModels
{
    /// <summary>
    /// The user's morphology choices; the router turns this into a kernel + params.
    /// </summary>
    internal sealed record MorphologySelection(
        ShapeType Shape, BorderMode Border, int BorderValue, int ElementSize);

    /// <summary>
    /// Viewmodel for <see cref="StandardMorphologicalWindow"/>.
    /// </summary>
    internal partial class StandardMorphologicalWindowViewModel : DialogViewModel<MorphologySelection>
    {
        [ObservableProperty]
        public partial ShapeType Shape { get; set; }

        [ObservableProperty]
        public partial BorderMode BorderPixelsOption { get; set; }

        [ObservableProperty]
        public partial int BorderValue { get; set; }

        [ObservableProperty]
        public partial int ElementSize { get; set; }

        public IReadOnlyList<string> EdgePixelsHandlingOptions { get; }

        public IReadOnlyList<string> ShapeTypeOptions { get; }

        public StandardMorphologicalWindowViewModel()
        {
            EdgePixelsHandlingOptions = BorderTypeHelper.GetLocalizedEdgePixelsHandlingOptions(
                [BorderMode.Isolated, BorderMode.Reflect, BorderMode.Replicate, BorderMode.Reflect101, BorderMode.Wrap]).ToList();
            ShapeTypeOptions = ShapeTypeHelper.GetLocalizedShapeTypes(
                [ShapeType.Rhombus, ShapeType.Rectangle]).ToList();

            Shape = ShapeType.Rhombus;
            BorderPixelsOption = BorderMode.Isolated;
            BorderValue = 0;
            ElementSize = 3;
        }

        [RelayCommand]
        private void Confirm() => Complete(new MorphologySelection(Shape, BorderPixelsOption, BorderValue, ElementSize));
    }
}
