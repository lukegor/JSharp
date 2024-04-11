using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Utility
{
    public static class ImageProcessingUtility
    {
        public static ColorSpaceType OnLoadingDetermineColorspace(int NumberOfChannels)
        {
            return NumberOfChannels switch
            {
                1 => ColorSpaceType.Grayscale,
                3 => ColorSpaceType.RGB,
                _ => throw new NotImplementedException()
            };
        }
    }
}
