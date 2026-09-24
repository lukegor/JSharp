using JSharp.Services;
using JSharp.Shared.Resources;
using JSharp.Utility.Utility;
using Microsoft.Win32;
using OpenCvSharp;
using System.Windows;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Open-file dialog + Imread loop for loading images from disk.
    /// </summary>
    internal static class ImageFileLoader
    {
        public static (List<Mat> Images, List<string> FilePaths) Load(ImreadModes mode, IMessageService messages)
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
                    Mat imageMat = Cv2.ImRead(fileName, mode);
                    if (imageMat is not null && !imageMat.Empty())
                    {
                        images.Add(imageMat);
                        fullFilePaths.Add(fileName);
                    }
                    else
                    {
                        messages.ShowMessage(Errors.LoadingFailed, Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                    }
                }
            }

            return (images, fullFilePaths);
        }

        public static (List<Mat> Images, List<string> FilePaths) LoadColor(IMessageService messages)
            => Load(ImreadModes.Color, messages);

        public static (List<Mat> Images, List<string> FilePaths) LoadGrayscale(IMessageService messages)
            => Load(ImreadModes.Grayscale, messages);
    }
}
