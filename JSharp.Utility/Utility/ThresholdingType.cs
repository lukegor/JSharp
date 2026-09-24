using JSharp.Shared.Resources;

namespace JSharp.Utility.Utility
{
    public enum ThresholdingType
    {
        Standard,
        Inverse,
        PreservingGrayscaleLevelsIdentity,
        PreservingGrayscaleLevelsNegation
    }

    public static class ThresholdingTypeHelper
    {
        private static readonly (string ResourceKey, ThresholdingType Value)[] Entries =
        {
            ("ThresholdingStandard", ThresholdingType.Standard),
            ("ThresholdingInverse", ThresholdingType.Inverse),
            ("ThresholdingPreservingGrayscaleLevels", ThresholdingType.PreservingGrayscaleLevelsIdentity),
            ("ThresholdingPreservingGrayscaleLevelsNegation", ThresholdingType.PreservingGrayscaleLevelsNegation),
        };

        private static readonly LocalizedEnumMap<ThresholdingType> Map = new(
            Thresholding.ResourceManager, Entries, () => Thresholding.Culture);

        public static ThresholdingType MapLocalStringToThresholdingType(string input) => Map.Parse(input);

        public static string MapThresholdingTypeToLocalString(ThresholdingType shape) => Map.Display(shape);

        public static IEnumerable<string> GetLocalizedShapeTypes(IEnumerable<ThresholdingType> thresholdingTypes) =>
            Map.DisplayMany(thresholdingTypes);
    }
}
