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

        public DelegateCommand SaveSettingsCommand { get; }

        public SettingsWindowViewModel()
        {
            SaveSettingsCommand = new DelegateCommand(SaveCommand);

            PngCompressionLevel = Settings.Default.pngCompressionLevel;
            JpgSaveQuality = Settings.Default.jpqSaveQuality;
            SaveFileExtension = Settings.Default.saveFileExtension;
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
            Settings.Default.Save();

            if (tmp != Language)
            {
                App current = (App)App.Current;
                current.Restart();
            }
        }

        public IEnumerable<string> GetFileExtensionTypes()
        {
            IEnumerable<string> types = new string[] { ".bmp", ".jpg", ".jpeg", ".tiff", ".png" };
            return types;
        }
    }
}
