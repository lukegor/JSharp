using JSharp.Resources;
using JSharp.ViewModels;

namespace JSharp.Validation.Validators
{
    public class ImageFocusedValidator : IValidator
    {
        private NewImageWindowViewModel focusedImage;
        public ImageFocusedValidator(NewImageWindowViewModel focusedImage)
        {
            this.focusedImage = focusedImage;
        }

        public string Validate()
        {
            if (focusedImage == null)
            {
                return Errors.NoImageFocused;
            }
            return null; // No error message if condition is met
        }
    }
}
