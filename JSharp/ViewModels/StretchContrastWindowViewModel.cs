using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using System.ComponentModel;

namespace JSharp.ViewModels
{
    internal class StretchContrastWindowViewModel : ObservableObject
    {
        public event EventHandler ValuesSelected;

        #region dual fields/properties
        [Description("Desired value lower bound")]
        private int _q3;
        public int Q3
        {
            get { return _q3; }
            set
            {
                SetProperty(ref _q3, value);
            }
        }
        [Description("Desired value upper bound")]
        private int _q4;
        public int Q4
        {
            get { return _q4; }
            set
            {
                SetProperty(ref _q4, value);
            }
        }

        private int _p1;
        public int P1
        {
            get { return _p1; }
            set
            {
                SetProperty(ref _p1, value);
            }
        }
        private int _p2;
        public int P2
        {
            get { return _p2; }
            set
            {
                SetProperty(ref _p2, value);
            }
        }
        #endregion

        public RelayCommand BtnConfirm_ClickCommand { get; }

        public StretchContrastWindowViewModel()
        {
            P2 = 255;
            Q4 = 255;
        }

        public void BtnConfirmLogic_Click(int q3, int q4)
        {
            this.Q3 = q3;
            this.Q4 = q4;
            ValuesSelected?.Invoke(this, EventArgs.Empty);
        }
    }
}
