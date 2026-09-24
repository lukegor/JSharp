using CommunityToolkit.Mvvm.ComponentModel;
using JSharp.Domain.Measurements;
using JSharp.Models;
using JSharp.UI.Views;
using JSharp.ViewModels.Abstractions;
using OpenCvSharp;
using System;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="HistogramWindow"/>
    /// </summary>
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

        public bool HasData => Histogram.LastMeasurement is not null;

        public string ExportCsv() =>
            Histogram.LastMeasurement is null ? string.Empty : MeasurementCsv.Histogram(Histogram.LastMeasurement);

        public string ExportJson() =>
            Histogram.LastMeasurement is null
                ? string.Empty
                : MeasurementJson.Envelope(new MeasurementSource(null, null), Histogram.LastMeasurement, null, null);

        public byte[] ExportPngBytes()
        {
            if (Histogram.LastMeasurement is null)
            {
                return Array.Empty<byte>();
            }

            using Mat chart = MeasurementChart.RenderHistogram(Histogram.LastMeasurement);
            return MeasurementChart.EncodePng(chart);
        }
    }
}
