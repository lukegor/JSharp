using JSharp.Shared.Resources;
using JSharp.Utility.Utility;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Builds the status-bar descriptor text for the profile-line / rectangle tools.
    /// Returns null when the descriptor must be left unchanged (rectangle tool with no image).
    /// </summary>
    internal static class StatusBarDescriptorBuilder
    {
        public static string? Build(string selectedTool, System.Windows.Point mousePosition, System.Collections.Generic.IReadOnlyList<System.Windows.Point?>? points)
        {
            if (selectedTool == Tools.ProfileLine)
            {
                if (points is null)
                {
                    return string.Empty;
                }

                System.Text.StringBuilder sb = new();

                sb.Append($"X: {mousePosition.X}, Y: {mousePosition.Y}");

                System.Windows.Point? point1 = points.Count > 0 ? points[0] : null;
                if (point1 != null)
                {
                    sb.Append($", {WindowSpecific.Point} 1: ({point1})");

                    System.Windows.Point? point2 = points.Count > 1 ? points[1] : null;
                    if (point2 != null)
                    {
                        sb.Append($", {WindowSpecific.Point} 2: ({point2})");
                        sb.Append($", {WindowSpecific.Length}: {Math.Floor(ImageProcessingUtility.GetDistance(point1.Value, point2.Value))}");
                    }
                }

                return sb.ToString();
            }

            if (selectedTool == Tools.Rectangle)
            {
                if (points is null)
                {
                    return null;
                }

                System.Text.StringBuilder sb = new();

                sb.Append($"X: {mousePosition.X}, Y: {mousePosition.Y}");

                System.Windows.Point? startPoint = points.Count > 0 ? points[0] : null;
                if (startPoint != null)
                {
                    sb.Append($", Top-left {WindowSpecific.Point} 1: ({startPoint})");

                    System.Windows.Point? endPoint = points.Count > 1 ? points[1] : null;
                    if (endPoint != null)
                    {
                        sb.Append($", Bottom-right {WindowSpecific.Point} 2: ({endPoint})");

                        double width = Math.Abs(endPoint.Value.X - startPoint.Value.X);
                        double height = Math.Abs(endPoint.Value.Y - startPoint.Value.Y);

                        double x = Math.Min(startPoint.Value.X, endPoint.Value.X);
                        double y = Math.Min(startPoint.Value.Y, endPoint.Value.Y);

                        sb.Append(' ').Append($"Rectangle = (x: {x}, y: {y}, width: {width}, height: {height})");
                    }
                }

                return sb.ToString();
            }

            if (selectedTool == Tools.None)
            {
                return string.Empty;
            }

            return null;
        }
    }
}
