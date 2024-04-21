using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using JSharp;
using JSharp.Events;
using JSharp.Models;
using JSharp.Resources;
using JSharp.Utility;
using LiveChartsCore.Defaults;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Diagnostics.CodeAnalysis;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JSharp.ViewModels
{
    public class NewImageWindowViewModel : BindableBase
    {
        public event EventHandler<string>? FocusChanged;
        public event EventHandler<Mat>? ImageChanged;
        public event EventHandler Closing;

        #region dual fields/properties
        private Mat _matImage;
        public Mat MatImage
        {
            get { return _matImage; }
            set
            {
                SetProperty(ref _matImage, value);
                histogramWindowViewModel?.UpdateHistogram(value);
            }
        }

        private double _zoomScale = 1.0;
        public double ZoomScale
        {
            get { return _zoomScale; }
            set { SetProperty(ref _zoomScale, value); }
        }

        /// <summary>
        /// basic name + extension
        /// for counting duplicates
        /// </summary>
        internal string CoreName { get; private set; }

        private string _fileName;
        public string FileName
        {
            get { return _fileName; }
            private set { SetProperty(ref _fileName, value); }
        }

        private BitmapSource? _source;
        public BitmapSource? Source
        {
            get { return _source; }
            set { SetProperty(ref _source, value); }
        }

        private ColorSpaceType _colorSpaceType;
        public ColorSpaceType ColorSpaceType
        {
            get { return _colorSpaceType; }
            set { SetProperty(ref _colorSpaceType, value); }
        }

        private string _title;
        public string Title
        {
            get { return _title; }
            set { SetProperty(ref _title, value); }
        }
        #endregion

        private int DuplicateCount { get; set; }

        internal HistogramWindowViewModel histogramWindowViewModel;

        public NewImageWindowViewModel(BitmapSource source, Mat matImage, string fileName, int duplicateCount)
        {
            Source = source;
            this.MatImage = matImage.Clone();

            HandleNaming(fileName, duplicateCount);

            //only Grayscale and RGB can be created by constructor (open file), so don't worry about HSV and LAB
            ColorSpaceType = ImageProcessingUtility.OnLoadingDetermineColorspace(matImage.NumberOfChannels);
            ZoomScale = 1.0;

            MakeTitleABoundComposite();
            UpdateTitle();

            //BIND this.Width = source.Width;
            //BIND this.Height = source.Height;
        }

        #region Title/name management
        public void MakeTitleABoundComposite()
        {
            PropertyChanged += (sender, e) =>
            {
                if (e.PropertyName == nameof(FileName) ||
                    e.PropertyName == nameof(ColorSpaceType) ||
                    e.PropertyName == nameof(ZoomScale))
                {
                    UpdateTitle();
                }
            };
        }

        private void UpdateTitle()
        {
            Title = $"{FileName} ({ColorSpaceType.GetName()}) ({Math.Round(ZoomScale * 100, 2)}%)";
        }

        private string GetDuplicateString(int duplicateCount)
        {
            if (duplicateCount == 0)
            {
                return string.Empty;
            }
            else
            {
                return $"-{duplicateCount}";
            }
        }

        private void HandleNaming(string fileName, int duplicateCount)
        {
            string coreNameWithoutExtension = System.IO.Path.GetFileNameWithoutExtension(fileName);
            string fileExtension = System.IO.Path.GetExtension(fileName);
            this.CoreName = coreNameWithoutExtension + fileExtension;
            this.DuplicateCount = duplicateCount;
            this.FileName = coreNameWithoutExtension + GetDuplicateString(duplicateCount) + fileExtension;
        }
        #endregion

        #region Other internal UI updates
        internal void ScaleZoom(bool isZoomIn)
        {
            if (isZoomIn)
                ZoomScale *= MainWindowViewModel.CumulativeZoomChange;
            else
                ZoomScale /= MainWindowViewModel.CumulativeZoomChange;
        }

        public void UpdateImageSource(Mat newImage)
        {
            this.MatImage = newImage;
            this.Source = newImage.MatToBitmapSource();

            // Trigger the ImageChanged event for histogram to update reactively
            ImageChanged?.Invoke(this, newImage);
        }
        #endregion

        #region External-related
        public void SaveChanges()
        {
            MatImage.Save(FileName);
        }

        public void Window_Activated()
        {
            MainWindowViewModel.FocusedImage = this;
            string titleForMainWindowLabel = $" ({ColorSpaceType.GetName()}) {FileName}";
            FocusChanged?.Invoke(this, titleForMainWindowLabel);
        }

        public void NewImageWindow_Closing()
        {
            FocusChanged.Invoke(this, "null");
            this.MatImage.Dispose();
            Closing?.Invoke(this, EventArgs.Empty);
        }

        public void CloseAssociatedNewImageWindow()
        {
            (App.Current.Windows.OfType<NewImageWindow>().FirstOrDefault(w => w.DataContext == this))?.Close();
        }
        #endregion

        #region Actions
        #region Histogram-related
        public void MakeHistogram()
        {
            HistogramWindowViewModel histogramWindowViewModel = new HistogramWindowViewModel();
            this.histogramWindowViewModel = histogramWindowViewModel;

            HistogramWindow histogramWindow = new HistogramWindow();
            histogramWindow.DataContext = histogramWindowViewModel;
            histogramWindowViewModel.UpdateHistogram(this.MatImage);
            histogramWindow.Show();
        }

        public void StretchHistogram()
        {
            this.MatImage = ImageProcessingCore.StretchHistogram(this.MatImage);
            UpdateImageSource(this.MatImage);
        }

        /// <summary>
        ///
        /// </summary>
        /// <param name="p1">The lower bound of brightness within which contrast stretching will be applied</param>
        /// <param name="p2">The upper bound of brightness within which contrast stretching will be applied</param>
        /// <param name="q3">The lower bound of brightness into which stretching will be applied</param>
        /// <param name="q4">The upper bound of brightness into which stretching will be applied</param>
        public void StretchContrast(int p1, int p2, int q3, int q4)
        {
            Mat image = this.MatImage;
            image = ImageProcessingCore.StretchContrast(image, p1, p2, q3, q4);
            UpdateImageSource(image);
        }

        public void EqualizeHistogram()
        {
            Mat image = this.MatImage;
            image = ImageProcessingCore.EqualizeHistogram(image, this.histogramWindowViewModel.HistogramData.ToList());
            UpdateImageSource(image);
        }
        #endregion

        [SuppressMessage("Style", "IDE0010:Add missing cases to switch statement", Justification = "It converts to RGB and there's no converting RGB to itself")]
        public void ConvertToRgb(ColorSpaceType space)
        {
            Mat image = this.MatImage;
            switch (space)
            {
                case ColorSpaceType.Grayscale:
                    image = ImageProcessingCore.ConvertRgb(image, ColorSpaceType.Grayscale);
                    this.ColorSpaceType = ColorSpaceType.Grayscale;
                    break;
                case ColorSpaceType.HSV:
                    image = ImageProcessingCore.ConvertRgb(image, ColorSpaceType.HSV);
                    this.ColorSpaceType = ColorSpaceType.HSV;
                    break;
                case ColorSpaceType.LAB:
                    image = ImageProcessingCore.ConvertRgb(image, ColorSpaceType.LAB);
                    this.ColorSpaceType = ColorSpaceType.LAB;
                    break;
                default:
                    throw new InvalidOperationException();
            };
            UpdateImageSource(image);
        }

        public void Negate()
        {
            Mat image = this.MatImage;
            image = ImageProcessingCore.Negate(image);
            this.ColorSpaceType = ColorSpaceType.Grayscale;
            UpdateImageSource(image);
        }

        public void Posterize(int levelsNumber)
        {
            Mat image = this.MatImage;
            image = ImageProcessingCore.Posterize(image, levelsNumber);
            UpdateImageSource(image);
        }

        public void Convolve(string currentKernel, ConvolutionInfo convolutionInfo, IEnumerable<int> TextBoxValues)
        {
            Mat image = this.MatImage;
            BorderType borderType = convolutionInfo.BorderPixelsOption;
            string[] edgeDetectionCases = { Kernels.SobelEW, Kernels.SobelNS, Kernels.Canny, Kernels.Laplacian };
            if (currentKernel == Kernels.BoxBlur)
            {
                image = ImageProcessingCore.ApplyBlur(image, borderType, 3);
            }
            else if (currentKernel == Kernels.GaussianBlur)
            {
                image = ImageProcessingCore.ApplyGaussianBlur(image, borderType, sigmaX: 1.5, sigmaY: 1.5, 3);
            }
            else if (edgeDetectionCases.Contains(currentKernel))
            {
                image = ImageProcessingCore.ApplyEdgeDetectionFilter(image, currentKernel, convolutionInfo);
            }
            else
            {
                float[,] kernel = new float[3, 3];
                int index = 0;
                foreach (int value in TextBoxValues)
                {
                    kernel[index / 3, index % 3] = value;
                    index++;
                }
                image = ImageProcessingCore.ApplyCustomKernel(image, kernel, borderType, 3, delta: 0);
            }
            UpdateImageSource(image);
        }

        public void DoubleConvolve(IEnumerable<int> resultMatrix, BorderType borderType)
        {
            Mat image = this.MatImage;

            float[,] kernel = new float[5, 5];
            int index = 0;
            foreach (int value in resultMatrix)
            {
                kernel[index / 5, index % 5] = value;
                index++;
            }

            image = ImageProcessingCore.ApplyCustomKernel(image, kernel, borderType, 5);

            UpdateImageSource(image);
        }

        public void MedianBlur(int kernelSize, BorderType borderType)
        {
            Mat image = this.MatImage;
            image = ImageProcessingCore.ApplyMedianFilter(image, kernelSize, borderType);
            UpdateImageSource(image);
        }

        public void PerformMorpologicalOperation(MorphologicalOperationType morphologicalOperationType, ElementShape elementShape, BorderType borderType, MCvScalar borderValue)
        {
            Mat image = this.MatImage;


            image = morphologicalOperationType switch
            {
                MorphologicalOperationType.Erode => ImageProcessingCore.Erode(image, elementShape, borderType, borderValue),
                MorphologicalOperationType.Dilate => ImageProcessingCore.Dilate(image, elementShape, borderType, borderValue),
                MorphologicalOperationType.MorphOpening => ImageProcessingCore.MorphologicalOpen(image, elementShape, borderType, borderValue),
                MorphologicalOperationType.MorphClosing => ImageProcessingCore.MorphologicalClose(image, elementShape, borderType, borderValue)
            };
            UpdateImageSource(image);
        }

        public void PerformThresholding(Mat img, int minThreshold, int maxThreshold, ThresholdingType thresholdingType)
        {
            Mat image = img.Clone();

            ImageProcessingCore.Threshold(image, minThreshold, maxThreshold, thresholdingType);

            UpdateImageSource(image);
        }

        public void Restore(Mat image)
        {
            UpdateImageSource(image);
        }

        public void Hough()
        {
            Mat image = this.MatImage;
            image = ImageProcessingCore.Hough(image);
            UpdateImageSource(image);
        }
        #endregion
    }
}
