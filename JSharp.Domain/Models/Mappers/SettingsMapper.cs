using JSharp.Domain.Abstractions;

namespace JSharp.Domain.Models.Mappers
{
    public static class SettingsMapper
    {
        public static EditableSettings ToEditables(this ISettingsService settings)
        {
            return new EditableSettings()
            {
                Language = settings.Language,
                ZoomFactor = settings.ZoomFactor,
                PngCompressionLevel = settings.PngCompressionLevel,
                JpgSaveQuality = settings.JpgSaveQuality,
                SaveFileExtension = settings.SaveFileExtension
            };
        }
    }
}
