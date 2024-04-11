using JSharp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Utility
{
    public enum PixelOverflowHandlingType
    {
        None_Clipping,
        Weights,
        Modulo,
        LinearScaling,
        AdaptiveScaling
    }

    public static class PixelOverflowHandlingHelper
    {
        public static string GetName(PixelOverflowHandlingType enumValue)
        {
            return enumValue switch
            {
                PixelOverflowHandlingType.None_Clipping => Strings.None_Clipping,
                PixelOverflowHandlingType.Weights => Strings.Weights,
                PixelOverflowHandlingType.Modulo => Strings.Modulo,
                PixelOverflowHandlingType.LinearScaling => Strings.LinearScaling,
                PixelOverflowHandlingType.AdaptiveScaling => Strings.AdaptiveScaling,
                _ => throw new InvalidOperationException(),
            };
        }

        private static Dictionary<string, PixelOverflowHandlingType> stringToEnumMapping = new Dictionary<string, PixelOverflowHandlingType>
        {
            { Strings.None_Clipping, PixelOverflowHandlingType.None_Clipping },
            { Strings.Weights, PixelOverflowHandlingType.Weights },
            { Strings.Modulo, PixelOverflowHandlingType.Modulo },
            { Strings.LinearScaling, PixelOverflowHandlingType.LinearScaling },
            { Strings.AdaptiveScaling, PixelOverflowHandlingType.AdaptiveScaling },
        };

        public static PixelOverflowHandlingType GetStringToEnumMapping(string selectedString)
        {
            if (stringToEnumMapping.TryGetValue(selectedString, out PixelOverflowHandlingType method))
            {
                return method;
            }
            else
            {
                throw new ArgumentException("Invalid selected string.");
            }
        }
    }
}
