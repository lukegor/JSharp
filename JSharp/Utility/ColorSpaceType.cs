using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows.Navigation;

namespace JSharp.Utility
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
            return Enum.GetName(typeof(ColorSpaceType), color);
        }
        public static ImreadModes ColorSpaceToImreadModes(this ColorSpaceType mycolorEnum)
        {
            return mycolorEnum switch
            {
                ColorSpaceType.Grayscale => ImreadModes.Grayscale,
                ColorSpaceType.RGB => ImreadModes.Color,
                _ => throw new NotImplementedException()
            };
        }
    }
}
