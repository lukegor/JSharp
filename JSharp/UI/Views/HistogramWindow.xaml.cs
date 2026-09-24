using System;
using System.IO;
using System.Text;
using System.Windows;
using JSharp.Domain.Measurements;
using JSharp.Shared.Resources;
using JSharp.Utility.Utility;
using JSharp.ViewModels;
using Microsoft.Win32;

namespace JSharp.UI.Views
{
    /// <summary>
    /// Interaction logic for HistogramWindow.xaml
    /// </summary>
    public partial class HistogramWindow : Window
    {
        private static readonly CompositeFormat ExportFailedFormat =
            CompositeFormat.Parse(Strings.ExportFailed);

        public HistogramWindow()
        {
            InitializeComponent();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not HistogramWindowViewModel vm || !vm.HasData)
            {
                return;
            }

            var dialog = new SaveFileDialog
            {
                FileName = "histogram.csv",
                DefaultExt = ".csv",
                Filter = Constants.MeasurementChartFilterString
            };
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                byte[]? pngBytes = dialog.FileName.EndsWith(".png", StringComparison.OrdinalIgnoreCase)
                    ? vm.ExportPngBytes()
                    : null;
                if (pngBytes is not null)
                {
                    if (pngBytes.Length == 0)
                    {
                        return;
                    }

                    MeasurementsFile.Save(dialog.FileName, pngBytes);
                }
                else
                {
                    string content = dialog.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                        ? vm.ExportJson()
                        : vm.ExportCsv();
                    MeasurementsFile.Save(dialog.FileName, content);
                }
            }
            catch (Exception ex) when (ex is IOException or UnauthorizedAccessException or ArgumentException)
            {
                MessageBox.Show(this,
                    string.Format(System.Globalization.CultureInfo.InvariantCulture, ExportFailedFormat, ex.Message),
                    Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            }
        }
    }
}
