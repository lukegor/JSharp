﻿using System.Windows.Media.Imaging;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Emgu.CV;
using Emgu.CV.Structure;
using JSharp.Domain.Services;
using JSharp.UI.Views;
using JSharp.Utility;
using JSharp.Utility.Utility;
using JSharp.Views;

namespace JSharp.ViewModels
{
    public class SimpleThresholderWindowViewModel : ObservableObject
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

        // window containing image to be modified
        private readonly NewImageWindowViewModel _windowToBeModified;

        public RelayCommand BtnConfirm_ClickCommand { get; }
        public RelayCommand BtnCancel_ClickCommand { get; }

        // parameterless constructor is necessary for ObjectDataProvider
        public SimpleThresholderWindowViewModel()
        {
            BtnConfirm_ClickCommand = new RelayCommand(BtnConfirm_Click);
            BtnCancel_ClickCommand = new RelayCommand(BtnCancel_Click);
        }

        public SimpleThresholderWindowViewModel(Mat image, NewImageWindowViewModel windowToBeModified) : this()
        {
            Origin = image;
            _windowToBeModified = windowToBeModified;

            FromValue = 0;
            Thresholding = SimpleThresholdingMethod.Standard;
            UpdatePercentage();

            Mat sizer = new Mat(new System.Drawing.Size(256, 256), image.Depth, image.NumberOfChannels);
            // Set the background to black
            sizer.SetTo(new MCvScalar(0, 0, 0));

            (int[] histogramData, _) = ImageProcessingCore.CalculateHistogramValues(image);
            DrawHistogram(sizer, histogramData);
            MySource = sizer.MatToBitmapSource();
        }

        /// <summary>
        /// Draws a small histogram of input image.
        /// </summary>
        /// <param name="image"></param>
        /// <param name="histogramData"></param>
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
            var window = App.Current.Windows.OfType<SimpleThresholderWindow>().FirstOrDefault(x => x.DataContext == this);
            window.DialogResult = true;
            window.Close();
        }

        private void BtnCancel_Click()
        {
            (App.Current.Windows.OfType<SimpleThresholderWindow>().FirstOrDefault(x => x.DataContext == this)).Close();
        }

        internal void OnClosing(bool? dialogResult)
        {
            if (dialogResult == false)
            {
                _windowToBeModified.Restore(Origin);
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
            _windowToBeModified.PerformSimpleThresholding(Origin, FromValue, Thresholding);
        }

        public IEnumerable<string> GetThresholdingTypes()
        {
            SimpleThresholdingMethod[] thresholdingTypes = (SimpleThresholdingMethod[])Enum.GetValues(typeof(SimpleThresholdingMethod));
            return SimpleThresholdingTypeHelper.GetLocalizedShapeTypes(thresholdingTypes);
        }
    }
}
