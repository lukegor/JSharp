using CommunityToolkit.Mvvm.ComponentModel;
using Emgu.CV;
using JSharp.Domain.Models.DataModels;

namespace JSharp.ViewModels
{
    internal class HistogramWindowViewModel : ObservableObject
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
