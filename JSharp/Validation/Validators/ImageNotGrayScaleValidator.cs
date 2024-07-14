using JSharp.Resources;
using JSharp.Utility;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Validation.Validators
{
    public class ImageNotGrayScaleValidator : IValidator
    {
        private readonly int? numberOfChannels;

        public ImageNotGrayScaleValidator(int? numberOfChannels)
        {
            this.numberOfChannels = numberOfChannels;
        }

        public string Validate()
        {
            if (numberOfChannels == Constants.Grayscale_ChannelCount)
            {
                return Errors.AlreadyGrayscale_Error;
            }
            return null; // No error message if condition is met
        }
    }
}
