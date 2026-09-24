using System.IO;

namespace JSharp
{
    /// <summary>
    /// Single source of truth for recipe storage locations and extensions.
    /// </summary>
    public static class RecipePaths
    {
        public static readonly string Directory =
            Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData),
                "JSharp",
                "recipes");

        public const string DefaultExtension = ".jsharp.json";
        public const string BundleExtension = ".jshrecipe";
    }
}
