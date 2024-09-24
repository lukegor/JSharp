using JSharp.Resources;

namespace JSharp.Utility
{
    public enum SimpleThresholdingMethod
    {
        Standard,
        Otsu
    }

    public static class SimpleThresholdingTypeHelper
    {
        private static readonly Dictionary<string, SimpleThresholdingMethod> ThresholdingTypeMapping = new Dictionary<string, SimpleThresholdingMethod>()
        {
            { Thresholding.ThresholdingStandard, SimpleThresholdingMethod.Standard },
            { Thresholding.ThresholdingOtsu, SimpleThresholdingMethod.Otsu },
        };

        public static SimpleThresholdingMethod MapLocalStringToThresholdingType(string input)
        {
            return EnumHelper.MapLocalStringToEnum(input, ThresholdingTypeMapping);
        }

        public static string MapThresholdingTypeToLocalString(SimpleThresholdingMethod shape)
        {
            return EnumHelper.MapEnumToLocalString(shape, ThresholdingTypeMapping);
        }

        public static IEnumerable<string> GetLocalizedShapeTypes(IEnumerable<SimpleThresholdingMethod> thresholdingTypes)
        {
            return EnumHelper.GetLocalizedOptions(thresholdingTypes, ThresholdingTypeMapping);
        }
    }
}
