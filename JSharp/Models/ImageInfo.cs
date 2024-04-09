using Emgu.CV;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Models
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
