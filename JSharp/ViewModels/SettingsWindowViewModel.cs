using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Views.Properties;

namespace JSharp.ViewModels
{
    public class SettingsWindowViewModel : ObservableObject
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

        public RelayCommand SaveSettingsCommand { get; }
        public RelayCommand RestoreDefaultsCommand { get; }

        public SettingsWindowViewModel()
        {
            SaveSettingsCommand = new RelayCommand(SaveCommand);
            RestoreDefaultsCommand = new RelayCommand(RestoreDefaults);

            ProcessPropertyValues();
        }

        private void ProcessPropertyValues()
        {
            PngCompressionLevel = Settings.Default.pngCompressionLevel;
            JpgSaveQuality = Settings.Default.jpqSaveQuality;
            SaveFileExtension = Settings.Default.saveFileExtension;
            ZoomFactor = Convert.ToUInt32(Settings.Default.ZoomFactor * 100);
            Language = string.IsNullOrEmpty(Settings.Default.LanguageVersion)
                ? "English"
                : Settings.Default.LanguageVersion;
        }

        public void SaveCommand()
        {
            Settings.Default.pngCompressionLevel = PngCompressionLevel;
            Settings.Default.jpqSaveQuality = JpgSaveQuality;
            Settings.Default.saveFileExtension = SaveFileExtension;

            var isChangedLanguage = Settings.Default.LanguageVersion != Language;
            Settings.Default.LanguageVersion = Language;

            float zoomFactor = (float)(ZoomFactor / 10.0);

            Settings.Default.ZoomFactor = zoomFactor;

            Settings.Default.Save();

            MainWindowViewModel.CumulativeZoomFactor = zoomFactor;

            if (isChangedLanguage)
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
