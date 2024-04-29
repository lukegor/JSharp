using Emgu.CV;
using JSharp.Models;
using Prism.Commands;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.ViewModels
{
    public class InpaintWindowViewModel : BindableBase
    {
        private string _selectedFileName;
        public string SelectedFileName
        {
            get { return _selectedFileName; }
            set { SetProperty(ref _selectedFileName, value); UpdateSelectedMat(value); }
        }
        private Mat _selectedImage;
        public Mat SelectedImage
        {
            get { return _selectedImage; }
            set { SetProperty(ref _selectedImage, value); }
        }
        private List<ImageInfo> _images;
        public List<ImageInfo> Images
        {
            get { return _images; }
            set { SetProperty(ref _images, value); }
        }

        public DelegateCommand BtnConfirm_ClickCommand { get; }

        public InpaintWindowViewModel(List<ImageInfo> images)
        {
            BtnConfirm_ClickCommand = new DelegateCommand(BtnConfirm_Click);

            this.Images = images;

            SelectedFileName = images[0].FileName;
        }

        private void UpdateSelectedMat(string value)
        {
            SelectedImage = Images.FirstOrDefault(x => x.FileName == value).Image;
        }

        private void BtnConfirm_Click()
        {
            App.Current.Windows.OfType<InpaintWindow>().FirstOrDefault(x => x.DataContext == this).DialogResult = true;
        }
    }
}
