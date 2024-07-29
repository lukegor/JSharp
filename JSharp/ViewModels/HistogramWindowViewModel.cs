using Emgu.CV;
using LiveChartsCore.SkiaSharpView;
using LiveChartsCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Prism.Mvvm;
using Emgu.CV.ML;
using JSharp.Models.BusinessLogicModels;

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
