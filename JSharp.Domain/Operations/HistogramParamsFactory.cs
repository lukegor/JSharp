using JSharp.Domain.Operations;
#if !JSHARP_OPEN
using JSharp.Domain.Recipes;
#endif
using JSharp.Domain.Services;
using OpenCvSharp;

namespace JSharp.Domain.Operations
{
    /// <summary>
    /// Builds equalize-histogram parameters deterministically from the source image.
    /// Always recomputes instead of reusing stored bins, so replay adapts to the
    /// image it actually runs on.
    /// </summary>
    public static class HistogramParamsFactory
    {
        public static EqualizeHistogramParams FromImage(Mat source)
        {
            ArgumentNullException.ThrowIfNull(source);
            (int[] histogramData, _) = ImageAnalysis.CalculateHistogramValues(source);
            return FromHistogramData(histogramData);
        }

        public static EqualizeHistogramParams FromHistogramData(int[] counts)
        {
            ArgumentNullException.ThrowIfNull(counts);
            if (counts.Length != 256)
            {
#if JSHARP_OPEN
                throw new OperationValidationException(ErrorCodes.Validation, "Equalize-histogram data must contain exactly 256 bins.");
#else
                throw new RecipeFormatException("Equalize-histogram data must contain exactly 256 bins.");
#endif
            }

            return new EqualizeHistogramParams(counts);
        }
    }
}
