using Emgu.CV.CvEnum;

namespace JSharp.Models.SimpleDataModels
{
    public class ConvolutionInfo
    {
        public BorderType BorderPixelsOption { get; set; }
        public double? Min { get; set; } = null;
        public double? Max { get; set; } = null;

        public ConvolutionInfo(BorderType borderType)
        {
            BorderPixelsOption = borderType;
        }

        public ConvolutionInfo(BorderType borderType, double min, double max) : this(borderType)
        {
            Min = min;
            Max = max;
        }
    }
}
