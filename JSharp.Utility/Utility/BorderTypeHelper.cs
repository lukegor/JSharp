using JSharp.Shared.Imaging;
using JSharp.Shared.Resources;

namespace JSharp.Utility.Utility
{
    public static class BorderTypeHelper
    {
        private static readonly (string ResourceKey, BorderMode Value)[] Entries =
        {
            ("BorderTypeIsolated", BorderMode.Isolated),
            ("BorderTypeReflect", BorderMode.Reflect),
            ("BorderTypeReplicate", BorderMode.Replicate),
            ("BorderTypeReflect101", BorderMode.Reflect101),
            ("BorderTypeWrap", BorderMode.Wrap),
        };

        private static readonly LocalizedEnumMap<BorderMode> Map = new(
            Kernels.ResourceManager, Entries, () => Kernels.Culture);

        public static string LocalizeBorderType(BorderMode borderType) => Map.Display(borderType);

        public static BorderMode BorderizeLocalizedBorderType(string localizedBorderType) => Map.Parse(localizedBorderType);

        public static IEnumerable<string> GetLocalizedEdgePixelsHandlingOptions(IEnumerable<BorderMode> borderTypes) =>
            Map.DisplayMany(borderTypes);
    }
}
