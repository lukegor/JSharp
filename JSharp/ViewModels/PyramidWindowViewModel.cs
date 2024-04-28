using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.ViewModels
{
    public class PyramidWindowViewModel : BindableBase
    {
        private int _effectSize;
        public int EffectSize
        {
            get { return _effectSize; }
            set { SetProperty(ref _effectSize, value); }
        }

        public DelegateCommand BtnConfirm_ClickCommand { get; }

        public PyramidWindowViewModel()
        {
            BtnConfirm_ClickCommand = new DelegateCommand(BtnConfirm_Click);
            EffectSize = 2;
        }

        private void BtnConfirm_Click()
        {
            var window = App.Current.Windows.OfType<PyramidWindow>().FirstOrDefault(x => x.DataContext == this);
            window.DialogResult = true;
        }
    }
}
