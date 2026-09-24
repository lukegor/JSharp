using JSharp.Domain.Abstractions;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Domain.Models;
using JSharp.Domain.Models.Mappers;
using JSharp.UI.Views;
using JSharp.ViewModels.Abstractions;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="SettingsWindow"/>
    /// </summary>
    internal class SettingsWindowViewModel : ObservableObject
    {
        public EditableSettings EditableSettings { get; }

        public IList<string> FileExtensionTypes { get; } =
            new[] { ".bmp", ".jpg", ".jpeg", ".tiff", ".png" };


        public RelayCommand SaveSettingsCommand { get; }
        public RelayCommand RestoreDefaultsCommand { get; }

        private readonly ISettingsService _settingsService;

        public SettingsWindowViewModel(ISettingsService settingsService)
        {
            _settingsService = settingsService;

            EditableSettings = settingsService.ToEditables();

            SaveSettingsCommand = new RelayCommand(SaveCommand);
            RestoreDefaultsCommand = new RelayCommand(_settingsService.RestoreDefaults);
        }

        public void SaveCommand()
        {
            _settingsService.SaveSettings(EditableSettings);
        }
    }
}
