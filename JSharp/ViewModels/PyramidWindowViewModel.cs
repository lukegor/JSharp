using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.UI.Views;
using JSharp.Views;

namespace JSharp.ViewModels
{
    public class PyramidWindowViewModel : ObservableObject
    {
        private int _effectSize;
        public int EffectSize
        {
            get { return _effectSize; }
            set { SetProperty(ref _effectSize, value); }
        }

        public RelayCommand BtnConfirm_ClickCommand { get; }

        public PyramidWindowViewModel()
        {
            BtnConfirm_ClickCommand = new RelayCommand(BtnConfirm_Click);
            EffectSize = 2;
        }

        private void BtnConfirm_Click()
        {
            var window = App.Current.Windows.OfType<PyramidWindow>().FirstOrDefault(x => x.DataContext == this);
            window.DialogResult = true;
        }
    }
}
