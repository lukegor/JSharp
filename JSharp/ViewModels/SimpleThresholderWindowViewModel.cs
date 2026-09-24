using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Domain.Models.SimpleDataModels;
using JSharp.Domain.Operations;
using JSharp.Utility.Utility;
using JSharp.ViewModels.Abstractions;
using OpenCvSharp;
using System.Windows.Media.Imaging;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="SimpleThresholderWindow"/>. Live preview: every change
    /// invokes the preview callback; confirm completes <see cref="SimpleThresholdParams"/>.
    /// </summary>
    internal partial class SimpleThresholderWindowViewModel : DialogViewModel<SimpleThresholdParams>
    {
        private const int MaxThreshold = 255;

        private readonly Mat _origin;
        private readonly Action _preview;
        private readonly bool _previewArmed;

        [ObservableProperty]
        public partial int FromValue { get; set; }

        public string SelectedPixelPercentage
        {
            get => field;
            set => SetProperty(ref field, value);
        } = string.Empty;

        [ObservableProperty]
        public partial SimpleThresholdingMethod Thresholding { get; set; }

        public Mat Origin => _origin;

        public BitmapSource? MySource
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        public IReadOnlyList<string> ThresholdingTypes { get; }

        public SimpleThresholderWindowViewModel(Mat origin, Action preview)
        {
            _origin = origin;
            _preview = preview;

            ThresholdingTypes = SimpleThresholdingTypeHelper.GetLocalizedShapeTypes(
                (SimpleThresholdingMethod[])Enum.GetValues(typeof(SimpleThresholdingMethod))).ToList();

            FromValue = 0;
            Thresholding = SimpleThresholdingMethod.Standard;
            UpdatePercentage();
            MySource = HistogramThumbnail.CreateFor(origin);
            _previewArmed = true;
        }

        partial void OnFromValueChanged(int value)
        {
            UpdatePercentage();
            PreviewIfArmed();
        }

        partial void OnThresholdingChanged(SimpleThresholdingMethod value) => PreviewIfArmed();

        /// <summary>
        /// Slider defaults assigned in the constructor must not preview: the dialog is not
        /// shown yet, and the router closure over this VM is not assigned until construction returns.
        /// </summary>
        private void PreviewIfArmed()
        {
            if (_previewArmed)
            {
                _preview();
            }
        }

        [RelayCommand]
        private void Confirm() => Complete(new SimpleThresholdParams(FromValue, Thresholding));

        [RelayCommand]
        private void Cancel() => Close();

        private void UpdatePercentage()
        {
            double percentage = Math.Round(
                ImageProcessingUtility.GetSelectedPixelPercentage(_origin, FromValue, MaxThreshold), 2);
            SelectedPixelPercentage = percentage.ToString();
        }
    }
}
