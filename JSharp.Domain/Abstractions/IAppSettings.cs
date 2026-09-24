namespace JSharp.Domain.Abstractions
{
    public interface IAppSettings
    {
        byte PngCompressionLevel { get; }
        byte JpgSaveQuality { get; }
        string SaveFileExtension { get; }
        string Language { get; }
        double CumulativeZoomFactor { get; }
    }
}
