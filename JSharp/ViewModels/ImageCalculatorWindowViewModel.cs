using Emgu.CV;
using JSharp.Models;
using JSharp.Utility;
using Prism.Commands;
using Prism.Mvvm;
using SkiaSharp;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.ViewModels
{
    public class ImageCalculatorWindowViewModel : BindableBase
    {
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

        private List<string> _processedValues;
        public List<string> ProcessedValues
        {
            get { return _processedValues; }
            set { SetProperty(ref _processedValues, value); }
        }

        public ImageCalculatorWindowViewModel(List<ImageInfo> images)
        {
            this.Images = images;
            ProcessedValues = GetProcessedEnumValues();
        }

        internal void BtnConfirm_Click(string fileName1, string fileName2, OperationType operation, PixelOverflowHandlingType option)
        {
            this.SelectedFileName1 = fileName1;
            this.SelectedFileName2 = fileName2;

            Mat img1 = Images.First(x => x.FileName == fileName1).Image;
            Mat img2 = Images.First(x => x.FileName == fileName2).Image;

            this.SelectedImage1 = img1;
            this.SelectedImage2 = img2;

            switch (operation)
            {
                case OperationType.ADD:
                    ImageProcessingCore.Add(img1, img2);
                    break;
                case OperationType.SUB:
                    break;
            }
        }

        public List<string> GetProcessedEnumValues()
        {
            List<string> processedValues = new List<string>();
            foreach (PixelOverflowHandlingType value in Enum.GetValues(typeof(PixelOverflowHandlingType)))
            {
                processedValues.Add(PixelOverflowHandlingHelper.GetName(value));
            }
            return processedValues;
        }
    }
}
