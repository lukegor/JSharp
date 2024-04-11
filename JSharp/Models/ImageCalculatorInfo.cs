using Emgu.CV;
using JSharp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Models
{
    public class ImageCalculatorInfo
    {
        public Mat Image1 { get; set; }
        public Mat Image2 { get; set; }
        public OperationType Operation { get; set; }
        public PixelOverflowHandlingType PixelOverflowHandlingOption { get; set; }
        public bool ShouldCreateNewWindow { get; set; }

        public ImageCalculatorInfo(Mat image1, Mat image2, OperationType operation, PixelOverflowHandlingType pixelOverflowHandlingOption, bool shouldCreateNewWindow)
        {
            Image1 = image1;
            Image2 = image2;
            Operation = operation;
            PixelOverflowHandlingOption = pixelOverflowHandlingOption;
            ShouldCreateNewWindow = shouldCreateNewWindow;
        }
    }
}
