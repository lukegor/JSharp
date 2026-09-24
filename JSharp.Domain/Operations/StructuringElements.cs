using System.Runtime.InteropServices;
using OpenCvSharp;

namespace JSharp.Domain.Operations
{
    public static class StructuringElements
    {
        public static Mat Diamond(int radius)
        {
            Mat diamond = new(new Size(radius * 2 + 1, radius * 2 + 1), MatType.CV_8UC1, Scalar.Black);
            int side = radius * 2 + 1;
            byte[] flat = new byte[side * side];

            for (int i = 0; i <= radius; i++)
            {
                for (int j = radius - i; j <= radius + i; j++)
                {
                    flat[i * side + j] = 255;
                    flat[(side - 1 - i) * side + j] = 255;
                }
            }

            Marshal.Copy(flat, 0, diamond.Data, flat.Length);
            return diamond;
        }
    }
}
