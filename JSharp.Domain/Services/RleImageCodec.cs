using System.IO;
using System.Runtime.InteropServices;
using System.Text;
using OpenCvSharp;

namespace JSharp.Domain.Services
{
    /// <summary>
    /// Saves and restores images using the versioned JSRLE container format:
    /// magic "JSRLE", version byte, int32 LE width, int32 LE height, channel count,
    /// followed by an RLE value/count payload produced by CompressionCore.
    /// </summary>
    public class RleImageCodec
    {
        private const string Magic = "JSRLE";
        private const byte Version = 1;
        private const int VersionOffset = 5;
        private const int WidthOffset = 6;
        private const int HeightOffset = 10;
        private const int ChannelsOffset = 14;
        private const int HeaderLength = 15;
        private const int MaxChannels = 4;

        public void Save(Mat image, string path)
        {
            ArgumentNullException.ThrowIfNull(image);
            ArgumentException.ThrowIfNullOrWhiteSpace(path);
            if (image.Empty())
            {
                throw new ArgumentException("Cannot save an empty image.", nameof(image));
            }

            int totalBytes = image.Rows * image.Cols * image.Channels();
            byte[] pixelData = new byte[totalBytes];
            Marshal.Copy(image.Data, pixelData, 0, totalBytes);
            List<byte> payload = CompressionCore.CompressRle(pixelData);

            using MemoryStream stream = new MemoryStream();
            using BinaryWriter writer = new BinaryWriter(stream);
            writer.Write(Magic.ToCharArray());
            writer.Write(Version);
            writer.Write(image.Cols);
            writer.Write(image.Rows);
            writer.Write((byte)image.Channels());
            writer.Write(payload.ToArray());

            File.WriteAllBytes(path, stream.ToArray());
        }

        public Mat Load(string path)
        {
            ArgumentException.ThrowIfNullOrWhiteSpace(path);

            byte[] fileBytes = File.ReadAllBytes(path);
            ValidateHeader(fileBytes);

            int width = BitConverter.ToInt32(fileBytes, WidthOffset);
            int height = BitConverter.ToInt32(fileBytes, HeightOffset);
            int channels = fileBytes[ChannelsOffset];
            if (width <= 0 || height <= 0 || channels is < 1 or > MaxChannels)
            {
                throw new InvalidDataException("Invalid image dimensions in RLE header.");
            }

            List<byte> payload = fileBytes.Skip(HeaderLength).ToList();
            if (payload.Count % 2 != 0)
            {
                throw new InvalidDataException("Truncated RLE payload.");
            }

            byte[] decompressed = CompressionCore.DecompressRle(payload);
            int expectedBytes = width * height * channels;
            if (decompressed.Length != expectedBytes)
            {
                throw new InvalidDataException(
                    $"Decompressed size mismatch: expected {expectedBytes} bytes, got {decompressed.Length}.");
            }

            Mat result = new Mat(height, width, MatType.CV_8UC(channels));
            Marshal.Copy(decompressed.ToArray(), 0, result.Data, decompressed.Length);
            return result;
        }

        private static void ValidateHeader(byte[] fileBytes)
        {
            if (fileBytes.Length < HeaderLength)
            {
                throw new InvalidDataException("File too short to be a JSharp RLE image.");
            }

            if (Encoding.ASCII.GetString(fileBytes, 0, Magic.Length) != Magic)
            {
                throw new InvalidDataException("Not a JSharp RLE image (bad magic).");
            }

            if (fileBytes[VersionOffset] != Version)
            {
                throw new InvalidDataException($"Unsupported RLE version {fileBytes[VersionOffset]}.");
            }
        }
    }
}
