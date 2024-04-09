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
        Weights,
        Modulo,
        Scaling
    }

    public static class PixelOverflowHandlingHelper
    {
        public static string GetName(PixelOverflowHandlingType enumValue)
        {
            return enumValue switch
            {
                PixelOverflowHandlingType.Weights => Strings.Weights,
                PixelOverflowHandlingType.Modulo => Strings.Modulo,
                PixelOverflowHandlingType.Scaling => Strings.Scaling,
                _ => throw new InvalidOperationException(),
            };
        }
    }
}
