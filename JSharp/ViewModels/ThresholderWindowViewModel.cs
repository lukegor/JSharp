using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Domain.Operations;
using JSharp.Utility.Utility;
using JSharp.ViewModels.Abstractions;
using OpenCvSharp;
using System.Windows.Media.Imaging;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="ThresholderWindow"/>. Dual-threshold live preview;
    /// confirm completes <see cref="DualThresholdParams"/>.
    /// </summary>
    internal partial class ThresholderWindowViewModel : DialogViewModel<DualThresholdParams>
    {
        private readonly Mat _origin;
        private readonly Action _preview;
        private readonly bool _previewArmed;

        [ObservableProperty]
        public partial int FromValue { get; set; }

        [ObservableProperty]
        public partial int ToValue { get; set; }

        public string SelectedPixelPercentage
        {
            get => field;
            set => SetProperty(ref field, value);
        } = string.Empty;

        [ObservableProperty]
        public partial ThresholdingType Thresholding { get; set; }

        [ObservableProperty]
        public partial bool EnableContrastMode { get; set; }

        public Mat Origin => _origin;

        public BitmapSource? MySource
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        public IReadOnlyList<string> ThresholdingTypes { get; }

        public ThresholderWindowViewModel(Mat origin, Action preview)
        {
            _origin = origin;
            _preview = preview;

            ThresholdingTypes = ThresholdingTypeHelper.GetLocalizedShapeTypes(
                (ThresholdingType[])Enum.GetValues(typeof(ThresholdingType))).ToList();

            FromValue = 0;
            ToValue = 255;
            Thresholding = ThresholdingType.Standard;
            EnableContrastMode = false;
            UpdatePercentage();
            MySource = HistogramThumbnail.CreateFor(origin);
            _previewArmed = true;
        }

        partial void OnFromValueChanged(int value)
        {
            AdjustSliderValues();
            UpdatePercentage();
            PreviewIfArmed();
        }

        partial void OnToValueChanged(int value)
        {
            AdjustSliderValues();
            UpdatePercentage();
            PreviewIfArmed();
        }

        partial void OnThresholdingChanged(ThresholdingType value) => PreviewIfArmed();

        partial void OnEnableContrastModeChanged(bool value) => PreviewIfArmed();

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
        private void Confirm() => Complete(new DualThresholdParams(FromValue, ToValue, Thresholding, EnableContrastMode));

        [RelayCommand]
        private void Cancel() => Close();

        /// <summary>Keeps the lower value below the upper value (existing UX behavior).</summary>
        private void AdjustSliderValues()
        {
            if (FromValue > ToValue)
            {
                (FromValue, ToValue) = (ToValue, FromValue);
            }
        }

        private void UpdatePercentage()
        {
            double percentage = Math.Round(
                ImageProcessingUtility.GetSelectedPixelPercentage(_origin, FromValue, ToValue), 2);
            SelectedPixelPercentage = percentage.ToString();
        }
    }
}
