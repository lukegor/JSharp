using CommunityToolkit.Mvvm.ComponentModel;

namespace JSharp.Domain.Models
{
    /// <summary>
    /// Represents a temporary, editable copy of application settings for the UI and passing settings to be saved
    /// </summary>
    public class EditableSettings : ObservableObject
    {
        public string Language
        {
            get => field;
            set => SetProperty(ref field, value);
        } = string.Empty;

        public double ZoomFactor
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        public byte PngCompressionLevel
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        public byte JpgSaveQuality
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        public string SaveFileExtension
        {
            get => field;
            set => SetProperty(ref field, value);
        } = string.Empty;

        public EditableSettings()
        {
        }
    }
}
