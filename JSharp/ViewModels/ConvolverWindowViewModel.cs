using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Domain.Models.SimpleDataModels;
using JSharp.Shared.Imaging;
using JSharp.Shared.Resources;
using JSharp.Utility.Utility;
using JSharp.ViewModels.Abstractions;
using System.Collections.ObjectModel;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="ConvolverWindow"/>. Confirm completes a
    /// <see cref="ConvolutionInfo"/>; the Canny case nests a TwoParams dialog
    /// through <see cref="IDialogService"/>.
    /// </summary>
    internal partial class ConvolverWindowViewModel : DialogViewModel<ConvolutionInfo>
    {
        private readonly IDialogService _dialogs;
        private bool _isPredefinedOptionSelected;

        public IList<string> BorderTypes { get; } = GetEdgePixelsHandlingOptions().ToList();

        [ObservableProperty]
        public partial ObservableCollection<int> TextBoxValues { get; set; }

        [ObservableProperty]
        public partial string CurrentKernel { get; set; } = string.Empty;

        [ObservableProperty]
        public partial BorderMode BorderPixelsOption { get; set; }

        public ConvolverWindowViewModel(IDialogService dialogs)
        {
            _dialogs = dialogs;
            TextBoxValues = new ObservableCollection<int>(
                KernelMappings.IdentityArray.Cast<int>().ToArray());
            BorderPixelsOption = BorderTypeHelper.BorderizeLocalizedBorderType(Kernels.BorderTypeIsolated);
        }

        [RelayCommand]
        private void UpdateKernelTextBoxes(string? kernelType)
        {
            _isPredefinedOptionSelected = true;

            if (kernelType != null && KernelMappings.TryGetArray(kernelType, out int[,] kernelArray))
            {
                UpdateTextBoxValues(kernelArray);
                CurrentKernel = DetectKernelType();
            }
            else if (kernelType == Kernels.Canny)
            {
                CurrentKernel = Kernels.Canny;
            }

            _isPredefinedOptionSelected = false;
        }

        [RelayCommand]
        private void Apply()
        {
            ConvolutionInfo convolutionInfo = new(BorderPixelsOption);

            if (CurrentKernel == Kernels.Canny)
            {
                TwoParamsVMInfo vmInfo = new(
                    new SliderProperties(0, 255, 100),
                    new SliderProperties(0, 255, 200),
                    "Canny filter thresholds:");

                TwoParamsWindowViewModel twoParamsVm = new(vmInfo);
                (bool confirmed, (int min, int max) thresholds) =
                    _dialogs.ShowDialog<TwoParamsWindowViewModel, (int, int)>(twoParamsVm);
                if (confirmed)
                {
                    convolutionInfo.Min = thresholds.min;
                    convolutionInfo.Max = thresholds.max;
                }
            }

            Complete(convolutionInfo);
        }

        internal void KernelInputCell_TextChanged()
        {
            if (!_isPredefinedOptionSelected)
            {
                CurrentKernel = DetectKernelType();
            }
        }

        private string DetectKernelType()
        {
            int[,] currentValues = new int[3, 3];
            int index = 0;
            foreach (int value in TextBoxValues)
            {
                currentValues[index / 3, index % 3] = value;
                index++;
            }

            return KernelMappings.GetDisplayName(currentValues);
        }

        private void UpdateTextBoxValues(int[,] values)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    TextBoxValues[i * 3 + j] = values[i, j];
                }
            }
        }

        private static IEnumerable<string> GetEdgePixelsHandlingOptions()
        {
            IEnumerable<BorderMode> borderTypes = [BorderMode.Isolated, BorderMode.Reflect, BorderMode.Replicate];
            return BorderTypeHelper.GetLocalizedEdgePixelsHandlingOptions(borderTypes);
        }
    }
}
