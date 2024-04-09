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
    public enum ColorSpace
    {
        Grayscale,
        RGB,
        HSV,
        LAB
    }

    public static class ColorSpaceExtensions
    {
        public static string GetName(this ColorSpace color)
        {
            return Enum.GetName(typeof(ColorSpace), color);
        }
        public static ImreadModes ColorSpaceToImreadModes(this ColorSpace mycolorEnum)
        {
            return mycolorEnum switch
            {
                ColorSpace.Grayscale => ImreadModes.Grayscale,
                ColorSpace.RGB => ImreadModes.Color,
                _ => throw new NotImplementedException()
            };
        }
    }
}
