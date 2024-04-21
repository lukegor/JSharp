using JSharp.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.ViewModels
{
    public class AnalyzeParticlesWindowViewModel : BindableBase
    {
        private string _sizeText;
        public string SizeText
        {
            get { return _sizeText; }
            set { SetProperty(ref _sizeText, value); }
        }
        public AnalysisSettings AnalysisSettings { get; set; }

        public DelegateCommand BtnConfirm_ClickCommand { get; }
        public AnalyzeParticlesWindowViewModel()
        {
            BtnConfirm_ClickCommand = new DelegateCommand(BtnConfirm_Click);
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
