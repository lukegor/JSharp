using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

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

        public SummaryWindowViewModel()
        {
            int count = ImageProcessingCore.CountObjectsInImage(MainWindowViewModel.FocusedImage.MatImage);
            List<object> list = new List<object>();
            list.Add(new { Image = MainWindowViewModel.FocusedImage.FileName, Count = count });
            Data = new ObservableCollection<object>(list);
        }
    }
}
