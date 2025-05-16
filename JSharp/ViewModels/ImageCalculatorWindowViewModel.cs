using CommunityToolkit.Mvvm.ComponentModel;
using Emgu.CV;
using JSharp.Domain.Models.SimpleDataModels;
using JSharp.UI.Views;
using JSharp.Views;

namespace JSharp.ViewModels
{
    public class ImageCalculatorWindowViewModel : ObservableObject
    {
        public event EventHandler<ImageCalculatorInfo> ParametersSelected;

        #region dual fields/properties
        private string _selectedFileName1;
        public string SelectedFileName1
        {
            get { return _selectedFileName1; }
            set { SetProperty(ref _selectedFileName1, value); }
        }
        private string _selectedFileName2;
        public string SelectedFileName2
        {
            get { return _selectedFileName2; }
            set { SetProperty(ref _selectedFileName2, value); }
        }
        private Mat _selectedImage1;
        public Mat SelectedImage1
        {
            get { return _selectedImage1; }
            set { SetProperty(ref _selectedImage1, value); }
        }
        private Mat _selectedImage2;
        public Mat SelectedImage2
        {
            get { return _selectedImage2; }
            set { SetProperty(ref _selectedImage2, value); }
        }

        private List<ImageInfo> _images;
        public List<ImageInfo> Images
        {
            get { return _images; }
            set { SetProperty(ref _images, value); }
        }
        #endregion

        public ImageCalculatorWindowViewModel(List<ImageInfo> images)
        {
            this.Images = images;
        }

        internal void BtnConfirm_Click(string fileName1, string fileName2, OperationData operationData, bool shouldCreateNewWindow)
        {
            this.SelectedFileName1 = fileName1;
            this.SelectedFileName2 = fileName2;

            Mat img1 = Images.First(x => x.FileName == fileName1).Image;
            Mat img2 = Images.First(x => x.FileName == fileName2).Image;

            this.SelectedImage1 = img1;
            this.SelectedImage2 = img2;

            ImageCalculatorInfo imageCalculatorInfo = new ImageCalculatorInfo(img1, img2, operationData, shouldCreateNewWindow);

            (App.Current.Windows.OfType<ImageCalculatorWindow>().FirstOrDefault(x => x.DataContext == this))?.Close();
            ParametersSelected?.Invoke(this, imageCalculatorInfo);
        }
    }
}
