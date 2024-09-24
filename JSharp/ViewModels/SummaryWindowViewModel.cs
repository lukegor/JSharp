using System.Collections.ObjectModel;
using JSharp.Models.Services;
using JSharp.Models.SimpleDataModels;
using Prism.Mvvm;

namespace JSharp.ViewModels
{
    public class SummaryWindowViewModel : BindableBase
    {
        private ObservableCollection<object> _data;
        public ObservableCollection<object> Data
        {
            get { return _data; }
            set { SetProperty(ref _data, value); }
        }

        private AnalysisSettings AnalysisSettings { get; set; }

        public SummaryWindowViewModel()
        {
            int count = ImageProcessingCore.CountObjectsInImage(MainWindowViewModel.FocusedImage.MatImage);
            List<object> list = new List<object>();
            list.Add(new { Image = MainWindowViewModel.FocusedImage.FileName, Count = count });
            Data = new ObservableCollection<object>(list);
        }

        public SummaryWindowViewModel(AnalysisSettings analysisSettings)
        {
            this.AnalysisSettings = analysisSettings;
            int? min = analysisSettings.SizeFrom;
            int? max = int.TryParse(analysisSettings.SizeTo?.ToString(), out int result) ? result : null;
            int count = ImageProcessingCore.CountObjectsInImage(MainWindowViewModel.FocusedImage.MatImage, min, max);
            List<object> list = new List<object>();
            list.Add(new { Image = MainWindowViewModel.FocusedImage.FileName, Count = count });
            Data = new ObservableCollection<object>(list);
        }
    }
}
