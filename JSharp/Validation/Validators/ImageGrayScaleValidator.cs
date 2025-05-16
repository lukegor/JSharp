using JSharp.Shared.Resources;
using JSharp.Utility.Utility;

namespace JSharp.Validation.Validators
{
    public class ImageGrayScaleValidator : IValidator
    {
        private readonly int? numberOfChannels;

        public ImageGrayScaleValidator(int? numberOfChannels)
        {
            this.numberOfChannels = numberOfChannels;
        }

        public string Validate()
        {
            if (numberOfChannels != Constants.Grayscale_ChannelCount)
            {
                return Errors.ImageNotGrayscale;
            }
            return null; // No error message if condition is met
        }
    }
}
