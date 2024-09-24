using JSharp.Resources;
using JSharp.Utility;

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
