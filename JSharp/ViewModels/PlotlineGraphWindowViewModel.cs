using CommunityToolkit.Mvvm.ComponentModel;
using JSharp.Domain.Measurements;
using JSharp.UI.Views;
using JSharp.ViewModels.Abstractions;
using LiveChartsCore;
using LiveChartsCore.Defaults;
using LiveChartsCore.SkiaSharpView;
using OpenCvSharp;
using System.Collections.ObjectModel;
using System.Windows;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="PlotlineGraphWindow"/>
    /// </summary>
    internal class PlotlineGraphWindowViewModel : ObservableObject
    {
        public ISeries[] Values
        {
            get => field;
            set => SetProperty(ref field, value);
        } = null!;

        public ProfileMeasurement Measurement { get; }

        public PlotlineGraphWindowViewModel(System.Windows.Point[] points, Mat image)
        {
            Measurement = MeasurementCompute.SampleProfileLine(
                image, (int)points[0].X, (int)points[0].Y, (int)points[1].X, (int)points[1].Y);
            Values = PlotGraph(Measurement);
        }

        public string ExportCsv() => MeasurementCsv.Profile(Measurement);

        public string ExportJson() =>
            MeasurementJson.Envelope(new MeasurementSource(null, null), null, null, new[] { Measurement });

        public byte[] ExportPngBytes()
        {
            using Mat chart = MeasurementChart.RenderProfile(Measurement);
            return MeasurementChart.EncodePng(chart);
        }

        private ISeries[] PlotGraph(ProfileMeasurement measurement)
        {
            var linePoints = measurement.Values
                .Select((value, index) => new ObservablePoint(index + 1, value))
                .ToList();
            var lineSeries = new LineSeries<ObservablePoint>
            {
                Values = new ObservableCollection<ObservablePoint>(linePoints),
                YToolTipLabelFormatter = chartPoint => $"{chartPoint.Coordinate}"
            };
            return new ISeries[] { lineSeries };
        }
    }
}
