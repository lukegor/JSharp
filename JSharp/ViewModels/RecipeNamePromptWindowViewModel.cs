using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.ViewModels.Abstractions;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="JSharp.UI.Views.RecipeNamePromptWindow"/>.
    /// Completes the entered name; blank input is rejected inline.
    /// </summary>
    internal partial class RecipeNamePromptWindowViewModel : DialogViewModel<string>
    {
        [ObservableProperty]
        public partial string Name { get; set; } = string.Empty;

        public string? Error
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        public RecipeNamePromptWindowViewModel(string prefill)
        {
            Name = prefill;
        }

        [RelayCommand]
        private void Ok()
        {
            if (string.IsNullOrWhiteSpace(Name))
            {
                Error = "Enter a name.";
                return;
            }

            Complete(Name.Trim());
        }

        [RelayCommand]
        private void Cancel() => Close();
    }
}
