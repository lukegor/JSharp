using JSharp.Domain.Abstractions.Validation;
using JSharp.Shared.Resources;
using JSharp.ViewModels;

namespace JSharp.Services.Validation.Validators
{
    internal class ImageNotNullValidator : IValidator
    {
        private readonly NewImageWindowViewModel _focusedImage;
        public ImageNotNullValidator(NewImageWindowViewModel focusedImage)
        {
            _focusedImage = focusedImage;
        }

        public string? Validate()
        {
            if (_focusedImage == null)
            {
                return Errors.NoImageFocused;
            }
            return null; // No error message if condition is met
        }
    }
}
