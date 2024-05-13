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
        private string _selectedFileName1;
        public string SelectedFileName1
        {
            get { return _selectedFileName1; }
            set { SetProperty(ref _selectedFileName1, value); UpdateSelectedImage(value); }
        }
        private Mat _selectedImage1;
        public Mat SelectedImage1
        {
            get { return _selectedImage1; }
            set { SetProperty(ref _selectedImage1, value); }
        }
        private string _selectedFileName2;
        public string SelectedFileName2
        {
            get { return _selectedFileName2; }
            set { SetProperty(ref _selectedFileName2, value); UpdateSelectedMask(value); }
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

        private bool isChecked;
        public bool IsChecked
        {
            get { return isChecked; }
            set { SetProperty(ref isChecked, value); }
        }

        public DelegateCommand BtnConfirm_ClickCommand { get; }

        public InpaintWindowViewModel(List<ImageInfo> images)
        {
            BtnConfirm_ClickCommand = new DelegateCommand(BtnConfirm_Click);

            this.Images = images;
            IsChecked = true;

            SelectedFileName1 = images[0].FileName;
            SelectedFileName2 = images[1].FileName;
        }

        private void UpdateSelectedImage(string value)
        {
            SelectedImage1 = Images.FirstOrDefault(x => x.FileName == value).Image;
        }

        private void UpdateSelectedMask(string value)
        {
            SelectedImage2 = Images.FirstOrDefault(x => x.FileName == value).Image;
        }

        private void BtnConfirm_Click()
        {
            App.Current.Windows.OfType<InpaintWindow>().FirstOrDefault(x => x.DataContext == this).DialogResult = true;
        }
    }
}
