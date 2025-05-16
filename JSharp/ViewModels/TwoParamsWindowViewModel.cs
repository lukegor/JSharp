using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Models.SimpleDataModels;
using JSharp.UI.Views;
using JSharp.Views;

namespace JSharp.ViewModels
{
    public class TwoParamsWindowViewModel : ObservableObject
    {
        public ObservableCollection<SliderProperties> SliderPropertiesCollection { get; set; } = new ObservableCollection<SliderProperties>();
        private string _txbText;
        public string TxbText
        {
            get { return _txbText; }
            set { SetProperty(ref _txbText, value); }
        }

        private int _min;
        public int Min
        {
            get { return _min; }
            set { SetProperty(ref _min, value); }
        }
        private int _max;
        public int Max
        {
            get { return _max; }
            set { SetProperty(ref _max, value); }
        }

        public RelayCommand BtnConfirm_ClickCommand { get; }

        public TwoParamsWindowViewModel(TwoParamsVMInfo info)
        {
            BtnConfirm_ClickCommand = new RelayCommand(BtnConfirm_Click);

            SliderPropertiesCollection.Add(info.Slider1Properties);
            SliderPropertiesCollection.Add(info.Slider2Properties);
            Min = info.Slider1Properties.DefaultValue;
            Max = info.Slider2Properties.DefaultValue;
        }

        private void BtnConfirm_Click()
        {
            var window = App.Current.Windows.OfType<TwoParamsWindow>().FirstOrDefault(x => x.DataContext == this);
            window.DialogResult = true;
        }
    }
}
