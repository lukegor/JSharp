using JSharp.Utility.Utility;
using OpenCvSharp;

namespace JSharp.Domain.Operations
{
    public sealed record InpaintParams(Mat Mask, int Radius) : OperationParams;

    public sealed record GrabCutParams(Rect Rectangle) : OperationParams;

    public sealed class HoughOperation : IImageOperation<NoParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("segment.hough", "segmentation", "Hough Lines", InputRequirement.Grayscale, typeof(NoParams));

        public Mat Apply(Mat source, NoParams parameters, CancellationToken cancellationToken = default)
        {
            cancellationToken.ThrowIfCancellationRequested();

            using Mat edges = new();
            Cv2.Canny(source, edges, 50, 100);

            // Grayscale canvases are converted to BGR so detected lines render red as intended
            // (the old single-channel path coerced the red scalar to black, erasing lines).
            Mat canvas = source.Clone();
            if (canvas.Channels() == 1)
            {
                Mat colorCanvas = new();
                Cv2.CvtColor(canvas, colorCanvas, ColorConversionCodes.GRAY2BGR);
                canvas.Dispose();
                canvas = colorCanvas;
            }

            LineSegmentPoint[] lines = Cv2.HoughLinesP(edges, 1, Math.PI / 180, 100, 50, 10);
            foreach (LineSegmentPoint line in lines)
            {
                Cv2.Line(canvas, line.P1, line.P2, new Scalar(0, 0, 255), 2);
            }

            return canvas;
        }
    }

    public sealed class SkeletonizeOperation : IImageOperation<NoParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("segment.skeletonize", "segmentation", "Skeletonize", InputRequirement.Any, typeof(NoParams));

        public Mat Apply(Mat source, NoParams parameters, CancellationToken cancellationToken = default)
        {
            Mat skeleton = new(source.Size(), MatType.CV_8UC1);
            skeleton.SetTo(Scalar.All(0)); // fixes F12: uninitialized memory leaked into the output

            Mat working = source.Clone();
            try
            {
                using Mat element = Cv2.GetStructuringElement(MorphShapes.Cross, new Size(3, 3), new Point(-1, -1));
                while (true)
                {
                    cancellationToken.ThrowIfCancellationRequested();

                    using Mat opened = new();
                    Cv2.MorphologyEx(working, opened, MorphTypes.Open, element, new Point(1, 1), 1, BorderTypes.Default, new Scalar());

                    using Mat spikes = new();
                    Cv2.Subtract(working, opened, spikes);

                    using Mat eroded = new();
                    Cv2.Erode(working, eroded, element, new Point(1, 1), 1, BorderTypes.Default, new Scalar());

                    Cv2.BitwiseOr(skeleton, spikes, skeleton);

                    Mat next = eroded.Clone();
                    working.Dispose();
                    working = next;

                    if (Cv2.CountNonZero(working) == 0)
                    {
                        break;
                    }
                }
            }
            finally
            {
                working.Dispose();
            }

            return skeleton;
        }
    }

    public sealed class WatershedOperation : IImageOperation<NoParams>
    {
        public OperationDescriptor Descriptor { get; } =
            // Any, not Color: grayscale sources are converted to BGR internally,
            // matching the historical behavior of the app.
            new("segment.watershed", "segmentation", "Watershed", InputRequirement.Any, typeof(NoParams));

        public Mat Apply(Mat source, NoParams parameters, CancellationToken cancellationToken = default)
        {
            Mat img = source.Clone();
            if (img.Channels() == 1)
            {
                Cv2.CvtColor(img, img, ColorConversionCodes.GRAY2BGR);
            }

            using Mat gray = new();
            Cv2.CvtColor(img, gray, ColorConversionCodes.BGR2GRAY);
            using Mat thresh = new();
            Cv2.Threshold(gray, thresh, 0, 255, ThresholdTypes.BinaryInv | ThresholdTypes.Otsu);

            cancellationToken.ThrowIfCancellationRequested();

            using Mat kernel = Mat.Ones(3, 3, MatType.CV_8UC1);
            using Mat opening = new();
            Cv2.MorphologyEx(thresh, opening, MorphTypes.Open, kernel, new Point(-1, -1), 2, BorderTypes.Default, new Scalar());
            using Mat sureBg = new();
            Cv2.Dilate(opening, sureBg, kernel, iterations: 3);

            using Mat distanceTransform = new();
            Cv2.DistanceTransform(opening, distanceTransform, DistanceTypes.L2, DistanceTransformMasks.Mask5);
            Cv2.MinMaxLoc(distanceTransform, out _, out double maxVal);

            using Mat sureFg0 = new();
            Cv2.Threshold(distanceTransform, sureFg0, 0.7 * maxVal, 255, ThresholdTypes.Binary);
            using Mat sureFg = new();
            sureFg0.ConvertTo(sureFg, MatType.CV_8U);

            using Mat unknown = new();
            Cv2.Subtract(sureBg, sureFg, unknown);

            using Mat initialMarkers = new();
            Cv2.ConnectedComponents(sureFg, initialMarkers, PixelConnectivity.Connectivity8, MatType.CV_32S);
            using Mat markers = new();
            Cv2.Add(initialMarkers, new Scalar(1), markers);

            using Mat zeros = markers - markers;
            zeros.CopyTo(markers, unknown);

            Cv2.Watershed(img, markers);

            cancellationToken.ThrowIfCancellationRequested();

            using Mat mask = new();
            zeros.SetTo(new Scalar(-1));
            Cv2.Compare(markers, zeros, mask, CmpTypes.EQ);
            mask.ConvertTo(mask, MatType.CV_8U);
            using Mat blue = new(img.Rows, img.Cols, MatType.CV_8UC3);
            blue.SetTo(new Scalar(255, 0, 0));
            blue.CopyTo(img, mask);

            return img;
        }
    }

    public sealed class InpaintOperation : IImageOperation<InpaintParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("segment.inpaint", "segmentation", "Inpaint", InputRequirement.Any, typeof(InpaintParams));

        public Mat Apply(Mat source, InpaintParams parameters, CancellationToken cancellationToken = default)
        {
            Mat result = new(source.Size(), source.Type());
            Cv2.Inpaint(source, parameters.Mask, result, parameters.Radius, InpaintTypes.Telea);
            return result;
        }
    }

    public sealed class GrabCutOperation : IImageOperation<GrabCutParams>
    {
        public OperationDescriptor Descriptor { get; } =
            new("segment.grabcut", "segmentation", "GrabCut", InputRequirement.Any, typeof(GrabCutParams));

        public Mat Apply(Mat source, GrabCutParams parameters, CancellationToken cancellationToken = default)
        {
            Mat formatted = source.Clone();
            if (formatted.Type() != MatType.CV_8UC3)
            {
                formatted.ConvertTo(formatted, MatType.CV_8U);
                Cv2.CvtColor(formatted, formatted, ColorConversionCodes.BGR2RGB);
            }

            using Mat dst = new(source.Size(), MatType.CV_8UC1);
            dst.SetTo(new Scalar(2));
            using Mat bgModel = new();
            using Mat fgModel = new();

            Cv2.GrabCut(formatted, dst, parameters.Rectangle, bgModel, fgModel, 5, GrabCutModes.InitWithRect);

            Mat foreground = new(formatted.Size(), MatType.CV_8UC3);
            foreground.SetTo(new Scalar(255, 255, 255));
            formatted.CopyTo(foreground, dst);
            return foreground;
        }
    }
}
