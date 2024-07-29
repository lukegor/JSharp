using JSharp.Properties;
using JSharp.Utility;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.ViewModels
{
    public class SettingsWindowViewModel : BindableBase
    {
        private byte _pngCompressionLevel;
        public byte PngCompressionLevel
        {
            get { return _pngCompressionLevel; }
            set { SetProperty(ref _pngCompressionLevel, value); }
        }

        private byte _jpgSaveQuality;
        public byte JpgSaveQuality
        {
            get { return _jpgSaveQuality; }
            set { SetProperty(ref _jpgSaveQuality, value); }
        }

        private string _saveFileExtension;
        public string SaveFileExtension
        {
            get { return _saveFileExtension; }
            set { SetProperty(ref _saveFileExtension, value); }
        }

        private string language;
        public string Language
        {
            get { return language; }
            set { SetProperty(ref language, value); }
        }

        public uint _zoomFactor;
        public uint ZoomFactor
        {
            get { return _zoomFactor; }
            set { SetProperty(ref _zoomFactor, value); }
        }

        public DelegateCommand SaveSettingsCommand { get; }
        public DelegateCommand RestoreDefaultsCommand { get; }

        public SettingsWindowViewModel()
        {
            SaveSettingsCommand = new DelegateCommand(SaveCommand);
            RestoreDefaultsCommand = new DelegateCommand(RestoreDefaults);

            PngCompressionLevel = Settings.Default.pngCompressionLevel;
            JpgSaveQuality = Settings.Default.jpqSaveQuality;
            SaveFileExtension = Settings.Default.saveFileExtension;
            ZoomFactor = Convert.ToUInt32(Settings.Default.ZoomFactor * 100);
            if (string.IsNullOrEmpty(Settings.Default.LanguageVersion))
                Language = "English";
            else Language = Settings.Default.LanguageVersion;
        }

        public void SaveCommand()
        {
            Settings.Default.pngCompressionLevel = PngCompressionLevel;
            Settings.Default.jpqSaveQuality = JpgSaveQuality;
            Settings.Default.saveFileExtension = SaveFileExtension;
            var tmp = Settings.Default.LanguageVersion;
            Settings.Default.LanguageVersion = Language;
            float zoomFactor = (float)(ZoomFactor / 10.0);
            Settings.Default.ZoomFactor = zoomFactor;
            Settings.Default.Save();
            MainWindowViewModel.CumulativeZoomFactor = zoomFactor;

            if (tmp != Language)
            {
                App current = (App)App.Current;
                current.Restart();
            }
        }

        public void RestoreDefaults()
        {
            Settings.Default.Reset();
        }

        public IEnumerable<string> GetFileExtensionTypes()
        {
            IEnumerable<string> types = new string[] { ".bmp", ".jpg", ".jpeg", ".tiff", ".png" };
            return types;
        }
    }
}
