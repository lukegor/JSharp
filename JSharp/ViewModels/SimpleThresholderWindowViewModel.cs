using Emgu.CV.Structure;
using Emgu.CV;
using JSharp.Utility;
using Prism.Commands;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Media.Imaging;
using Prism.Mvvm;

namespace JSharp.ViewModels
{
    public class SimpleThresholderWindowViewModel : BindableBase
    {
        private int _fromValue;
        public int FromValue
        {
            get { return _fromValue; }
            set
            {
                SetProperty(ref _fromValue, value);
                UpdatePercentage();
                UpdateImage();
            }
        }

        private const int MaxThreshold = 255;

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
        public string SelectedPixelPercentage
        {
            get { return _selectedPixelPercentage; }
            set { SetProperty(ref _selectedPixelPercentage, value); }
        }

        private SimpleThresholdingMethod _thresholding;
        public SimpleThresholdingMethod Thresholding
        {
            get { return _thresholding; }
            set
            {
                SetProperty(ref _thresholding, value);
                UpdateImage();
            }
        }

        private Mat _origin;
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
        public SimpleThresholderWindowViewModel()
        {
            BtnConfirm_ClickCommand = new DelegateCommand(BtnConfirm_Click);
            BtnCancel_ClickCommand = new DelegateCommand(BtnCancel_Click);
        }

        public SimpleThresholderWindowViewModel(Mat image) : this()
        {
            Origin = image;

            FromValue = 0;
            Thresholding = SimpleThresholdingMethod.Standard;
            UpdatePercentage();

            Mat sizer = new Mat(new System.Drawing.Size(256, 1000), image.Depth, image.NumberOfChannels);
            (int[] histogramData, int pixelCount) = ImageProcessingCore.CalculateHistogramValues(image);
            DrawHistogram(sizer, histogramData);
            MySource = sizer.MatToBitmapSource();
        }

        private void DrawHistogram(Mat image, int[] histogramData)
        {
            // Calculate the maximum value in the histogram
            int maxCount = histogramData.Max();

            // Define the histogram dimensions
            const int histogramWidth = 256; // Fixed width
            const int histogramHeight = 1000; // Fixed height
            const int bottomOffset = 905; // Adjust this value to set the bottom offset

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
            (App.Current.Windows.OfType<SimpleThresholderWindow>().FirstOrDefault(x => x.DataContext == this)).Close();
        }

        private void BtnCancel_Click()
        {
            (App.Current.Windows.OfType<SimpleThresholderWindow>().FirstOrDefault(x => x.DataContext == this)).Close();
        }

        internal void OnClosing()
        {
            if (dialogResult.Result == ButtonResult.Cancel)
            {
                MainWindowViewModel.FocusedImage.Restore(Origin);
            }
        }

        private void UpdatePercentage()
        {
            Mat image = Origin;

            // Calculate the percentage of selected pixels
            double percentage = Math.Round(ImageProcessingUtility.GetSelectedPixelPercentage(image, FromValue, MaxThreshold), 2);

            // Update the SelectedPixelPercentage property
            SelectedPixelPercentage = percentage.ToString();
        }

        private void UpdateImage()
        {
            MainWindowViewModel.FocusedImage.PerformSimpleThresholding(Origin, FromValue, Thresholding);
        }

        public IEnumerable<string> GetThresholdingTypes()
        {
            SimpleThresholdingMethod[] thresholdingTypes = (SimpleThresholdingMethod[])Enum.GetValues(typeof(SimpleThresholdingMethod));
            return SimpleThresholdingTypeHelper.GetLocalizedShapeTypes(thresholdingTypes);
        }
    }
}
