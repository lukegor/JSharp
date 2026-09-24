using CommunityToolkit.Mvvm.ComponentModel;
using JSharp.Domain.Measurements;
using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using OpenCvSharp;
using System.Collections.ObjectModel;

namespace JSharp.Models
{
    public class Histogram : ObservableObject
    {
        public ISeries[] HistogramSeries
        {
            get => field;
            set => SetProperty(ref field, value);
        } = null!;

        public int PixelSum
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        public ObservableCollection<HistogramBin> HistogramData
        {
            get => field;
            set => SetProperty(ref field, value);
        } = new();

        public HistogramMeasurement? LastMeasurement { get; private set; }

        public void UpdateHistogram(Mat image)
        {
            HistogramMeasurement measurement = MeasurementCompute.ComputeHistogram(image);
            LastMeasurement = measurement;
            PixelSum = measurement.PixelCount;

            var dataSeries = new ColumnSeries<int>
            {
                Values = measurement.Bins,
                YToolTipLabelFormatter = chartPoint => $"{chartPoint.Coordinate}"
            };
            HistogramSeries = new ISeries[] { dataSeries };

            HistogramData = new ObservableCollection<HistogramBin>(measurement.Rows());
        }
    }
}
