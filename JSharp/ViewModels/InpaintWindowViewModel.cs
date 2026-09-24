using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using JSharp.Domain.Models.SimpleDataModels;
using JSharp.Services;
using JSharp.Shared.Resources;
using JSharp.ViewModels.Abstractions;
using OpenCvSharp;
using System.Windows;

namespace JSharp.ViewModels
{
    internal sealed record InpaintSelection(Mat Mask, int Radius);

    /// <summary>
    /// Viewmodel for <see cref="InpaintWindow"/>. Completes the selected mask image.
    /// </summary>
    internal partial class InpaintWindowViewModel : DialogViewModel<InpaintSelection>
    {
        private readonly IMessageService _messageService;

        [ObservableProperty]
        public partial string SelectedFileName1 { get; set; } = null!;

        public Mat? SelectedImage1
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        [ObservableProperty]
        public partial string SelectedFileName2 { get; set; } = null!;

        public Mat? SelectedImage2
        {
            get => field;
            set => SetProperty(ref field, value);
        }

        public List<ImageInfo> Images
        {
            get => field;
            set => SetProperty(ref field, value);
        } = [];

        [ObservableProperty]
        public partial bool IsChecked { get; set; }

        [ObservableProperty]
        public partial int Radius { get; set; } = 3;

        public InpaintWindowViewModel(IMessageService messageService, List<ImageInfo> images)
        {
            _messageService = messageService;

            Images = images;
            IsChecked = true;

            SelectedFileName1 = images[0].FileName;
            SelectedFileName2 = images[1].FileName;
        }

        partial void OnSelectedFileName1Changed(string value) => UpdateSelectedImage(value);

        partial void OnSelectedFileName2Changed(string value) => UpdateSelectedMask(value);

        private void UpdateSelectedImage(string value)
        {
            SelectedImage1 = Images.First(x => x.FileName == value).Image;
        }

        private void UpdateSelectedMask(string value)
        {
            SelectedImage2 = Images.First(x => x.FileName == value).Image;
        }

        [RelayCommand]
        private void Confirm()
        {
            if (SelectedImage1 is not { } selectedImage1 || SelectedImage2 is not { } selectedImage2)
            {
                return;
            }

            if (selectedImage1.Width != selectedImage2.Width || selectedImage1.Height != selectedImage2.Height)
            {
                _messageService.ShowMessage(Errors.ImagesMustBeOfSameSize, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            if (Radius is < 1 or > 20)
            {
                _messageService.ShowMessage("Inpaint radius must be between 1 and 20.", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Complete(new InpaintSelection(SelectedImage1, Radius));
        }
    }
}
