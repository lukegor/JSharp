using JSharp.Domain.Models.SimpleDataModels;
using JSharp.ViewModels.Abstractions;
using OpenCvSharp;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Viewmodel for <see cref="ImageCalculatorWindow"/>.
    /// </summary>
    internal partial class ImageCalculatorWindowViewModel : DialogViewModel<ImageCalculatorInfo>
    {
        public string SelectedFileName1
        {
            get => field;
            set => SetProperty(ref field, value);
        } = string.Empty;

        public string SelectedFileName2
        {
            get => field;
            set => SetProperty(ref field, value);
        } = string.Empty;

        public IEnumerable<ImageInfo> Images
        {
            get => field;
            set => SetProperty(ref field, value);
        } = [];

        public ImageCalculatorWindowViewModel(IEnumerable<ImageInfo> images)
        {
            Images = images;
        }

        /// <summary>Called by the view after it parses the combo boxes.</summary>
        internal void Confirm(string fileName1, string fileName2, OperationData operationData, bool shouldCreateNewWindow)
        {
            Mat img1 = Images.First(x => x.FileName == fileName1).Image;
            Mat img2 = Images.First(x => x.FileName == fileName2).Image;

            Complete(new ImageCalculatorInfo(img1, img2, operationData, shouldCreateNewWindow));
        }
    }
}
