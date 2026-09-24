using CommunityToolkit.Mvvm.ComponentModel;
using JSharp.Domain.Measurements;
using JSharp.Domain.Models.SimpleDataModels;
using OpenCvSharp;
using System.Collections.ObjectModel;

namespace JSharp.ViewModels
{
    internal sealed record ParticleSummaryRow(string? Image, int Count);

    /// <summary>
    /// Viewmodel for <see cref="SummaryWindow"/>. Pure: the focused image is passed in.
    /// </summary>
    internal class SummaryWindowViewModel : ObservableObject
    {
        public ObservableCollection<ParticleSummaryRow> Data
        {
            get => field;
            set => SetProperty(ref field, value);
        } = new ObservableCollection<ParticleSummaryRow>();

        public ParticleMeasurement? Measurement { get; }

        public string? FileName { get; }

        public SummaryWindowViewModel(NewImageWindowViewModel? focusedImage)
            : this(focusedImage?.FileName, focusedImage?.MatImage, null, null)
        {
        }

        public SummaryWindowViewModel(AnalysisSettings settings, NewImageWindowViewModel? focusedImage)
            : this(focusedImage?.FileName, focusedImage?.MatImage, settings.SizeFrom, settings.SizeTo)
        {
        }

        private SummaryWindowViewModel(string? fileName, Mat? image, int? min, int? max)
        {
            FileName = fileName;
            if (fileName is null || image is null)
            {
                return;
            }

            Measurement = min.HasValue
                ? MeasurementCompute.ComputeParticles(image, min, max)
                : MeasurementCompute.ComputeParticles(image, null, null);
            Data = new ObservableCollection<ParticleSummaryRow>(
                new[] { new ParticleSummaryRow(fileName, Measurement.Count) });
        }

        public string ExportCsv() =>
            Measurement is null ? string.Empty : MeasurementCsv.Particles(Measurement);

        public string ExportJson() =>
            Measurement is null
                ? string.Empty
                : MeasurementJson.Envelope(new MeasurementSource(FileName, null), null, Measurement, null);
    }
}
