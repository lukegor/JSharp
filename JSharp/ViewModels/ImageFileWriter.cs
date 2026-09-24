using JSharp.Services;
using JSharp.Shared.Resources;
using OpenCvSharp;
using System.IO;
using System.Windows;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Writes a Mat to disk in the format implied by the file extension.
    /// </summary>
    internal static class ImageFileWriter
    {
        public static void Write(Mat image, string filePath)
        {
            bool ok;
            switch (Path.GetExtension(filePath).ToLowerInvariant())
            {
                case ".bmp":
                    ok = Cv2.ImWrite(filePath, image);
                    break;
                case ".jpg":
                case ".jpeg":
                    ok = Cv2.ImWrite(filePath, image,
                        [new ImageEncodingParam(ImwriteFlags.JpegQuality, Properties.Settings.Default.jpqSaveQuality)]);
                    break;
                case ".png":
                    ok = Cv2.ImWrite(filePath, image,
                        [new ImageEncodingParam(ImwriteFlags.PngCompression, Properties.Settings.Default.pngCompressionLevel)]);
                    break;
                case ".gif":
                    ok = Cv2.ImWrite(filePath, image);
                    break;
                case ".tiff":
                    ok = Cv2.ImWrite(filePath, image,
                        [new ImageEncodingParam(ImwriteFlags.TiffCompression, (int)ImwriteFlags.TiffCompression)]);
                    break;
                default:
                    throw new InvalidOperationException($"Unsupported image extension: '{Path.GetExtension(filePath)}'.");
            }

            if (!ok)
            {
                throw new InvalidOperationException($"Cannot write image: '{filePath}'.");
            }
        }

        public static bool TryWrite(Mat image, string filePath, IMessageService messages)
        {
            try
            {
                Write(image, filePath);
                return true;
            }
            catch (InvalidOperationException)
            {
                messages.ShowMessage(
                    "Invalid file format selected.", Strings.Error, MessageBoxButton.OK, MessageBoxImage.Error);
                return false;
            }
        }
    }
}
