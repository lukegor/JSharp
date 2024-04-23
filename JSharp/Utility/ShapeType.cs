using Emgu.CV.CvEnum;
using Emgu.CV.Reg;
using JSharp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Utility
{
    public enum ShapeType
    {
        Rhombus,
        Rectangle
    }

    public static class ShapeTypeHelper
    {
        private static readonly Dictionary<string, ShapeType> shapeTypeMapping = new Dictionary<string, ShapeType>
        {
            { Strings.Rhombus, ShapeType.Rhombus },
            { Strings.Rectangle, ShapeType.Rectangle }
        };

        public static ShapeType MapLocalStringToShapeType(string input)
        {
            return EnumHelper.MapLocalStringToEnum(input, shapeTypeMapping);
        }

        public static string MapShapeTypeToLocalString(ShapeType shape)
        {
            return EnumHelper.MapEnumToLocalString(shape, shapeTypeMapping);
        }

        public static IEnumerable<string> GetLocalizedShapeTypes(IEnumerable<ShapeType> shapeTypes)
        {
            return EnumHelper.GetLocalizedOptions(shapeTypes, shapeTypeMapping);
        }
    }
}
