using System.IO;

namespace JSharp
{
    /// <summary>
    /// Single source of truth for the location of the application's log directory.
    /// </summary>
    public static class LogPaths
    {
        public static readonly string Directory =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "JSharp",
                "logs");
    }
}
