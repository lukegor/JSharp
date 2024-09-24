using System.Collections.ObjectModel;
using Emgu.CV;
using JSharp.Models.Services;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using Prism.Mvvm;

namespace JSharp.Models.DataModels
{
    internal class Histogram : BindableBase
    {
        private ISeries[] _histogramSeries;
        public ISeries[] HistogramSeries
        {
            get { return _histogramSeries; }
            set { SetProperty(ref _histogramSeries, value); }
        }

        private int _pixelSum;
        public int PixelSum
        {
            get { return _pixelSum; }
            set { SetProperty(ref _pixelSum, value); }
        }

        private ObservableCollection<object> _histogramData;
        public ObservableCollection<object> HistogramData
        {
            get { return _histogramData; }
            set { SetProperty(ref _histogramData, value); }
        }

        public void UpdateHistogram(Mat image)
        {
            (int[] histogramData, int sum) = ImageProcessingCore.CalculateHistogramValues(image);

            PixelSum = sum;

            var dataSeries = new ColumnSeries<int>
            {
                Values = histogramData,
                YToolTipLabelFormatter = chartPoint => $"{chartPoint.Coordinate}"
            };
            HistogramSeries = new ISeries[] { dataSeries };

            List<object> tableData = AggregateTableHistogramData(histogramData);
            HistogramData = new ObservableCollection<object>(tableData);
        }

        public static List<object> AggregateTableHistogramData(int[] histogramData)
        {
            List<object> tableData = new List<object>();
            for (int i = 0; i < 256; i++)
            {
                tableData.Add(new { LightnessLevel = i, PixelCount = histogramData[i] });
            }

            return tableData;
        }
    }
}
