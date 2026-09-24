using JSharp.Utility.Utility;
using OpenCvSharp;

namespace JSharp.Domain.Documents
{
    /// <summary>
    /// Immutable snapshot of one open image: current pixels, color space, and a bounded
    /// undo history. Apply/Undo return new instances; Mats retained in any live instance
    /// stay alive, evicted history entries are disposed.
    /// </summary>
    public sealed class ImageDocument
    {
        public const int MaxHistoryDepth = 10;

        private ImageDocument(Mat currentMat, ColorSpaceType colorSpaceType, IReadOnlyList<Mat> undoHistory)
        {
            CurrentMat = currentMat;
            ColorSpaceType = colorSpaceType;
            UndoHistory = undoHistory;
        }

        public Mat CurrentMat { get; }

        public ColorSpaceType ColorSpaceType { get; }

        /// <summary>Previous pixel states, most recent first.</summary>
        public IReadOnlyList<Mat> UndoHistory { get; }

        public bool CanUndo => UndoHistory.Count > 0;

        public static ImageDocument Create(Mat source) =>
            new(source, DetermineInitialColorSpace(source), Array.Empty<Mat>());

        public ImageDocument Apply(Mat next, ColorSpaceType? newColorSpace = null)
        {
            ArgumentNullException.ThrowIfNull(next);

            List<Mat> history = new(UndoHistory) { CurrentMat };
            while (history.Count > MaxHistoryDepth)
            {
                // Evict the OLDEST state; everything referenced by live documents survives.
                history[0].Dispose();
                history.RemoveAt(0);
            }

            return new ImageDocument(next, newColorSpace ?? ColorSpaceType, history);
        }

        public ImageDocument Undo()
        {
            if (!CanUndo)
            {
                throw new InvalidOperationException("Nothing to undo.");
            }

            Mat restored = UndoHistory[0];
            List<Mat> remaining = UndoHistory.Skip(1).ToList();
            return new ImageDocument(restored, ColorSpaceType, remaining);
        }

        private static ColorSpaceType DetermineInitialColorSpace(Mat source) =>
            ImageProcessingUtility.OnLoadingDetermineColorspace(source.Channels());
    }
}
