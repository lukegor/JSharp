using JSharp.Domain.Models;

namespace JSharp.Domain.Abstractions
{
    public interface ISettingsService
    {
        byte PngCompressionLevel { get; }
        byte JpgSaveQuality { get; }
        string SaveFileExtension { get; }
        string Language { get; }
        double ZoomFactor { get; }

        abstract void SaveSettings(EditableSettings newSettings);
        abstract void RestoreDefaults();
    }
}
