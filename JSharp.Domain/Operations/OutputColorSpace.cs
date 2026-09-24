using JSharp.Utility.Utility;

namespace JSharp.Domain.Operations
{
    /// <summary>
    /// Single source of truth for post-operation colorspace labels. Declared
    /// colorspace wins; otherwise infer within the Grayscale/RGB pair from the
    /// output channel count (HSV/LAB documents are kept since channels alone
    /// cannot distinguish them). Null means unchanged.
    /// </summary>
    public static class OutputColorSpace
    {
        public static ColorSpaceType? ApplyStep(ColorSpaceType current, string? newColorSpaceName, int outputChannels)
        {
            if (newColorSpaceName is not null
                && Enum.TryParse<ColorSpaceType>(newColorSpaceName, out ColorSpaceType declared))
            {
                return declared.Equals(current) ? null : declared;
            }

            ColorSpaceType inferred = outputChannels switch
            {
                1 => ColorSpaceType.Grayscale,
                3 when current is ColorSpaceType.Grayscale or ColorSpaceType.RGB => ColorSpaceType.RGB,
                _ => current,
            };

            return inferred.Equals(current) ? null : inferred;
        }
    }
}
