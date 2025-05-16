using Emgu.CV;

namespace JSharp.Domain.Models.SimpleDataModels
{
    public class ImageCalculatorInfo
    {
        public Mat Image1 { get; set; }
        public Mat Image2 { get; set; }
        public OperationData OperationData { get; set; }
        public bool ShouldCreateNewWindow { get; set; }

        public ImageCalculatorInfo(Mat image1, Mat image2, OperationData operationData, bool shouldCreateNewWindow)
        {
            Image1 = image1;
            Image2 = image2;
            OperationData = operationData;
            ShouldCreateNewWindow = shouldCreateNewWindow;
        }
    }
}
