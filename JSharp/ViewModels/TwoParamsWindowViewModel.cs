using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.ViewModels
{
    public class TwoParamsWindowViewModel : BindableBase
    {
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

        public DelegateCommand BtnConfirm_ClickCommand { get; }

        public TwoParamsWindowViewModel()
        {
            BtnConfirm_ClickCommand = new DelegateCommand(BtnConfirm_Click);

            Min = 100;
            Max = 200;
        }

        private void BtnConfirm_Click()
        {
            var window = App.Current.Windows.OfType<TwoParamsWindow>().FirstOrDefault(x => x.DataContext == this);
            window.DialogResult = true;
        }
    }
}
