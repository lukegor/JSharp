using Emgu.CV;
using Emgu.CV.CvEnum;
using Emgu.CV.Structure;
using Emgu.CV.Util;
using JSharp.Models;
using JSharp.Resources;
using JSharp.Utility;
using JSharp.Validation.Validators;
using LiveChartsCore.Kernel;
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
using System.Net;
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

        private string _descriptor;
        public string Descriptor
        {
            get { return _descriptor; }
            set { SetProperty(ref _descriptor, value); }
        }

        private System.Windows.Controls.RadioButton _selectedButton;
        public System.Windows.Controls.RadioButton SelectedButton
        {
            get { return _selectedButton; }
            set { SetProperty(ref _selectedButton, value); }
        }

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
        public DelegateCommand PyramidUp_ClickCommand { get; }
        public DelegateCommand PyramidDown_ClickCommand { get; }
        public DelegateCommand SimpleThreshold_ClickCommand { get; }
        public DelegateCommand AdaptiveThresholding_ClickCommand { get; }
        public DelegateCommand Watershed_ClickCommand { get; }
        public DelegateCommand Inpaint_ClickCommand { get; }
        public DelegateCommand GrabCut_ClickCommand { get; }
        public DelegateCommand DetailedAnalyze_ClickCommand { get; }
        public DelegateCommand OpenSettings_ClickCommand { get; }
        public DelegateCommand Rotate_ClickCommand { get; }
        public DelegateCommand Flip_ClickCommand { get; }
        public DelegateCommand CopyToSystem_ClickCommand { get; }
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
            PyramidUp_ClickCommand = new DelegateCommand(PyramidUp_Click);
            PyramidDown_ClickCommand = new DelegateCommand(PyramidDown_Click);
            SimpleThreshold_ClickCommand = new DelegateCommand(SimpleThreshold_Click);
            AdaptiveThresholding_ClickCommand = new DelegateCommand(AdaptiveThresholding_Click);
            Watershed_ClickCommand = new DelegateCommand(Watershed_Click);
            Inpaint_ClickCommand = new DelegateCommand(Inpaint_Click);
            GrabCut_ClickCommand = new DelegateCommand(GrabCut_Click);
            DetailedAnalyze_ClickCommand = new DelegateCommand(DetailedAnalyze_Click);
            OpenSettings_ClickCommand = new DelegateCommand(OpenSettings_Click);
            Rotate_ClickCommand = new DelegateCommand(Rotate_Click);
            Flip_ClickCommand = new DelegateCommand(Flip_Click);
            CopyToSystem_ClickCommand = new DelegateCommand(CopyToSystem_Click);
            #endregion
        }

        internal void UpdateCheckedRadioButton(object sender)
        {
            SelectedButton = sender as RadioButton;
        }

        internal void UpdateDescriptor()
        {
            if (SelectedButton?.Name == Constants.RadioBtnProfileLine)
            {
                StringBuilder sb = new StringBuilder();
                var focusedImage = FocusedImage;

                if (focusedImage != null)
                {
                    sb.Append($"X: {focusedImage.MousePosition.X}, Y: {focusedImage.MousePosition.Y}");

                    System.Windows.Point? point1 = focusedImage.Points[0];
                    if (point1 != null)
                    {
                        sb.Append($", Point 1: ({focusedImage.Points[0]})");

                        System.Windows.Point? point2 = focusedImage.Points[1];
                        if (point2 != null)
                        {
                            sb.Append($", Point 2: ({FocusedImage.Points[1]})");
                            sb.Append($", Length: {Math.Floor(ImageProcessingUtility.GetDistance((Point)point1, (Point)point2))}");
                        }
                    }
                }
                else
                {
                    //notify to update to empty when window closing
                    sb.Append(string.Empty);
                }
                System.Diagnostics.Debug.WriteLine(sb.ToString());
                Descriptor = sb.ToString();
            }
            else if (SelectedButton?.Name == Constants.RadioBtnRectangle)
            {
                StringBuilder sb = new StringBuilder();
                var focusedImage = FocusedImage;

                if (focusedImage != null)
                {
                    sb.Append($"X: {focusedImage.MousePosition.X}, Y: {focusedImage.MousePosition.Y}");

                    System.Windows.Point? startPoint = focusedImage.Points[0];
                    if (startPoint != null)
                    {
                        sb.Append($", Top-left Point 1: ({focusedImage.Points[0]})");

                        System.Windows.Point? endPoint = focusedImage.Points[1];
                        if (endPoint != null)
                        {
                            sb.Append($", Bottom-right Point 2: ({FocusedImage.Points[1]})");

                            // Calculate the width and height of the rectangle
                            double width = Math.Abs(endPoint.Value.X - startPoint.Value.X);
                            double height = Math.Abs(endPoint.Value.Y - startPoint.Value.Y);

                            // Determine the top-left corner coordinates of the rectangle
                            double x = Math.Min(startPoint.Value.X, endPoint.Value.X);
                            double y = Math.Min(startPoint.Value.Y, endPoint.Value.Y);

                            sb.Append(" ").Append($"Rectangle = (x: {x}, y: {y}, width: {width}, height: {height})");
                        }
                    }
                    else
                    {
                        //notify to update to empty when window closing
                        sb.Append(string.Empty);
                    }
                    System.Diagnostics.Debug.WriteLine(sb.ToString());
                    Descriptor = sb.ToString();
                }
            }
            else if (SelectedButton?.Name == Constants.RadioBtnNone)
            {
                Descriptor = string.Empty;
            }
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
            openFileDialog.DefaultExt = Properties.Settings.Default.saveFileExtension;
            openFileDialog.Filter = Constants.ImageFilterString;

            List<Mat> images = new List<Mat>();
            List<string> fullFilePaths = new List<string>();

            if (openFileDialog.ShowDialog() == true)
            {
                foreach (string fileName in openFileDialog.FileNames)
                {
                    Mat imageMat = CvInvoke.Imread(fileName, color);
                    if (!imageMat.IsEmpty && imageMat != null)
                    {
                        images.Add(imageMat);
                        fullFilePaths.Add(fileName);
                    }
                    else
                    {
                        MessageBox.Show(Errors.LoadingFailed);
                    }
                }
            }
            return (images, fullFilePaths);
        }

        internal void DisplayImage(Mat matImage, string fileName)
        {
            BitmapSource source = matImage.MatToBitmapSource();

            int duplicateCount = OpenImageWindows.Count(x => x.CoreName == Path.GetFileName(fileName));

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
                UpdateDescriptor();
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
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FocusedImage.SaveChanges();
        }

        private void SaveAs_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FocusedImage.SaveAs();
        }

        private void SaveAll_Click()
        {
            if (OpenImageWindows == null || OpenImageWindows.Count == 0)
            {
                MessageBox.Show(Errors.NoImageOpen, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
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
            string? errorMessage = ValidationManager.ValidateCondition(Errors.NoImageFocused, condition);
            if (errorMessage != null)
            {
                MessageBox.Show(errorMessage, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FocusedImage.Negate();
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
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.MatImage.NumberOfChannels != Constants.Grayscale_ChannelCount)
            {
                MessageBox.Show(Errors.ImageNotGrayscale, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.histogramWindowViewModel != null)
            {
                MessageBox.Show(Errors.HistogramAlreadyOpen, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            FocusedImage.MakeHistogram();
        }

        private void ConvertRgbToHsv_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.MatImage.NumberOfChannels != Constants.XYZ_ChannelCount)
            {
                MessageBox.Show(Errors.ImageNotColor, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.MatImage.NumberOfChannels != Constants.XYZ_ChannelCount)
            {
                MessageBox.Show(Errors.ImageNotColor, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.MatImage.NumberOfChannels != Constants.XYZ_ChannelCount)
            {
                MessageBox.Show(Errors.ImageNotColor, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.MatImage.NumberOfChannels != Constants.Grayscale_ChannelCount)
            {
                MessageBox.Show(Errors.ImageNotGrayscale, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.MatImage.NumberOfChannels != Constants.Grayscale_ChannelCount)
            {
                MessageBox.Show(Errors.ImageNotGrayscale, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.MatImage.NumberOfChannels != Constants.Grayscale_ChannelCount)
            {
                MessageBox.Show(Errors.ImageNotGrayscale, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(Errors.NoImageOpen, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
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

                if (img1.NumberOfChannels != Constants.Grayscale_ChannelCount || img2.NumberOfChannels != Constants.Grayscale_ChannelCount)
                {
                    MessageBox.Show(Errors.ImageNotGrayscale, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    return;
                }

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
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.MatImage.NumberOfChannels != Constants.Grayscale_ChannelCount)
            {
                MessageBox.Show(Errors.ImageNotGrayscale, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
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
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
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
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.MatImage.NumberOfChannels != Constants.Grayscale_ChannelCount)
            {
                MessageBox.Show(Errors.ImageNotGrayscale, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Mat image = FocusedImage.MatImage.Clone();
            ThresholderWindowViewModel thresholderWindowViewModel = new ThresholderWindowViewModel(image, FocusedImage);
            ThresholderWindow thresholderWindow = new ThresholderWindow();
            thresholderWindow.DataContext = thresholderWindowViewModel;

            thresholderWindow.Show();
        }

        private void SimpleAnalyze_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            SummaryWindowViewModel summaryWindowViewModel = new SummaryWindowViewModel();
            SummaryWindow summaryWindow = new SummaryWindow();
            summaryWindow.DataContext = summaryWindowViewModel;

            summaryWindow.Show();
        }

        private void Analyze_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

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
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            (int[] data, _) = ImageProcessingCore.CalculateHistogramValues(FocusedImage.MatImage);
            if (!ImageProcessingUtility.IsBinaryImage(data))
            {
                MessageBox.Show("Obraz poddany szkieletyzacji musi być obrazem binarnym. Sposteryzuj obraz do 2 poziomów szarości lub przeprowadź progowanie.", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FocusedImage.Skeletonize();
        }

        private void Hough_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FocusedImage.Hough();
        }

        private void PlotProfile_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var points = FocusedImage.Points.Select(p => p != null && p.HasValue ? p.Value : (System.Windows.Point?)null).ToArray();

            if (points[0] == null || points[1] == null)
            {
                MessageBox.Show("Select 2 points in image", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.MatImage.NumberOfChannels != Constants.Grayscale_ChannelCount)
            {
                MessageBox.Show(Errors.ImageNotGrayscale, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Point[] validPoints = points.Cast<System.Windows.Point>().ToArray();

            PlotlineGraphWindowViewModel plotlineGraphWindowViewModel = new PlotlineGraphWindowViewModel(validPoints, FocusedImage.MatImage);
            PlotlineGraphWindow plotlineGraphWindow = new PlotlineGraphWindow();
            plotlineGraphWindow.DataContext = plotlineGraphWindowViewModel;

            plotlineGraphWindow.Show();
        }

        public void PyramidUp_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PyramidWindowViewModel pyramidWindowViewModel = new PyramidWindowViewModel();
            PyramidWindow pyramidWindow = new PyramidWindow();
            pyramidWindow.DataContext = pyramidWindowViewModel;

            if (pyramidWindow.ShowDialog() == true)
            {
                int reps = pyramidWindowViewModel.EffectSize switch
                {
                    2 => 1,
                    4 => 2
                };
                for (int i = 0; i < reps; ++i)
                {
                    FocusedImage.PyramidUp();
                }
            }
        }

        private void PyramidDown_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            PyramidWindowViewModel pyramidWindowViewModel = new PyramidWindowViewModel();
            PyramidWindow pyramidWindow = new PyramidWindow();
            pyramidWindow.DataContext = pyramidWindowViewModel;

            if (pyramidWindow.ShowDialog() == true)
            {
                int reps = pyramidWindowViewModel.EffectSize switch
                {
                    2 => 1,
                    4 => 2
                };
                for (int i = 0; i < reps; ++i)
                {
                    FocusedImage.PyramidDown();
                }
            }
        }

        private void Rotate_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FocusedImage.Rotate(RotateFlags.Rotate90Clockwise);
        }

        private void Flip_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FocusedImage.Rotate(RotateFlags.Rotate180);
        }

        private void SimpleThreshold_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.MatImage.NumberOfChannels != Constants.Grayscale_ChannelCount)
            {
                MessageBox.Show(Errors.ImageNotGrayscale, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            Mat image = FocusedImage.MatImage.Clone();
            SimpleThresholderWindowViewModel thresholderWindowViewModel = new SimpleThresholderWindowViewModel(image, FocusedImage);
            SimpleThresholderWindow thresholderWindow = new SimpleThresholderWindow();
            thresholderWindow.DataContext = thresholderWindowViewModel;

            thresholderWindow.Show();
        }

        private void AdaptiveThresholding_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            else if (FocusedImage.MatImage.NumberOfChannels != Constants.Grayscale_ChannelCount)
            {
                MessageBox.Show(Errors.ImageNotGrayscale, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FocusedImage.PerformAdaptiveThresholding();
        }

        private void Watershed_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            FocusedImage.Watershed();
        }

        private void Inpaint_Click()
        {
            if (OpenImageWindows == null || OpenImageWindows.Count < 2)
            {
                MessageBox.Show("Open at least 2 images", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            List<ImageInfo> imageInfoList = OpenImageWindows
                .Select(vm => new ImageInfo(vm.MatImage, vm.FileName))
                .ToList();

            InpaintWindowViewModel inpaintWindowViewModel = new InpaintWindowViewModel(imageInfoList);
            InpaintWindow inpaintWindow = new InpaintWindow();
            inpaintWindow.DataContext = inpaintWindowViewModel;

            if (inpaintWindow.ShowDialog() == true)
            {
                Mat image = ImageProcessingCore.Inpainting(inpaintWindowViewModel.SelectedImage1, inpaintWindowViewModel.SelectedImage2);
                DisplayImage(image, "Inpaint Result Window");
            }
        }

        private void GrabCut_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            var points = FocusedImage.Points.Select(p => p.Value).ToArray();
            if (points[0] == null || points[1] == null)
            {
                MessageBox.Show("Select 2 points in image", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }
            //else if (FocusedImage.MatImage.NumberOfChannels != Constants.Grayscale_ChannelCount)
            //{
            //    MessageBox.Show(Errors.ImageNotGrayscale, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
            //    return;
            //}

            Point startPoint = (Point)FocusedImage.Points[0];
            Point endPoint = (Point)FocusedImage.Points[1];

            // Calculate the width and height of the rectangle
            int width = (int)Math.Abs(endPoint.X - startPoint.X);
            int height = (int)Math.Abs(endPoint.Y - startPoint.Y);

            // Determine the top-left corner coordinates of the rectangle
            int x = (int)Math.Min(startPoint.X, endPoint.X);
            int y = (int)Math.Min(startPoint.Y, endPoint.Y);

            Mat image = ImageProcessingCore.GrabCut(FocusedImage.MatImage, new System.Drawing.Rectangle(x, y, width, height));
            FocusedImage.UpdateImageSource(image);
        }

        private void OpenSettings_Click()
        {
            SettingsWindow settingsWindow = new SettingsWindow();
            SettingsWindowViewModel settingsWindowViewModel = new SettingsWindowViewModel();
            settingsWindow.DataContext = settingsWindowViewModel;
            settingsWindow.ShowDialog();

            if (settingsWindow.DialogResult == true)
            {

            }
        }

        private void CopyToSystem_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            BitmapSource bitmapSource = FocusedImage.MatImage.MatToBitmapSource();
            Clipboard.SetImage(bitmapSource);
        }

        private void DetailedAnalyze_Click()
        {
            if (FocusedImage == null)
            {
                MessageBox.Show(Errors.NoImageFocused, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return;
            }

            var data = ImageProcessingCore.AnalyseImage(FocusedImage.MatImage, RetrType.List, ChainApproxMethod.ChainApproxNone);
            double[,] moments; double[] area; double[] perimeter; double[] aspectRatio; double[] extent;

            //(moments, area, perimeter, aspectRatio, extent) = ImageProcessingCore.AnalyzeContours(data);
        }
    }
}
