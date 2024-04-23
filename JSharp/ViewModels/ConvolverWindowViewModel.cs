using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Flann;
using JSharp.Models;
using JSharp.Resources;
using JSharp.Utility;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Collections.Specialized;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.ViewModels
{
    internal class ConvolverWindowViewModel : BindableBase
    {
        public event EventHandler<ConvolutionInfo> KernelAppliable;

        private ObservableCollection<int> _textBoxValues;
        public ObservableCollection<int> TextBoxValues
        {
            get { return _textBoxValues; }
            set { SetProperty(ref _textBoxValues, value); }
        }
        private string _currentKernel;
        public string CurrentKernel
        {
            get { return _currentKernel; }
            set { SetProperty(ref _currentKernel, value); }
        }
        private BorderType _borderPixelsOption;
        public BorderType BorderPixelsOption
        {
            get { return _borderPixelsOption; }
            set { SetProperty(ref _borderPixelsOption, value); }
        }

        private bool isPredefinedOptionSelected = false;

        public DelegateCommand<string> UpdateKernelTextBoxesCommand { get; private set; }
        public DelegateCommand BtnApply_ClickCommand { get; private set; }
        public ConvolverWindowViewModel()
        {
            UpdateKernelTextBoxesCommand = new DelegateCommand<string>(UpdateKernelTextBoxes);
            BtnApply_ClickCommand = new DelegateCommand(BtnApply_Click);
            this.TextBoxValues = new ObservableCollection<int>(KernelMappings.KernelNameToArray[Kernels.Identity]
                                                                        .Cast<int>()
                                                                        .ToArray());
            this.BorderPixelsOption = BorderTypeHelper.BorderizeLocalizedBorderType(Kernels.BorderTypeIsolated);
        }

        private void BtnApply_Click()
        {
            Mat focusedImage = MainWindowViewModel.FocusedImage.MatImage;
            ConvolutionInfo convolutionInfo = new ConvolutionInfo(this.BorderPixelsOption);

            if (CurrentKernel == Kernels.Canny)
            {
                TwoParamsVMInfo vmInfo = new TwoParamsVMInfo(new SliderProperties(0, 255, 100), new SliderProperties(0, 255, 200), "Canny filter thresholds:");

                TwoParamsWindowViewModel twoParamsWindowViewModel = new TwoParamsWindowViewModel(vmInfo);
                TwoParamsWindow twoParamsWindow = new TwoParamsWindow(Constants.CannySettingsWindowTitle);
                twoParamsWindow.DataContext = twoParamsWindowViewModel;

                twoParamsWindow.ShowDialog();

                if (twoParamsWindow.DialogResult == true)
                {
                    convolutionInfo.Min = twoParamsWindowViewModel.Min;
                    convolutionInfo.Max = twoParamsWindowViewModel.Max;
                }
            }

            KernelAppliable?.Invoke(this, convolutionInfo);
        }

        internal void KernelInputCell_TextChanged()
        {
            if (!isPredefinedOptionSelected)
            {
                CurrentKernel = DetectKernelType(); // Perform detection after TextBoxValues is updated
            }
        }

        private string DetectKernelType()
        {
            // Read values from ObservableCollection<int> into a 2D array
            int[,] currentValues = new int[3, 3];
            int index = 0;
            foreach (int value in TextBoxValues)
            {
                currentValues[index / 3, index % 3] = value;
                index++;
            }

            // Check if current values match any predefined kernel pattern
            foreach (var kernel in KernelMappings.KernelNameToArray)
            {
                if (ArraysAreEqual(currentValues, kernel.Value))
                {
                    //return predefined name
                    return kernel.Key;
                }
            }
            // Return "custom" if no match found (mask isn't predefined)
            return Kernels.Custom;
        }

        // Helper method to check if two 2D arrays are equal
        private bool ArraysAreEqual(int[,] array1, int[,] array2)
        {
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    if (array1[i, j] != array2[i, j])
                    {
                        return false;
                    }
                }
            }
            return true;
        }

        private void UpdateKernelTextBoxes(string kernelType)
        {
            // Set the flag when a predefined option is selected
            isPredefinedOptionSelected = true;

            int[,] kernelArray;
            if (KernelMappings.KernelNameToArray.TryGetValue(kernelType, out kernelArray))
            {
                // Apply the kernel by updating the text boxes
                UpdateTextBoxValues(kernelArray);

                // Check kernel type after all TextBoxes have been updated
                CurrentKernel = DetectKernelType();
                UpdateCellsVisibility();
            }
            else if (kernelType == Kernels.Canny)
            {
                CurrentKernel = Kernels.Canny;
                UpdateCellsVisibility();
            }
            // Reset the flag
            isPredefinedOptionSelected = false;
        }

        private void UpdateCellsVisibility()
        {
            var window = App.Current.Windows.OfType<ConvolverWindow>().FirstOrDefault(x => x.DataContext == this);
            window.CurrentKernelText_TextChanged(CurrentKernel);
        }

        private void UpdateTextBoxValues(int[,] values)
        {
            // Update TextBox values based on the provided matrix
            for (int i = 0; i < 3; i++)
            {
                for (int j = 0; j < 3; j++)
                {
                    int index = i * 3 + j;
                    TextBoxValues[index] = values[i, j];
                }
            }
        }

        //public BorderType[] GetEdgePixelsHandlingOptions()
        //{
        //    BorderType[] borderTypes = { BorderType.Isolated, BorderType.Reflect, BorderType.Replicate };
        //    return borderTypes;
        //}

        public IEnumerable<string> GetEdgePixelsHandlingOptions()
        {
            IEnumerable<BorderType> borderTypes = [BorderType.Isolated, BorderType.Reflect, BorderType.Replicate];
            return BorderTypeHelper.GetLocalizedEdgePixelsHandlingOptions(borderTypes);
        }
    }
}
