using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Domain.Models.SimpleDataModels;
using JSharp.Services;
using JSharp.Shared.Resources;
using JSharp.ViewModels.Abstractions;
using System.Windows;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="JSharp.UI.Views.AnalyzeParticlesWindow"/>.
    /// </summary>
    internal partial class AnalyzeParticlesWindowViewModel : DialogViewModel<AnalysisSettings>
    {
        private readonly IMessageService _messageService;

        [ObservableProperty]
        public partial string SizeText { get; set; } = "0-inf";

        public AnalyzeParticlesWindowViewModel(IMessageService messageService)
        {
            _messageService = messageService;
        }

        [RelayCommand]
        private void Confirm()
        {
            string[] parts = SizeText.Split('-');
            if (parts.Length != 2 || !int.TryParse(parts[0].Trim(), out int min))
            {
                _messageService.ShowMessage(Errors.InvalidSizeRange, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            int? max = int.TryParse(parts[1].Trim(), out int parsedMax) ? parsedMax : null;
            if (max.HasValue && max.Value < min)
            {
                _messageService.ShowMessage(Errors.InvalidSizeRange, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Complete(new AnalysisSettings(min, max));
        }
    }
}
