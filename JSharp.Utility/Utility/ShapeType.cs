using JSharp.Shared.Resources;

namespace JSharp.Utility.Utility
{
    public enum ShapeType
    {
        Rhombus,
        Rectangle
    }

    public static class ShapeTypeHelper
    {
        private static readonly (string ResourceKey, ShapeType Value)[] Entries =
        {
            ("Rhombus", ShapeType.Rhombus),
            ("Rectangle", ShapeType.Rectangle),
        };

        private static readonly LocalizedEnumMap<ShapeType> Map = new(
            Strings.ResourceManager, Entries, () => Strings.Culture);

        public static ShapeType MapLocalStringToShapeType(string input) => Map.Parse(input);

        public static string MapShapeTypeToLocalString(ShapeType shape) => Map.Display(shape);

        public static IEnumerable<string> GetLocalizedShapeTypes(IEnumerable<ShapeType> shapeTypes) =>
            Map.DisplayMany(shapeTypes);
    }
}
