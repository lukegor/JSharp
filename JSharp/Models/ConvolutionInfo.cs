using Emgu.CV.CvEnum;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Models
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
