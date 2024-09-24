using Emgu.CV;
using JSharp.Models.DataModels;
using Prism.Mvvm;

namespace JSharp.ViewModels
{
    internal class HistogramWindowViewModel : BindableBase
    {
        public Histogram Histogram { get; private set; }

        public HistogramWindowViewModel()
        {
            Histogram = new Histogram();
        }

        public void UpdateHistogram(Mat image)
        {
            Histogram.UpdateHistogram(image);
        }
    }
}
