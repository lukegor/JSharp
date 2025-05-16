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
        private static readonly Dictionary<string, ThresholdingType> ThresholdingTypeMapping = new Dictionary<string, ThresholdingType>()
        {
            { Thresholding.ThresholdingStandard, ThresholdingType.Standard },
            { Thresholding.ThresholdingInverse, ThresholdingType.Inverse },
            { Thresholding.ThresholdingPreservingGrayscaleLevels, ThresholdingType.PreservingGrayscaleLevelsIdentity },
            { Thresholding.ThresholdingPreservingGrayscaleLevelsNegation, ThresholdingType.PreservingGrayscaleLevelsNegation }
        };

        public static ThresholdingType MapLocalStringToThresholdingType(string input)
        {
            return EnumHelper.MapLocalStringToEnum(input, ThresholdingTypeMapping);
        }

        public static string MapThresholdingTypeToLocalString(ThresholdingType shape)
        {
            return EnumHelper.MapEnumToLocalString(shape, ThresholdingTypeMapping);
        }

        public static IEnumerable<string> GetLocalizedShapeTypes(IEnumerable<ThresholdingType> thresholdingTypes)
        {
            return EnumHelper.GetLocalizedOptions(thresholdingTypes, ThresholdingTypeMapping);
        }
    }
}
