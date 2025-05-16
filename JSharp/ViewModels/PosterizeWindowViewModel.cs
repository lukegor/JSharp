using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.UI.Views;
using JSharp.Views;

namespace JSharp.ViewModels
{
    internal class PosterizeWindowViewModel : ObservableObject
    {
        public event EventHandler ValueSelected;

        private int _levelsNumber = 2;
        public int LevelsNumber
        {
            get { return _levelsNumber; }
            set { SetProperty(ref _levelsNumber, value); }
        }

        public RelayCommand BtnConfirm_ClickCommand { get; }

        public PosterizeWindowViewModel()
        {
            BtnConfirm_ClickCommand = new RelayCommand(BtnConfirmLogic_Click);
        }

        public void BtnConfirmLogic_Click()
        {
            ValueSelected?.Invoke(this, EventArgs.Empty);
            (App.Current.Windows.OfType<PosterizeWindow>().FirstOrDefault(w => w.DataContext == this))?.Close();
        }
    }
}
