using Emgu.CV;

namespace JSharp.Models.SimpleDataModels
{
    public class ImageInfo
    {
        public Mat Image { get; set; }
        public string FileName { get; set; }

        public ImageInfo(Mat image, string description)
        {
            Image = image;
            FileName = description;
        }
    }
}
