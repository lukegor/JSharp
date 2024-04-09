using JSharp.Resources;
using JSharp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Validators
{
    public class ImageXyzValidator
    {
        private readonly int numberOfChannels;
        public ImageXyzValidator(int numberOfChannels)
        {
            this.numberOfChannels = numberOfChannels;
        }

        public string Validate()
        {
            if (this.numberOfChannels == Constants.XYZ_ChannelCount)
            {
                return Strings.AlreadyGrayscale_Error;
            }
            return null; // No error message if condition is met
        }
    }
}
