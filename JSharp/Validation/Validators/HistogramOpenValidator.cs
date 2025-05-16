using JSharp.Shared.Resources;
using JSharp.ViewModels;

namespace JSharp.Validation.Validators
{
    internal class HistogramOpenValidator : IValidator
    {
        private HistogramWindowViewModel? histogramWindowViewModel;

        public HistogramOpenValidator(HistogramWindowViewModel? histogramWindowViewModel)
        {
            this.histogramWindowViewModel = histogramWindowViewModel;
        }

        public string Validate()
        {
            if (histogramWindowViewModel == null)
            {
                return Errors.HistogramNotOpen;
            }
            return null; // No error message if condition is met
        }
    }
}
