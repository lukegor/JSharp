using JSharp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Utility
{
    public enum SimpleThresholdingMethod
    {
        Standard,
        Otsu
    }

    public static class SimpleThresholdingTypeHelper
    {
        private static readonly Dictionary<string, SimpleThresholdingMethod> thresholdingTypeMapping = new Dictionary<string, SimpleThresholdingMethod>()
        {
            { Thresholding.ThresholdingStandard, SimpleThresholdingMethod.Standard },
            { Thresholding.ThresholdingOtsu, SimpleThresholdingMethod.Otsu },
        };

        public static SimpleThresholdingMethod MapLocalStringToThresholdingType(string input)
        {
            return EnumHelper.MapLocalStringToEnum(input, thresholdingTypeMapping);
        }

        public static string MapThresholdingTypeToLocalString(SimpleThresholdingMethod shape)
        {
            return EnumHelper.MapEnumToLocalString(shape, thresholdingTypeMapping);
        }

        public static IEnumerable<string> GetLocalizedShapeTypes(IEnumerable<SimpleThresholdingMethod> thresholdingTypes)
        {
            return EnumHelper.GetLocalizedOptions(thresholdingTypes, thresholdingTypeMapping);
        }
    }
}
