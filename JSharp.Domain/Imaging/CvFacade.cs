using JSharp.Shared.Imaging;
using OpenCvSharp;

namespace JSharp.Domain.Imaging;
internal static class CvFacade
{
    public static BorderTypes ToBorder(BorderMode mode) => mode switch
    {
        BorderMode.Default => BorderTypes.Default,
        BorderMode.Constant => BorderTypes.Constant,
        BorderMode.Replicate => BorderTypes.Replicate,
        BorderMode.Reflect => BorderTypes.Reflect,
        BorderMode.Wrap => BorderTypes.Wrap,
        BorderMode.Reflect101 => BorderTypes.Reflect101,
        BorderMode.Transparent => BorderTypes.Transparent,
        BorderMode.Isolated => BorderTypes.Isolated,
        _ => throw new ArgumentOutOfRangeException(nameof(mode)),
    };

    public static MorphShapes ToMorph(MorphShape shape) => shape switch
    {
        MorphShape.Rectangle => MorphShapes.Rect,
        MorphShape.Cross => MorphShapes.Cross,
        _ => throw new ArgumentOutOfRangeException(nameof(shape)),
    };

    public static Mat RectElement(MorphShape shape, int size)
        => Cv2.GetStructuringElement(ToMorph(shape), new Size(size, size));

    public static Mat Gray(int rows, int cols) => new(rows, cols, MatType.CV_8UC1);
    public static Mat Bgr(int rows, int cols, Scalar fill) => new(rows, cols, MatType.CV_8UC3, fill);

    public static byte[] EncodePng(Mat chart)
    {
        ArgumentNullException.ThrowIfNull(chart);
        if (chart.Empty())
        {
            throw new ArgumentException("Chart must not be empty.", nameof(chart));
        }
        Cv2.ImEncode(".png", chart, out byte[] bytes);
        return bytes;
    }
}
