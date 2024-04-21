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
        Negated,
        PreservingGrayscaleLevelsIdentity,
        PreservingGrayscaleLevelsNegation
    }

    public static class ThresholdingTypeHelper
    {
        private static readonly Dictionary<string, ThresholdingType> thresholdingTypeMapping = new Dictionary<string, ThresholdingType>()
        {
            { Project.ThresholdingStandard, ThresholdingType.Standard },
            { Project.ThresholdingInverse, ThresholdingType.Inverse },
            { Project.ThresholdingNegated, ThresholdingType.Negated },
            { Project.ThresholdingPreservingGrayscaleLevels, ThresholdingType.PreservingGrayscaleLevelsIdentity },
            { Project.ThresholdingPreservingGrayscaleLevelsNegation, ThresholdingType.PreservingGrayscaleLevelsNegation }
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
