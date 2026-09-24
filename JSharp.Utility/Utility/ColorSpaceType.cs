using OpenCvSharp;
using System.Diagnostics;

namespace JSharp.Utility.Utility
{
    public enum ColorSpaceType
    {
        Grayscale,
        RGB,
        HSV,
        LAB
    }

    public static class ColorSpaceExtensions
    {
        public static string GetName(this ColorSpaceType color)
        {
            return Enum.GetName(color) ?? color.ToString();
        }
        public static ImreadModes ColorSpaceToImreadModes(this ColorSpaceType mycolorEnum)
        {
            return mycolorEnum switch
            {
                ColorSpaceType.Grayscale => ImreadModes.Grayscale,
                ColorSpaceType.RGB => ImreadModes.AnyColor,
                _ => throw new UnreachableException("Enum mapping exhausted without a match - non-exhaustive switch.")
            };
        }
    }
}
