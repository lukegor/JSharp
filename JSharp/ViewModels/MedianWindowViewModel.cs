using Emgu.CV.CvEnum;
using JSharp.UI.Views;
using JSharp.Utility;
using JSharp.Views;
using Prism.Commands;
using Prism.Mvvm;

namespace JSharp.ViewModels
{
    internal class MedianWindowViewModel : BindableBase
    {
        public IList<string> BorderTypes { get; } = GetEdgePixelsHandlingOptions().ToList();

		private int _matrixSize = 3;
        public int MatrixSize
        {
            get => _matrixSize;
            set
            {
                SetProperty(ref _matrixSize, value);
            }
        }

        private BorderType _borderPixelsOption;
        public BorderType BorderPixelsOption
        {
            get { return _borderPixelsOption; }
            set { SetProperty(ref _borderPixelsOption, value); }
        }

        public DelegateCommand BtnApply_ClickCommand { get; }

        public MedianWindowViewModel()
        {
            BtnApply_ClickCommand = new DelegateCommand(BtnApply_Click);

            this.BorderPixelsOption = BorderType.Isolated;
        }

        private void BtnApply_Click()
        {
            var associatedWindow = App.Current.Windows.OfType<MedianWindow>().First(window => window.DataContext == this);
            associatedWindow.DialogResult = true;
        }

        private static IEnumerable<string> GetEdgePixelsHandlingOptions()
        {
	        IEnumerable<BorderType> borderTypes = [BorderType.Isolated, BorderType.Reflect, BorderType.Replicate];
	        return BorderTypeHelper.GetLocalizedEdgePixelsHandlingOptions(borderTypes);
        }
	}
}
