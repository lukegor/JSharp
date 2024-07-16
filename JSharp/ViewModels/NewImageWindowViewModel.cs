using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using JSharp;
using JSharp.Models;
using JSharp.Resources;
using JSharp.Utility;
using LiveChartsCore.Defaults;
using Microsoft.Win32;
using Prism.Events;
using Prism.Mvvm;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics.CodeAnalysis;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;

namespace JSharp.ViewModels
{
    public class NewImageWindowViewModel : BindableBase
    {
        private const int widthAdjustmentConst = 16;
        private const int heightAdjustmentConst = 39;

        public event EventHandler<string>? FocusChanged;
        public event EventHandler<Mat>? ImageChanged;
        public event EventHandler Closing;

        private System.Windows.Point _mousePosition;
        public System.Windows.Point MousePosition
        {
            get { return _mousePosition; }
            set { SetProperty(ref _mousePosition, value); (App.Current.MainWindow.DataContext as MainWindowViewModel).UpdateDescriptor(); }
        }

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

        private double _height;
        public double Height
        {
            get { return _height; }
            set { SetProperty(ref _height, value); }
        }

        private double _width;
        public double Width
        {
            get { return _width; }
            set { SetProperty(ref _width, value); }
        }
        #endregion

        internal ObservableCollection<System.Windows.Point?> Points { get; set; } = new ObservableCollection<System.Windows.Point?> { null, null };
        private int DuplicateCount { get; set; }
        internal string? filePath;

        internal HistogramWindowViewModel histogramWindowViewModel;

        public NewImageWindowViewModel(BitmapSource source, Mat matImage, string fileName, int duplicateCount)
        {
            Source = source;
            this.MatImage = matImage.Clone();

            string saveFileName = Path.GetFileName(fileName);

            if (saveFileName != fileName)
            {
                filePath = fileName;
            }

            HandleNaming(saveFileName, duplicateCount);

            //only Grayscale and RGB can be created by constructor (open file), so don't worry about HSV and LAB
            ColorSpaceType = ImageProcessingUtility.OnLoadingDetermineColorspace(matImage.NumberOfChannels);
            ZoomScale = 1.0;

            MakeTitleABoundComposite();
            UpdateTitle();

            UpdateWindowSize();
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

            UpdateWindowSize();
        }

        public void UpdateImageSource(Mat newImage)
        {
            this.MatImage = newImage;
            this.Source = newImage.MatToBitmapSource();

            // Trigger the ImageChanged event for histogram to update reactively
            ImageChanged?.Invoke(this, newImage);
        }

        private void UpdateWindowSize()
        {
            this.Width = (Source.Width * ZoomScale) + widthAdjustmentConst;
            this.Height = (Source.Height * ZoomScale) + heightAdjustmentConst;
        }
        #endregion

        #region External-related
        public void SaveChanges()
        {
            if (!string.IsNullOrEmpty(filePath))
                MatImage.Save(filePath);
            else
                SaveAs();
        }

        public void SaveAs()
        {
            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = Constants.ImageFilterString;
            saveFileDialog.Title = "Save Image As...";
            saveFileDialog.FileName = this.FileName; // Set default file name here if needed

            if (saveFileDialog.ShowDialog() == true)
            {
                // Determine the file format based on the selected filter index
                string fileName = saveFileDialog.FileName;
                string fileExtension = System.IO.Path.GetExtension(fileName).ToLower();

                switch (fileExtension)
                {
                    case ".bmp":
                        this.MatImage.Save(fileName);
                        break;
                    case ".jpg":
                    case ".jpeg":
                        // For JPEG format, save the MatImage with JPEG compression quality
                        CvInvoke.Imwrite(fileName, this.MatImage, new[] { new KeyValuePair<ImwriteFlags, int>(ImwriteFlags.JpegQuality, Properties.Settings.Default.jpqSaveQuality) });
                        break;
                    case ".png":
                        // For PNG format, save the MatImage with PNG compression level
                        CvInvoke.Imwrite(fileName, this.MatImage, new[] { new KeyValuePair<ImwriteFlags, int>(ImwriteFlags.PngCompression, Properties.Settings.Default.pngCompressionLevel) });
                        break;
                    case ".gif":
                        // For GIF format, save the MatImage with specified parameters
                        CvInvoke.Imwrite(fileName, this.MatImage);
                        break;
                    case ".tiff":
                        // For TIFF format, save the MatImage with specified parameters
                        CvInvoke.Imwrite(fileName, this.MatImage, new[] { new KeyValuePair<ImwriteFlags, int>(ImwriteFlags.TiffCompression, (int)ImwriteFlags.TiffCompression) });
                        break;
                    default:
                        MessageBox.Show("Invalid file format selected.", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }
            }
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
            List<object> histogramData;
            histogramData = this.histogramWindowViewModel?.HistogramData.ToList()
                            ?? HistogramWindowViewModel.AggregateTableHistogramData(ImageProcessingCore.CalculateHistogramValues(image).histogramData);
            image = ImageProcessingCore.EqualizeHistogram(image, histogramData);
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

        public void DoubleConvolve5x5(IEnumerable<int> resultMatrix, BorderType borderType)
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

        public void DoubleConvolve3x3(IEnumerable<int> firstMatrix, IEnumerable<int> secondMatrix, BorderType borderType)
        {
            Mat image = this.MatImage;

            float[,] firstKernel = new float[3, 3];
            float[,] secondKernel = new float[3, 3];
            int index = 0;
            foreach (int value in firstMatrix)
            {
                firstKernel[index / 3, index % 3] = value;
                index++;
            }
            index = 0;
            foreach (int value in secondMatrix)
            {
                secondKernel[index / 3, index % 3] = value;
                index++;
            }

            image = ImageProcessingCore.FullConvolution(image, firstKernel, secondKernel, 0);

            UpdateImageSource(image);
        }

        public void MedianBlur(int kernelSize, BorderType borderType)
        {
            Mat image = this.MatImage;
            image = ImageProcessingCore.ApplyMedianFilter(image, kernelSize, borderType);
            UpdateImageSource(image);
        }

        public void PerformMorpologicalOperation(BasicMorphologicalInfo basicMorphologicalInfo, MCvScalar borderValue)
        {
            Mat image = this.MatImage;

            MorphologicalOperationType operation = basicMorphologicalInfo.MorphologicalOperationType;
            ShapeType elementShape = basicMorphologicalInfo.ElementShape;
            BorderType borderType = basicMorphologicalInfo.BorderType;
            int size = (int)basicMorphologicalInfo.ElementSize;

            Mat element = elementShape switch
            {
                ShapeType.Rectangle => CvInvoke.GetStructuringElement(ElementShape.Rectangle, new System.Drawing.Size(size, size), new System.Drawing.Point(1, 1)),
                ShapeType.Rhombus => ImageProcessingCore.Diamond(size)
            };

            image = operation switch
            {
                MorphologicalOperationType.Erode => ImageProcessingCore.Erode(image, element, borderType, borderValue),
                MorphologicalOperationType.Dilate => ImageProcessingCore.Dilate(image, element, borderType, borderValue),
                MorphologicalOperationType.MorphOpening => ImageProcessingCore.MorphologicalOpen(image, element, borderType, borderValue),
                MorphologicalOperationType.MorphClosing => ImageProcessingCore.MorphologicalClose(image, element, borderType, borderValue)
            };
            UpdateImageSource(image);
        }

        public void PerformThresholding(Mat img, int minThreshold, int maxThreshold, ThresholdingType thresholdingType, bool enableContrastMode)
        {
            Mat image = img.Clone();

            ImageProcessingCore.Threshold(image, minThreshold, maxThreshold, thresholdingType, enableContrastMode);

            UpdateImageSource(image);
        }

        public void PerformSimpleThresholding(Mat img, int threshold, SimpleThresholdingMethod thresholdingMethod)
        {
            Mat image = img.Clone();

            image = ImageProcessingCore.SimpleThreshold(image, threshold, thresholdingMethod);

            UpdateImageSource(image);
        }

        public void PerformAdaptiveThresholding()
        {
            Mat image = MatImage;
            image = ImageProcessingCore.AdaptiveThreshold(image);
            UpdateImageSource(image);
        }

        public void Restore(Mat image)
        {
            UpdateImageSource(image);
        }

        public void Skeletonize()
        {
            Mat image = this.MatImage;

            image = ImageProcessingCore.Skeletonize(image);

            UpdateImageSource(image);
        }

        public void Hough()
        {
            Mat image = this.MatImage;
            image = ImageProcessingCore.Hough(image);
            UpdateImageSource(image);
        }

        public void Watershed()
        {
            Mat image = this.MatImage;

            image = ImageProcessingCore.Watershed(image);
            UpdateImageSource(image);
        }

        internal void PyramidUp()
        {
            Mat image = this.MatImage;
            CvInvoke.PyrUp(image, image, BorderType.Default);
            UpdateImageSource(image);
            UpdateWindowSize();
        }

        internal void PyramidDown()
        {
            Mat image = this.MatImage;
            CvInvoke.PyrDown(image, image, BorderType.Default);
            UpdateImageSource(image);
            UpdateWindowSize();
        }

        internal void Rotate(RotateFlags rotateFlags)
        {
            Mat image = this.MatImage;
            CvInvoke.Rotate(image, image, rotateFlags);
            UpdateImageSource(image);
            UpdateWindowSize();
        }
        #endregion
    }
}
