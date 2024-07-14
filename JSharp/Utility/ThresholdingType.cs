using JSharp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Utility
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
        private static readonly Dictionary<string, ThresholdingType> thresholdingTypeMapping = new Dictionary<string, ThresholdingType>()
        {
            { Thresholding.ThresholdingStandard, ThresholdingType.Standard },
            { Thresholding.ThresholdingInverse, ThresholdingType.Inverse },
            { Thresholding.ThresholdingPreservingGrayscaleLevels, ThresholdingType.PreservingGrayscaleLevelsIdentity },
            { Thresholding.ThresholdingPreservingGrayscaleLevelsNegation, ThresholdingType.PreservingGrayscaleLevelsNegation }
        };

        public static ThresholdingType MapLocalStringToThresholdingType(string input)
        {
            return EnumHelper.MapLocalStringToEnum(input, thresholdingTypeMapping);
        }

        public static string MapThresholdingTypeToLocalString(ThresholdingType shape)
        {
            return EnumHelper.MapEnumToLocalString(shape, thresholdingTypeMapping);
        }

        public static IEnumerable<string> GetLocalizedShapeTypes(IEnumerable<ThresholdingType> thresholdingTypes)
        {
            return EnumHelper.GetLocalizedOptions(thresholdingTypes, thresholdingTypeMapping);
        }
    }
}
