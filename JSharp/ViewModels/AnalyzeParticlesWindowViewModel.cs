using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Domain.Models.SimpleDataModels;
using JSharp.UI.Views;
using JSharp.Views;

namespace JSharp.ViewModels
{
	public class AnalyzeParticlesWindowViewModel : ObservableObject
    {
        private string _sizeText;
        public string SizeText
        {
            get { return _sizeText; }
            set { SetProperty(ref _sizeText, value); }
        }
        public AnalysisSettings AnalysisSettings { get; set; }

        public RelayCommand BtnConfirm_ClickCommand { get; }
        public AnalyzeParticlesWindowViewModel()
        {
            BtnConfirm_ClickCommand = new RelayCommand(BtnConfirm_Click);
            SizeText = "0-inf";
        }

        private void BtnConfirm_Click()
        {
            string[] parts = SizeText.Split('-');
            int? max = int.TryParse(parts[1]?.ToString(), out int result) ? result : null;
            AnalysisSettings = new AnalysisSettings(int.Parse(parts[0]), max);

            var window = App.Current.Windows.OfType<AnalyzeParticlesWindow>().FirstOrDefault(x => x.DataContext == this);
            window.DialogResult = true;
        }
    }
}
