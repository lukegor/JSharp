using JSharp.Domain.Abstractions.Validation;
using JSharp.Shared.Resources;
using JSharp.ViewModels;

namespace JSharp.Services.Validation.Validators
{
    internal class HistogramOpenValidator : IValidator
    {
        private readonly HistogramWindowViewModel? histogramWindowViewModel;

        public HistogramOpenValidator(HistogramWindowViewModel? histogramWindowViewModel)
        {
            this.histogramWindowViewModel = histogramWindowViewModel;
        }

        public string? Validate()
        {
            if (histogramWindowViewModel == null)
            {
                return Errors.HistogramNotOpen;
            }
            return null; // No error message if condition is met
        }
    }
}
