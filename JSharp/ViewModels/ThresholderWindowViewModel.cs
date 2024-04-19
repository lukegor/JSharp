using Emgu.CV;
using JSharp.Utility;
using Prism.Commands;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace JSharp.ViewModels
{
    public class ThresholderWindowViewModel : BindableBase
    {
        private int _fromValue;
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
        public string SelectedPixelPercentage
        {
            get { return _selectedPixelPercentage; }
            set { SetProperty(ref _selectedPixelPercentage, value); }
        }

        private ThresholdingType _thresholding;
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
        public Mat Origin
        {
            get { return _origin; }
            set { SetProperty(ref _origin, value); }
        }

        private DialogResult dialogResult = new DialogResult(ButtonResult.Cancel);

        public DelegateCommand BtnConfirm_ClickCommand { get; }
        public DelegateCommand BtnCancel_ClickCommand { get; }

        public ThresholderWindowViewModel()
        {
            BtnConfirm_ClickCommand = new DelegateCommand(BtnConfirm_Click);
            BtnCancel_ClickCommand = new DelegateCommand(BtnCancel_Click);
            Origin = MainWindowViewModel.FocusedImage.MatImage.Clone();

            FromValue = 0;
            ToValue = 255;
            Thresholding = ThresholdingType.Standard;
            UpdatePercentage();
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

        public IEnumerable<string> GetThresholdingTypes()
        {
            ThresholdingType[] thresholdingTypes = (ThresholdingType[])Enum.GetValues(typeof(ThresholdingType));
            return ThresholdingTypeHelper.GetLocalizedShapeTypes(thresholdingTypes);
        }
    }
}
