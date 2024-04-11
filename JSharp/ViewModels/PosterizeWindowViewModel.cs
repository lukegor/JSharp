using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.ViewModels
{
    internal class PosterizeWindowViewModel : BindableBase
    {
        public event EventHandler ValueSelected;

        private int _levelsNumber = 2;
        public int LevelsNumber
        {
            get { return _levelsNumber; }
            set { SetProperty(ref _levelsNumber, value); }
        }

        public DelegateCommand BtnConfirm_ClickCommand { get; }

        public PosterizeWindowViewModel()
        {
            BtnConfirm_ClickCommand = new DelegateCommand(BtnConfirmLogic_Click);
        }

        public void BtnConfirmLogic_Click()
        {
            ValueSelected?.Invoke(this, EventArgs.Empty);
            (App.Current.Windows.OfType<PosterizeWindow>().FirstOrDefault(w => w.DataContext == this))?.Close();
        }
    }
}
