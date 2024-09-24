using JSharp.Resources;

namespace JSharp.Utility
{
    public enum ShapeType
    {
        Rhombus,
        Rectangle
    }

    public static class ShapeTypeHelper
    {
        private static readonly Dictionary<string, ShapeType> ShapeTypeMapping = new Dictionary<string, ShapeType>
        {
            { Strings.Rhombus, ShapeType.Rhombus },
            { Strings.Rectangle, ShapeType.Rectangle }
        };

        public static ShapeType MapLocalStringToShapeType(string input)
        {
            return EnumHelper.MapLocalStringToEnum(input, ShapeTypeMapping);
        }

        public static string MapShapeTypeToLocalString(ShapeType shape)
        {
            return EnumHelper.MapEnumToLocalString(shape, ShapeTypeMapping);
        }

        public static IEnumerable<string> GetLocalizedShapeTypes(IEnumerable<ShapeType> shapeTypes)
        {
            return EnumHelper.GetLocalizedOptions(shapeTypes, ShapeTypeMapping);
        }
    }
}
