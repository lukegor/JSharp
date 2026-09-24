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
    /// Interaction logic for SummaryWindow.xaml
    /// </summary>
    public partial class SummaryWindow : Window
    {
        private static readonly CompositeFormat ExportFailedFormat =
            CompositeFormat.Parse(Strings.ExportFailed);

        public SummaryWindow()
        {
            InitializeComponent();
        }

        private void Export_Click(object sender, RoutedEventArgs e)
        {
            if (DataContext is not SummaryWindowViewModel vm || vm.Measurement is null)
            {
                return;
            }

            var dialog = new SaveFileDialog
            {
                FileName = "particles.csv",
                DefaultExt = ".csv",
                Filter = Constants.MeasurementFilterString
            };
            if (dialog.ShowDialog() != true)
            {
                return;
            }

            try
            {
                string content = dialog.FileName.EndsWith(".json", StringComparison.OrdinalIgnoreCase)
                    ? vm.ExportJson()
                    : vm.ExportCsv();
                MeasurementsFile.Save(dialog.FileName, content);
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
