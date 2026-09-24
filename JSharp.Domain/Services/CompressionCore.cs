namespace JSharp.Domain.Services
{
    public static class CompressionCore
    {
        public static List<byte> CompressRle(byte[] imageData)
        {
            List<byte> compressed = new List<byte>();
            int length = imageData.Length;
            int runLength;

            for (int i = 0; i < length; i += runLength)
            {
                byte currentByte = imageData[i];
                runLength = 1;

                // Calculate the run length for the current byte
                while (i + runLength < length && imageData[i + runLength] == currentByte && runLength < 255)
                {
                    runLength++;
                }

                // Add the byte and its run length to the compressed list
                compressed.Add(currentByte);
                compressed.Add((byte)runLength);
            }

            return compressed;
        }

        public static byte[] DecompressRle(List<byte> compressedData)
        {
            ArgumentNullException.ThrowIfNull(compressedData);

            if (compressedData.Count % 2 != 0)
            {
                throw new ArgumentException("Compressed data must contain value/count pairs.", nameof(compressedData));
            }

            List<byte> decompressed = new List<byte>();

            for (int i = 0; i < compressedData.Count; i += 2)
            {
                byte value = compressedData[i];
                int runLength = compressedData[i + 1];

                for (int j = 0; j < runLength; j++)
                {
                    decompressed.Add(value);
                }
            }

            return decompressed.ToArray();
        }

        public static double CalculateCompressionRatio(int compressedSize, int originalSize)
        {
            return (double)compressedSize / originalSize;
        }

        public static byte[] ConvertArrayToByteArray(Array data)
        {
            ArgumentNullException.ThrowIfNull(data);

            int byteLength = Buffer.ByteLength(data);
            byte[] byteArray = new byte[byteLength];
            Buffer.BlockCopy(data, 0, byteArray, 0, byteLength);
            return byteArray;
        }
    }
}
