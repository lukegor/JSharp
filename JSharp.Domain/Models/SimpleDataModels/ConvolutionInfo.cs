using JSharp.Shared.Imaging;

namespace JSharp.Domain.Models.SimpleDataModels
{
    public class ConvolutionInfo
    {
        public BorderMode BorderPixelsOption { get; set; }
        public double? Min { get; set; }
        public double? Max { get; set; }

        public ConvolutionInfo(BorderMode borderType)
        {
            BorderPixelsOption = borderType;
        }

        public ConvolutionInfo(BorderMode borderType, double min, double max) : this(borderType)
        {
            Min = min;
            Max = max;
        }
    }
}
