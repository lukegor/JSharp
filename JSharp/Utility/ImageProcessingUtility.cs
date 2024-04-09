using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Utility
{
    public static class ImageProcessingUtility
    {
        public static ColorSpace OnLoadingDetermineColorspace(int NumberOfChannels)
        {
            return NumberOfChannels switch
            {
                1 => ColorSpace.Grayscale,
                3 => ColorSpace.RGB,
                _ => throw new NotImplementedException()
            };
        }
    }
}
