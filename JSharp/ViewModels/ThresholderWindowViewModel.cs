using Emgu.CV;
using Emgu.CV.Structure;
using JSharp.Utility;
using LiveChartsCore.Measure;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace JSharp.ViewModels
{
    public class ThresholderWindowViewModel : BindableBase
    {
        private int _fromValue;
        /// <summary>
        /// The starting value of the threshold range.
        /// </summary>
        public int FromValue
        {
            get { return _fromValue; }
            set
            {
                SetProperty(ref _fromValue, value);
                AdjustSliderValues();
                UpdatePercentage();
                UpdateImage();
            }
        }

        private int _toValue;
        /// <summary>
        /// The ending value of the threshold range.
        /// </summary>
        public int ToValue
        {
            get { return _toValue; }
            set
            {
                SetProperty(ref _toValue, value);
                AdjustSliderValues();
                UpdatePercentage();
                UpdateImage();
            }
        }

        #region alternatives working for slider behavior
        //public double FromValue
        //{
        //    get { return _fromValue; }
        //    set
        //    {
        //        SetProperty(ref _fromValue, value);
        //        if (_fromValue > _toValue)
        //        {
        //            ToValue = _fromValue;
        //        }
        //    }
        //}

        //public double ToValue
        //{
        //    get { return _toValue; }
        //    set
        //    {
        //        SetProperty(ref _toValue, value);
        //        if (_toValue < _fromValue)
        //        {
        //            FromValue = _toValue;
        //        }
        //    }
        //}
        #endregion

        private string _selectedPixelPercentage;
        /// <summary>
        /// The percentage of pixels in the image that are within the selected range.
        /// </summary>
        /// <remarks>
        /// Displayed in the user interface.
        /// </remarks>
        public string SelectedPixelPercentage
        {
            get { return _selectedPixelPercentage; }
            set { SetProperty(ref _selectedPixelPercentage, value); }
        }

        private ThresholdingType _thresholding;
        /// <summary>
        /// The selected thresholding method.
        /// </summary>
        public ThresholdingType Thresholding
        {
            get { return _thresholding; }
            set
            {
                SetProperty(ref _thresholding, value);
                UpdateImage();
            }
        }

        private Mat _origin;
        /// <summary>
        /// Original image
        /// </summary>
        public Mat Origin
        {
            get { return _origin; }
            set { SetProperty(ref _origin, value); }
        }

        private BitmapSource _mySource;
        public BitmapSource MySource
        {
            get { return _mySource; }
            set { SetProperty(ref _mySource, value); }
        }

        private DialogResult dialogResult = new DialogResult(ButtonResult.Cancel);

        public DelegateCommand BtnConfirm_ClickCommand { get; }
        public DelegateCommand BtnCancel_ClickCommand { get; }

        // parameterless constructor is necessary for ObjectDataProvider
        public ThresholderWindowViewModel()
        {
            BtnConfirm_ClickCommand = new DelegateCommand(BtnConfirm_Click);
            BtnCancel_ClickCommand = new DelegateCommand(BtnCancel_Click);
        }

        public ThresholderWindowViewModel(Mat image) : this()
        {
            Origin = image;

            FromValue = 0;
            ToValue = 255;
            Thresholding = ThresholdingType.Standard;
            UpdatePercentage();

            Mat sizer = new Mat(new Size(256, 256), image.Depth, image.NumberOfChannels);

            // Set the background to black
            sizer.SetTo(new MCvScalar(0, 0, 0));

            (int[] histogramData, _) = ImageProcessingCore.CalculateHistogramValues(image);
            DrawHistogram(sizer, histogramData);
            MySource = sizer.MatToBitmapSource();
        }

        private void DrawHistogram(Mat image, int[] histogramData)
        {
            // Calculate the maximum value in the histogram
            int maxCount = histogramData.Max();

            // Define the histogram dimensions
            int histogramWidth = 256; // Fixed width
            int histogramHeight = 256; // Fixed height
            int bottomOffset = 159; // Adjust this value to set the bottom offset

            // Draw the histogram bars directly on the image
            for (int i = 0; i < histogramData.Length; i++)
            {
                // Calculate the height of the bar
                int barHeight = (int)Math.Round((double)histogramData[i] / maxCount * (histogramHeight - bottomOffset));

                // Ensure that at least one pixel is drawn for very small counts
                barHeight = Math.Max(barHeight, 1);

                // Calculate the top-left corner of the bar
                int barX = i * (histogramWidth / histogramData.Length);
                int barY = histogramHeight - barHeight - bottomOffset;

                // Create a rectangle to represent the bar
                System.Drawing.Rectangle rect = new System.Drawing.Rectangle(barX, barY, histogramWidth / histogramData.Length, barHeight);

                // Draw filled rectangle
                CvInvoke.Rectangle(image, rect, new MCvScalar(255, 0, 0), 1);
            }
        }

        private void BtnConfirm_Click()
        {
            dialogResult = new DialogResult(ButtonResult.OK);
            (App.Current.Windows.OfType<ThresholderWindow>().FirstOrDefault(x => x.DataContext == this)).Close();
        }

        private void BtnCancel_Click()
        {
            (App.Current.Windows.OfType<ThresholderWindow>().FirstOrDefault(x => x.DataContext == this)).Close();
        }

        internal void OnClosing()
        {
            if (dialogResult.Result == ButtonResult.Cancel)
            {
                MainWindowViewModel.FocusedImage.Restore(Origin);
            }
        }

        /// <summary>
        /// Keeps upper slider's value below lower slider's value and vice-versa
        /// </summary>
        private void AdjustSliderValues()
        {
            if (FromValue > ToValue)
            {
                var temp = FromValue;
                FromValue = ToValue;
                ToValue = temp;
            }
            else if (ToValue < FromValue)
            {
                var temp = ToValue;
                ToValue = FromValue;
                FromValue = temp;
            }
        }

        private void UpdatePercentage()
        {
            Mat image = Origin;

            // Calculate the percentage of selected pixels
            double percentage = Math.Round(ImageProcessingUtility.GetSelectedPixelPercentage(image, FromValue, ToValue), 2);

            // Update the SelectedPixelPercentage property
            SelectedPixelPercentage = percentage.ToString();
        }

        private void UpdateImage()
        {
            MainWindowViewModel.FocusedImage.PerformThresholding(Origin, FromValue, ToValue, Thresholding);
        }

        /// <summary>
        /// Retrieves the available thresholding types.
        /// </summary>
        /// <returns>An IEnumerable of strings representing the available thresholding types.</returns>
        /// <remarks>
        /// This method is primarily used in XAML to populate UI elements with available thresholding types.
        /// </remarks>
        public IEnumerable<string> GetThresholdingTypes()
        {
            ThresholdingType[] thresholdingTypes = (ThresholdingType[])Enum.GetValues(typeof(ThresholdingType));
            return ThresholdingTypeHelper.GetLocalizedShapeTypes(thresholdingTypes);
        }
    }
}
