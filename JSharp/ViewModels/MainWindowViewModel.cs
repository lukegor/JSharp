using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using JSharp.Models;
using JSharp.Resources;
using JSharp.Utility;
using JSharp.Validators;
using Microsoft.Win32;
using Prism.Commands;
using Prism.Events;
using Prism.Mvvm;
using Prism.Services.Dialogs;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Drawing.Imaging;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Media.Imaging;

namespace JSharp.ViewModels
{
    internal class MainWindowViewModel : BindableBase
    {
        private ObservableCollection<NewImageWindowViewModel> _openImageWindows = new ObservableCollection<NewImageWindowViewModel>();
        public ObservableCollection<NewImageWindowViewModel> OpenImageWindows
        {
            get { return _openImageWindows; }
            set { SetProperty(ref _openImageWindows, value); }
        }

        public static NewImageWindowViewModel FocusedImage { get; set; }

        private string _lblFocusedImageContent;
        public string LblFocusedImageContent
        {
            get => _lblFocusedImageContent;
            set => SetProperty(ref _lblFocusedImageContent, value);
        }

        public static System.Windows.Controls.RadioButton SelectedButton { get; set; }

        internal static ObservableCollection<Point?> points { get; set; } = new ObservableCollection<Point?> { null, null };

        private static double zoomChange = 0.2;
        public static double CumulativeZoomChange { get => 1.0 + zoomChange; set => zoomChange = value; }

        #region Commands
        public DelegateCommand OpenRgb_ClickCommand { get; }
        public DelegateCommand OpenGray_ClickCommand { get; }
        public DelegateCommand Duplicate_CommandClick { get; }
        public DelegateCommand Save_ClickCommand { get; }
        public DelegateCommand SaveAll_ClickCommand { get; }
        public DelegateCommand Negate_ClickCommand { get; }
        public DelegateCommand Grayize_ClickCommand { get; }
        public DelegateCommand SaveAs_ClickCommand { get; }
        public DelegateCommand Exit_ClickCommand { get; }
        public DelegateCommand ShowHistogram_ClickCommand { get; }
        public DelegateCommand ConvertRgbToHsv_ClickCommand { get; }
        public DelegateCommand SplitChannels_ClickCommand { get; }
        public DelegateCommand ConvertRgbToLab_ClickCommand { get; }
        public DelegateCommand StretchHistogram_ClickCommand { get; }
        public DelegateCommand StretchContrast_ClickCommand { get; }
        public DelegateCommand EqualizeHistogram_ClickCommand { get; }
        public DelegateCommand Posterize_ClickCommand { get; }
        public DelegateCommand Convolve_ClickCommand { get; }
        public DelegateCommand ImageCalculator_ClickCommand { get; }
        public DelegateCommand Median_ClickCommand { get; }
        public DelegateCommand Erode_ClickCommand { get; }
        public DelegateCommand Dilate_ClickCommand { get; }
        public DelegateCommand MorphologicalOpen_ClickCommand { get; }
        public DelegateCommand MorphologicalClose_ClickCommand { get; }
        public DelegateCommand DoubleConvolve_ClickCommand { get; }
        public DelegateCommand Threshold_ClickCommand { get; }
        public DelegateCommand SimpleAnalyze_ClickCommand { get; }
        public DelegateCommand Analyze_ClickCommand { get; }
        public DelegateCommand Hough_ClickCommand { get; }
        public DelegateCommand Skeletonize_ClickCommand { get; }
        public DelegateCommand PlotProfile_ClickCommand { get; }
        #endregion

        public MainWindowViewModel()
        {
            #region Commands Initialization
            OpenRgb_ClickCommand = new DelegateCommand(OpenRgb_Click);
            OpenGray_ClickCommand = new DelegateCommand(OpenGray_Click);
            Duplicate_CommandClick = new DelegateCommand(Duplicate_Click);
            Save_ClickCommand = new DelegateCommand(Save);
            SaveAll_ClickCommand = new DelegateCommand(SaveAll_Click);
            Negate_ClickCommand = new DelegateCommand(Negate_Click);
            Grayize_ClickCommand = new DelegateCommand(Grayize_Click);
            SaveAs_ClickCommand = new DelegateCommand(SaveAs_Click);
            Exit_ClickCommand = new DelegateCommand(Exit_Click);
            ShowHistogram_ClickCommand = new DelegateCommand(ShowHistogram_Click);
            ConvertRgbToHsv_ClickCommand = new DelegateCommand(ConvertRgbToHsv_Click);
            SplitChannels_ClickCommand = new DelegateCommand(SplitChannels_Click);
            ConvertRgbToLab_ClickCommand = new DelegateCommand(ConvertRgbToLab_Click);
            StretchHistogram_ClickCommand = new DelegateCommand(StretchHistogram_Click);
            StretchContrast_ClickCommand = new DelegateCommand(StretchContrast_Click);
            EqualizeHistogram_ClickCommand = new DelegateCommand(EqualizeHistogram_Click);
            Posterize_ClickCommand = new DelegateCommand(Posterize_Click);
            Convolve_ClickCommand = new DelegateCommand(Convolve_Click);
            ImageCalculator_ClickCommand = new DelegateCommand(ImageCalculator_Click);
            Median_ClickCommand = new DelegateCommand(Median_Click);
            Erode_ClickCommand = new DelegateCommand(Erode_Click);
            Dilate_ClickCommand = new DelegateCommand(Dilate_Click);
            MorphologicalOpen_ClickCommand = new DelegateCommand(MorphologicalOpen_Click);
            MorphologicalClose_ClickCommand = new DelegateCommand(MorphologicalClose_Click);
            DoubleConvolve_ClickCommand = new DelegateCommand(DoubleConvolve_Click);
            Threshold_ClickCommand = new DelegateCommand(Threshold_Click);
            SimpleAnalyze_ClickCommand = new DelegateCommand(SimpleAnalyze_Click);
            Analyze_ClickCommand = new DelegateCommand(Analyze_Click);
            Skeletonize_ClickCommand = new DelegateCommand(Skeletonize_Click);
            Hough_ClickCommand = new DelegateCommand(Hough_Click);
            PlotProfile_ClickCommand = new DelegateCommand(PlotProfile_Click);
            #endregion
        }

        internal void UpdateCheckedRadioButton(object sender)
        {
            SelectedButton = sender as RadioButton;
        }

        private void OpenRgb_Click()
        {
            (List<Mat> matImages, List<string> fileNames) = LoadImageFromFile(ImreadModes.Color);
            if (matImages.Count > 0)
            {
                for (int i = 0; i < matImages.Count; ++i)
                {
                    DisplayImage(matImages[i], fileNames[i]);
                }
            }
        }

        private void OpenGray_Click()
        {
            (List<Mat> matImages, List<string> fileNames) = LoadImageFromFile(ImreadModes.Grayscale);
            if (matImages.Count > 0)
            {
                for (int i = 0; i < matImages.Count; ++i)
                {
                    DisplayImage(matImages[i], fileNames[i]);
                }
            }
        }

        private void Duplicate_Click()
        {
            if (FocusedImage != null)
            {
                DisplayImage(FocusedImage.MatImage, FocusedImage.FileName);
            }
        }

        private (List<Mat>, List<string>) LoadImageFromFile(ImreadModes color)
        {
            OpenFileDialog openFileDialog = new OpenFileDialog();
            openFileDialog.Multiselect = true;
            openFileDialog.DefaultExt = ".bmp";
            openFileDialog.Filter = Constants.ImageFilterString;

            List<Mat> images = new List<Mat>();
            List<string> fileNames = new List<string>();

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    Mat imageMat = CvInvoke.Imread(fileName, color);
                    if (!imageMat.IsEmpty && imageMat != null)
                    {
                        images.Add(imageMat);
                        fileNames.Add(Path.GetFileName(fileName));
                    }
                    else
                    {
                        MessageBox.Show(Strings.LoadingFailed);
                    }
                }
            }
            return (images, fileNames);
        }

        internal void DisplayImage(Mat matImage, string fileName)
        {
            BitmapSource source = matImage.MatToBitmapSource();

            int duplicateCount = OpenImageWindows.Count(x => x.CoreName == fileName);

            NewImageWindowViewModel imageWindowViewModel = new NewImageWindowViewModel(source, matImage, fileName, duplicateCount);
            NewImageWindow newImageWindow = new NewImageWindow();

            OpenImageWindows.Add(imageWindowViewModel);
            imageWindowViewModel.FocusChanged += HandleFocusChanged!;
            imageWindowViewModel.ImageChanged += NewImageWindow_ImageChanged!;
            imageWindowViewModel.Closing += OnNewImageWindowClosing!;

            newImageWindow.DataContext = imageWindowViewModel;
            newImageWindow.Show();
        }

        #region event handler methods
        // Event handler for the Closing event from NewImageWindow
        private void OnNewImageWindowClosing(object sender, EventArgs e)
        {
            // Handle cleanup or removal of the view model from the collection
            var closingViewModel = sender as NewImageWindowViewModel;
            if (closingViewModel != null)
            {
                MainWindowViewModel.FocusedImage = null;
                OpenImageWindows.Remove(closingViewModel);

                //unsubscribe to events
                closingViewModel.FocusChanged -= HandleFocusChanged!;
                closingViewModel.ImageChanged -= NewImageWindow_ImageChanged!;
                closingViewModel.Closing -= OnNewImageWindowClosing!;
            }
        }

        // Event handler for the ContentChanged event from NewImageWindow
        public void HandleFocusChanged(object sender, string newContent)
        {
            UpdateLabelContent(newContent);
        }

        private void NewImageWindow_ImageChanged(object sender, Mat newImage)
        {
            if (sender is NewImageWindowViewModel newImageViewModel)
            {
                //if (newImageViewModel.histogramWindowViewModel != null)
                //{
                //    newImageViewModel.histogramWindowViewModel.UpdateHistogram(newImage);
                //} // exactly the same at the one below
                newImageViewModel.histogramWindowViewModel?.UpdateHistogram(newImage);
            }
        }
        #endregion

        // Method to update the label content based on the new content
        private void UpdateLabelContent(string newContent)
        {
            LblFocusedImageContent = newContent;
        }

        internal void CloseAllWindows()
        {
            foreach (var windowViewModel in OpenImageWindows)
            {
                windowViewModel.CloseAssociatedNewImageWindow();
            }
        }

        #region Saves
        internal void Save()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Strings.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FocusedImage.SaveChanges();
        }

        private void SaveAs_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Strings.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SaveFileDialog saveFileDialog = new SaveFileDialog();
            saveFileDialog.Filter = "Bitmap Image|*.bmp|JPEG Image|*.jpg|PNG Image|*.png";
            saveFileDialog.Title = "Save Image As...";
            saveFileDialog.FileName = FocusedImage.FileName; // Set default file name here if needed

            if (saveFileDialog.ShowDialog() == true)
            {
                // Determine the file format based on the selected filter index
                string fileName = saveFileDialog.FileName;
                string fileExtension = System.IO.Path.GetExtension(fileName).ToLower();

                switch (fileExtension)
                {
                    case ".bmp":
                        FocusedImage.MatImage.Save(fileName);
                        break;
                    case ".jpg":
                        // For JPEG format, save the MatImage with JPEG compression quality
                        CvInvoke.Imwrite(fileName, FocusedImage.MatImage, new[] { new KeyValuePair<ImwriteFlags, int>(ImwriteFlags.JpegQuality, 90) });
                        break;
                    case ".png":
                        // For PNG format, save the MatImage with PNG compression level
                        CvInvoke.Imwrite(fileName, FocusedImage.MatImage, new[] { new KeyValuePair<ImwriteFlags, int>(ImwriteFlags.PngCompression, 3) });
                        break;
                    default:
                        MessageBox.Show("Invalid file format selected.", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                        return;
                }
            }
        }

        private void SaveAll_Click()
        {
            if (OpenImageWindows == null || OpenImageWindows.Count == 0)
            {
                MessageBox.Show(Strings.NoImageOpen, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            foreach (var window in OpenImageWindows)
            {
                window.SaveChanges();
            }
        }
        #endregion

        private void Negate_Click()
        {
            Func<bool> condition = () => FocusedImage == null ? false : true;
            string? errorMessage = ValidationManager.ValidateCondition(Strings.NoImageFocused, condition);
            if (errorMessage != null)
            {
                MessageBox.Show(errorMessage, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FocusedImage.Negate();

            //OnGrayizationSetMainWindowLabels();
        }

        /// <summary>
        /// MenuItem event that coverts an RGB image to Grayscale
        /// </summary>
        private void Grayize_Click()
        {
            var validationManager = new ValidationManager();
            validationManager.AddValidator(new ImageFocusedValidator(FocusedImage));
            validationManager.AddValidator(new ImageNotGrayScaleValidator(FocusedImage?.MatImage.NumberOfChannels));

            foreach (var errorMessage in validationManager.ValidateAll())
            {
                MessageBox.Show(errorMessage, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FocusedImage.ConvertToRgb(ColorSpaceType.Grayscale);
            OnConvertionUpdateFocusedTitle(ColorSpaceType.Grayscale);
        }

        private void OnConvertionUpdateFocusedTitle(ColorSpaceType colorSpace)
        {
            LblFocusedImageContent = $"({colorSpace}) {FocusedImage.FileName}";
        }

        private void Exit_Click()
        {
            Environment.Exit(Environment.ExitCode);
            //BETTER FOR FUTURE App.Current.Shutdown();
        }

        private void ShowHistogram_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Strings.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //else if (FocusedImage.matImage.NumberOfChannels != Constants.Grayscale_ChannelCount)
            //{
            //    MessageBox.Show(Strings.ImageNotGrayscale, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}
            else if (FocusedImage.histogramWindowViewModel != null)
            {
                MessageBox.Show(Strings.HistogramAlreadyOpen, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            FocusedImage.MakeHistogram();
        }

        private void ConvertRgbToHsv_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Strings.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.MatImage.NumberOfChannels != Constants.XYZ_ChannelCount)
            {
                MessageBox.Show(Strings.ImageNotColor, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            FocusedImage.ConvertToRgb(ColorSpaceType.HSV);
            OnConvertionUpdateFocusedTitle(ColorSpaceType.HSV);
            DisplaySplitChannels(ColorSpaceType.HSV);
        }

        private void SplitChannels_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Strings.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.MatImage.NumberOfChannels != Constants.XYZ_ChannelCount)
            {
                MessageBox.Show(Strings.ImageNotColor, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            DisplaySplitChannels(FocusedImage.ColorSpaceType);
        }

        private void DisplaySplitChannels(ColorSpaceType colorSpace)
        {
            VectorOfMat channels = ImageProcessingCore.SplitChannels(FocusedImage.MatImage);
            string originalName = FocusedImage.FileName;

            for (int i = 0; i < channels.Size; ++i)
            {
                Mat channel = channels[i];

                if (channel != null)
                {
                    string channelName = ChooseChannelName(colorSpace, i);
                    string title = $"{channelName} - {originalName}";
                    DisplayImage(channel, title);
                }
            }
        }

        private static string ChooseChannelName(ColorSpaceType colorSpace, int channelNumber)
        {
            string channelName = colorSpace switch
            {
                ColorSpaceType.HSV => channelNumber switch
                {
                    0 => "Hue (H)",
                    1 => "Saturation (S)",
                    2 => "Value (V)",
                    _ => throw new InvalidOperationException()
                },
                ColorSpaceType.RGB => channelNumber switch
                {
                    0 => "Blue (B)",
                    1 => "Green (G)",
                    2 => "Red (R)",
                    _ => throw new InvalidOperationException()
                },
                ColorSpaceType.LAB => channelNumber switch
                {
                    0 => "Lightness (L)",
                    1 => "Green-Red (A)",
                    2 => "Blue-Yellow (B)",
                    _ => throw new InvalidOperationException()
                },
                _ => throw new NotImplementedException()
            };
            return channelName;
        }

        private void ConvertRgbToLab_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Strings.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.MatImage.NumberOfChannels != Constants.XYZ_ChannelCount)
            {
                MessageBox.Show(Strings.ImageNotColor, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FocusedImage.ConvertToRgb(ColorSpaceType.LAB);
            OnConvertionUpdateFocusedTitle(ColorSpaceType.LAB);
            DisplaySplitChannels(ColorSpaceType.LAB);
        }

        private void StretchHistogram_Click()
        {
            var validationManager = new ValidationManager();
            validationManager.AddValidator(new ImageFocusedValidator(FocusedImage));
            validationManager.AddValidator(new HistogramOpenValidator(FocusedImage?.histogramWindowViewModel));
            validationManager.AddValidator(new ImageGrayScaleValidator(FocusedImage?.MatImage.NumberOfChannels));

            foreach (var errorMessage in validationManager.ValidateAll())
            {
                MessageBox.Show(errorMessage, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FocusedImage.StretchHistogram();
        }

        private void StretchContrast_Click()
        {
            var validationManager = new ValidationManager();
            validationManager.AddValidator(new ImageFocusedValidator(FocusedImage));
            validationManager.AddValidator(new HistogramOpenValidator(FocusedImage?.histogramWindowViewModel));
            validationManager.AddValidator(new ImageGrayScaleValidator(FocusedImage?.MatImage.NumberOfChannels));

            foreach (var errorMessage in validationManager.ValidateAll())
            {
                MessageBox.Show(errorMessage, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            StretchContrastWindowViewModel stretchContrastWindowViewModel = new StretchContrastWindowViewModel();

            StretchContrastWindow stretchContrastWindow = new StretchContrastWindow();
            stretchContrastWindow.DataContext = stretchContrastWindowViewModel;

            stretchContrastWindow.Show();
            stretchContrastWindowViewModel.ValuesSelected += (s, args) =>
            {
                FocusedImage.StretchContrast(stretchContrastWindowViewModel.P1, stretchContrastWindowViewModel.P2, stretchContrastWindowViewModel.Q3, stretchContrastWindowViewModel.Q4);
            };
        }

        private void EqualizeHistogram_Click()
        {
            var validationManager = new ValidationManager();
            validationManager.AddValidator(new ImageFocusedValidator(FocusedImage));
            validationManager.AddValidator(new ImageGrayScaleValidator(FocusedImage?.MatImage.NumberOfChannels));
            validationManager.AddValidator(new HistogramOpenValidator(FocusedImage?.histogramWindowViewModel));

            foreach (var errorMessage in validationManager.ValidateAll())
            {
                MessageBox.Show(errorMessage, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FocusedImage.EqualizeHistogram();
        }

        private void Posterize_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Strings.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.MatImage.NumberOfChannels != Constants.Grayscale_ChannelCount)
            {
                MessageBox.Show(Strings.ImageNotGrayscale, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PosterizeWindowViewModel posterizeWindowViewModel = new PosterizeWindowViewModel();
            PosterizeWindow posterizeWindow = new PosterizeWindow();

            posterizeWindow.DataContext = posterizeWindowViewModel;

            posterizeWindow.Show();
            posterizeWindowViewModel.ValueSelected += (s, e) =>
            {
                FocusedImage.Posterize(posterizeWindowViewModel.LevelsNumber);
            };
        }

        private void Convolve_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Strings.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            ConvolverWindowViewModel convolverWindowViewModel = new ConvolverWindowViewModel();
            ConvolverWindow convolverWindow = new ConvolverWindow();
            convolverWindow.DataContext = convolverWindowViewModel;

            convolverWindow.Show();

            convolverWindowViewModel.KernelAppliable += (s, e) =>
            {
                FocusedImage.Convolve(convolverWindowViewModel.CurrentKernel, e, convolverWindowViewModel.TextBoxValues);
                convolverWindow.Close();
            };
        }

        private void Median_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Strings.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            MedianWindowViewModel medianWindowViewModel = new MedianWindowViewModel();
            MedianWindow medianWindow = new MedianWindow();
            medianWindow.DataContext = medianWindowViewModel;

            medianWindow.ShowDialog();

            if (medianWindow.DialogResult == true)
            {
                FocusedImage.MedianBlur(medianWindowViewModel.MatrixSize, medianWindowViewModel.BorderPixelsOption);
                medianWindow.Close();
            }
        }

        private void ImageCalculator_Click()
        {
            if (OpenImageWindows == null || OpenImageWindows.Count == 0)
            {
                MessageBox.Show(Strings.NoImageOpen, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<ImageInfo> imageInfoList = OpenImageWindows
                .Select(vm => new ImageInfo(vm.MatImage, vm.FileName))
                .ToList();

            ImageCalculatorWindowViewModel imageCalculatorWindowViewModel = new ImageCalculatorWindowViewModel(imageInfoList);
            ImageCalculatorWindow imageCalculatorWindow = new ImageCalculatorWindow();
            imageCalculatorWindow.DataContext = imageCalculatorWindowViewModel;

            imageCalculatorWindowViewModel.ParametersSelected += (s, imageCalculatorInfo) =>
            {
                Mat img1 = imageCalculatorInfo.Image1, img2 = imageCalculatorInfo.Image2;

                Mat image = imageCalculatorInfo.OperationData.Operation switch
                {
                    OperationType.ADD => ImageProcessingCore.Add(img1, img2),
                    OperationType.SUB => ImageProcessingCore.Subtract(img1, img2),
                    OperationType.BLEND => CallBlend(img1, img2, imageCalculatorInfo.OperationData),
                    OperationType.AND => ImageProcessingCore.BitwiseAnd(img1, img2),
                    OperationType.OR => ImageProcessingCore.BitwiseOr(img1, img2),
                    OperationType.NOT => ImageProcessingCore.BitwiseNot(img1),
                    OperationType.XOR => ImageProcessingCore.BitwiseXor(img1, img2),
                };
                if (imageCalculatorInfo.ShouldCreateNewWindow)
                {
                    DisplayImage(image, "Image Calculator Result Window");
                }
                else
                {
                    FocusedImage.MatImage = image;
                }
            };

            //ShowDialog() shows the dialog modally, meaning it blocks until the dialog is closed
            imageCalculatorWindow.ShowDialog();
        }

        private Mat CallBlend(Mat img1, Mat img2, OperationData operationData)
        {
            return ImageProcessingCore.Blend(img1, img2, (double)operationData.BlendFactor1);
        }

        private void DoubleConvolve_Click()
        {
            DoubleConvolverWindowViewModel doubleConvolverWindowViewModel = new DoubleConvolverWindowViewModel();
            DoubleConvolverWindow doubleConvolverWindow = new DoubleConvolverWindow();
            doubleConvolverWindow.DataContext = doubleConvolverWindowViewModel;

            doubleConvolverWindow.ShowDialog();

            if (doubleConvolverWindow.DialogResult == true)
            {
                Mat image = FocusedImage.MatImage.Clone();
                string fileName = FocusedImage.FileName;
                FocusedImage.DoubleConvolve5x5(doubleConvolverWindowViewModel.ResultMatrix, BorderType.Isolated);

                DisplayImage(image, fileName);
                FocusedImage.DoubleConvolve3x3(doubleConvolverWindowViewModel.FirstMatrix, doubleConvolverWindowViewModel.SecondMatrix, BorderType.Isolated);
            }
        }

        private void Erode_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Strings.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            StandardMorphologicalWindowViewModel standardMorphologicalWindowViewModel = new StandardMorphologicalWindowViewModel();
            StandardMorphologicalWindow standardMorphologicalWindow = new StandardMorphologicalWindow();
            standardMorphologicalWindow.DataContext = standardMorphologicalWindowViewModel;

            standardMorphologicalWindow.ShowDialog();

            if (standardMorphologicalWindow.DialogResult == true)
            {
                MorphologicalOperationType operation = MorphologicalOperationType.Erode;
                ShapeType shape = standardMorphologicalWindowViewModel.Shape;
                BorderType borderType = standardMorphologicalWindowViewModel.BorderPixelsOption;
                int? elementSize = standardMorphologicalWindowViewModel.ElementSize;
                BasicMorphologicalInfo info = new BasicMorphologicalInfo(operation, shape, borderType, elementSize);

                FocusedImage.PerformMorpologicalOperation(info, new MCvScalar());
                standardMorphologicalWindow.Close();
            }
        }

        private void Dilate_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Strings.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            StandardMorphologicalWindowViewModel standardMorphologicalWindowViewModel = new StandardMorphologicalWindowViewModel();
            StandardMorphologicalWindow standardMorphologicalWindow = new StandardMorphologicalWindow();
            standardMorphologicalWindow.DataContext = standardMorphologicalWindowViewModel;

            standardMorphologicalWindow.ShowDialog();

            if (standardMorphologicalWindow.DialogResult == true)
            {
                MorphologicalOperationType operation = MorphologicalOperationType.Dilate;
                ShapeType shape = standardMorphologicalWindowViewModel.Shape;
                BorderType borderType = standardMorphologicalWindowViewModel.BorderPixelsOption;
                int? elementSize = standardMorphologicalWindowViewModel.ElementSize;
                BasicMorphologicalInfo info = new BasicMorphologicalInfo(operation, shape, borderType, elementSize);

                FocusedImage.PerformMorpologicalOperation(info, new MCvScalar());
                standardMorphologicalWindow.Close();
            }
        }

        private void MorphologicalOpen_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Strings.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            StandardMorphologicalWindowViewModel standardMorphologicalWindowViewModel = new StandardMorphologicalWindowViewModel();
            StandardMorphologicalWindow standardMorphologicalWindow = new StandardMorphologicalWindow();
            standardMorphologicalWindow.DataContext = standardMorphologicalWindowViewModel;

            standardMorphologicalWindow.ShowDialog();

            if (standardMorphologicalWindow.DialogResult == true)
            {
                MorphologicalOperationType operation = MorphologicalOperationType.MorphOpening;
                ShapeType shape = standardMorphologicalWindowViewModel.Shape;
                BorderType borderType = standardMorphologicalWindowViewModel.BorderPixelsOption;
                int? elementSize = standardMorphologicalWindowViewModel.ElementSize;
                BasicMorphologicalInfo info = new BasicMorphologicalInfo(operation, shape, borderType, elementSize);

                FocusedImage.PerformMorpologicalOperation(info, new MCvScalar());
                standardMorphologicalWindow.Close();
            }
        }

        private void MorphologicalClose_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Strings.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            StandardMorphologicalWindowViewModel standardMorphologicalWindowViewModel = new StandardMorphologicalWindowViewModel();
            StandardMorphologicalWindow standardMorphologicalWindow = new StandardMorphologicalWindow();
            standardMorphologicalWindow.DataContext = standardMorphologicalWindowViewModel;

            standardMorphologicalWindow.ShowDialog();

            if (standardMorphologicalWindow.DialogResult == true)
            {
                MorphologicalOperationType operation = MorphologicalOperationType.MorphClosing;
                ShapeType shape = standardMorphologicalWindowViewModel.Shape;
                BorderType borderType = standardMorphologicalWindowViewModel.BorderPixelsOption;
                int? elementSize = standardMorphologicalWindowViewModel.ElementSize;
                BasicMorphologicalInfo info = new BasicMorphologicalInfo(operation, shape, borderType, elementSize);

                FocusedImage.PerformMorpologicalOperation(info, new MCvScalar());
                standardMorphologicalWindow.Close();
            }
        }

        private void Threshold_Click()
        {
            Mat image = FocusedImage.MatImage.Clone();
            ThresholderWindowViewModel thresholderWindowViewModel = new ThresholderWindowViewModel(image);
            ThresholderWindow thresholderWindow = new ThresholderWindow();
            thresholderWindow.DataContext = thresholderWindowViewModel;

            thresholderWindow.Show();
            //ImageTestWindow imageTestWindow = new ImageTestWindow();
            //imageTestWindow.ShowDialog();
        }

        private void SimpleAnalyze_Click()
        {
            SummaryWindowViewModel summaryWindowViewModel = new SummaryWindowViewModel();
            SummaryWindow summaryWindow = new SummaryWindow();
            summaryWindow.DataContext = summaryWindowViewModel;

            summaryWindow.Show();
        }

        private void Analyze_Click()
        {
            AnalyzeParticlesWindowViewModel analyzeParticlesWindowViewModel = new AnalyzeParticlesWindowViewModel();
            AnalyzeParticlesWindow analyzeParticlesWindow = new AnalyzeParticlesWindow();
            analyzeParticlesWindow.DataContext = analyzeParticlesWindowViewModel;

            analyzeParticlesWindow.ShowDialog();

            if (analyzeParticlesWindow.DialogResult == true)
            {
                analyzeParticlesWindow.Close();
                SummaryWindowViewModel summaryWindowViewModel = new SummaryWindowViewModel(analyzeParticlesWindowViewModel.AnalysisSettings);
                SummaryWindow summaryWindow = new SummaryWindow();
                summaryWindow.DataContext = summaryWindowViewModel;

                summaryWindow.Show();
            }
        }

        private void Skeletonize_Click()
        {
            FocusedImage.Skeletonize();
        }

        private void Hough_Click()
        {
            FocusedImage.Hough();
        }

        private void PlotProfile_Click()
        {
            if (points[0] == null || points[1] == null)
            {
                MessageBox.Show("Select 2 points in image", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PlotlineGraphWindowViewModel plotlineGraphWindowViewModel = new PlotlineGraphWindowViewModel(points.Where(p => p != null).Select(p => p.Value).ToArray(), FocusedImage.MatImage);
            PlotlineGraphWindow plotlineGraphWindow = new PlotlineGraphWindow();
            plotlineGraphWindow.DataContext = plotlineGraphWindowViewModel;

            plotlineGraphWindow.Show();
        }
    }
}
