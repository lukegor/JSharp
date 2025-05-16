using JSharp.Shared.Resources;
using JSharp.Utility.Utility;

namespace JSharp.Validation.Validators
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
            if (numberOfChannels == Constants.Xyz_ChannelCount)
            {
                return Errors.AlreadyGrayscale_Error;
            }
            return null; // No error message if condition is met
        }
    }
}
