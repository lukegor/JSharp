using JSharp.Shared.Resources;

namespace JSharp.Utility.Utility
{
    public enum SimpleThresholdingMethod
    {
        Standard,
        Otsu
    }

    public static class SimpleThresholdingTypeHelper
    {
        private static readonly (string ResourceKey, SimpleThresholdingMethod Value)[] Entries =
        {
            ("ThresholdingStandard", SimpleThresholdingMethod.Standard),
            ("ThresholdingOtsu", SimpleThresholdingMethod.Otsu),
        };

        private static readonly LocalizedEnumMap<SimpleThresholdingMethod> Map = new(
            Thresholding.ResourceManager, Entries, () => Thresholding.Culture);

        public static SimpleThresholdingMethod MapLocalStringToThresholdingType(string input) => Map.Parse(input);

        public static string MapThresholdingTypeToLocalString(SimpleThresholdingMethod shape) => Map.Display(shape);

        public static IEnumerable<string> GetLocalizedShapeTypes(IEnumerable<SimpleThresholdingMethod> thresholdingTypes) =>
            Map.DisplayMany(thresholdingTypes);
    }
}
