using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Domain.Models.SimpleDataModels;
using JSharp.ViewModels.Abstractions;
using System.Collections.ObjectModel;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="TwoParamsWindow"/>. Completes (Min, Max) on confirm.
    /// </summary>
    internal partial class TwoParamsWindowViewModel : DialogViewModel<(int Min, int Max)>
    {
        public ObservableCollection<SliderProperties> SliderPropertiesCollection { get; set; } = new();

        public string TxbText
        {
            get => field;
            set => SetProperty(ref field, value);
        } = string.Empty;

        [ObservableProperty]
        public partial int Min { get; set; }

        [ObservableProperty]
        public partial int Max { get; set; }

        public TwoParamsWindowViewModel(TwoParamsVMInfo info)
        {
            SliderPropertiesCollection.Add(info.Slider1Properties);
            SliderPropertiesCollection.Add(info.Slider2Properties);
            Min = info.Slider1Properties.DefaultValue;
            Max = info.Slider2Properties.DefaultValue;
        }

        [RelayCommand]
        private void Confirm() => Complete((Min, Max));
    }
}
