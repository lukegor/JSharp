using JSharp.Domain.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using JSharp.Domain.Models;
using JSharp.Services;
using JSharp.Properties;
using JSharp.Shared.Resources;
using Microsoft.Extensions.Logging;
using Microsoft.Extensions.Logging.Abstractions;
using System.Windows;

namespace JSharp.Services
{
    /// <summary>
    /// Provides app-wide access to committed settings.
    /// Reads/writes from <see cref="Properties.Settings.Default"/>
    /// </summary>
    internal class SettingsService : ObservableObject, IAppSettings, ISettingsService
    {
        public byte PngCompressionLevel
        {
            get => field;
            private set => SetProperty(ref field, value);
        }

        public byte JpgSaveQuality
        {
            get => field;
            private set => SetProperty(ref field, value);
        }

        public string SaveFileExtension
        {
            get => field;
            private set => SetProperty(ref field, value);
        } = null!;

        public string Language
        {
            get => field;
            private set => SetProperty(ref field, value);
        } = null!;

        private static readonly double BaseZoomFactor = 1.0;
        public double ZoomFactor
        {
            get => field;
            private set => SetProperty(ref field, value);
        }
        public double CumulativeZoomFactor => BaseZoomFactor + ZoomFactor / 100.0;

        private readonly IMessageService _messageService;
        private readonly ILogger<SettingsService> _logger;

        public SettingsService(IMessageService messageService, ILogger<SettingsService>? logger = null)
        {
            _messageService = messageService;
            _logger = logger ?? NullLogger<SettingsService>.Instance;

            ProcessPropertyValues();
        }

        private void ProcessPropertyValues()
        {
            try
            {
                LoadSettings();
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Failed to load user settings; falling back to defaults.");

                _messageService.ShowMessage(Errors.LoadingSettingsFailed, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);

                LoadDefaultSettings();
            }
        }

        private void LoadSettings()
        {

            Language = string.IsNullOrEmpty(Settings.Default.LanguageVersion)
                    ? "English"
                    : Settings.Default.LanguageVersion;
            PngCompressionLevel = Settings.Default.pngCompressionLevel;
            JpgSaveQuality = Settings.Default.jpqSaveQuality;
            SaveFileExtension = Settings.Default.saveFileExtension;
            ZoomFactor = Settings.Default.ZoomFactor;
        }

        private void LoadDefaultSettings()
        {
            Language = (string)GetDefaultSettingValue(nameof(Settings.Default.LanguageVersion));
            PngCompressionLevel = (byte)GetDefaultSettingValue(nameof(Settings.Default.pngCompressionLevel));
            JpgSaveQuality = (byte)GetDefaultSettingValue(nameof(Settings.Default.jpqSaveQuality));
            SaveFileExtension = (string)GetDefaultSettingValue(nameof(Settings.Default.saveFileExtension));
            ZoomFactor = Convert.ToDouble(GetDefaultSettingValue(nameof(Settings.Default.ZoomFactor)));
        }

        private object GetDefaultSettingValue(string propertyName)
        {
            return Settings.Default.Properties[propertyName].DefaultValue;
        }

        public void SaveSettings(EditableSettings newSettings)
        {
            Settings.Default.pngCompressionLevel = newSettings.PngCompressionLevel;
            Settings.Default.jpqSaveQuality = newSettings.JpgSaveQuality;
            Settings.Default.saveFileExtension = newSettings.SaveFileExtension;

            var isChangedLanguage = Settings.Default.LanguageVersion != newSettings.Language;
            Settings.Default.LanguageVersion = newSettings.Language;

            Settings.Default.ZoomFactor = (float)newSettings.ZoomFactor;

            Settings.Default.Save();

            if (isChangedLanguage)
            {
                App.Restart();
            }

            // Reload settings
            ProcessPropertyValues();
        }

        public void RestoreDefaults()
        {
            Settings.Default.Reset();
        }
    }
}
