using JSharp.Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace JSharp.Utility
{
    public class KernelMappings
    {
        // Dictionary mapping kernel names to their corresponding arrays
        public static Dictionary<string, int[,]> KernelNameToArray { get; } = new Dictionary<string, int[,]>
        {
            {Kernels.BoxBlur, new int[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } }},
            {Kernels.GaussianBlur, new int[,] { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } }},
            {Kernels.SobelNS, new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } }},
            {Kernels.SobelEW, new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } }},
            {Kernels.Laplacian, new int[,] { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } }},
            {Kernels.SharpenMask1, new int[,] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } }},
            {Kernels.SharpenMask2, new int[,] { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } }},
            {Kernels.SharpenMask3, new int[,] { { 1, -2, 1 }, { -2, 5, -2 }, { 1, -2, 1 } }},
            {Kernels.PrewittN, new int[,] { { -1, -1, -1 }, { 0, 0, 0 }, { 1, 1, 1 } }},
            {Kernels.PrewittNE, new int[,] { { 0, -1, -1 }, { 1, 0, -1 }, { 1, 1, 0 } }},
            {Kernels.PrewittE, new int[,] { { 1, 0, -1 }, { 1, 0, -1 }, { 1, 0, -1 } }},
            {Kernels.PrewittSE, new int[,] { { 1, 1, 0 }, { 1, 0, -1 }, { 0, -1, -1 } }},
            {Kernels.PrewittS, new int[,] { { 1, 1, 1 }, { 0, 0, 0 }, { -1, -1, -1 } }},
            {Kernels.PrewittSW, new int[,] { { 0, 1, 1 }, { -1, 0, 1 }, { -1, -1, 0 } }},
            {Kernels.PrewittW, new int[,] { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } }},
            {Kernels.PrewittNW, new int[,] { { -1, -1, 0 }, { -1, 0, 1 }, { 0, 1, 1 } }},
            {Kernels.Identity, new int[,] { { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } }},
        };

        // Dictionary mapping kernel arrays to their corresponding names
        public static Dictionary<int[,], string> ArrayToKernelName { get; } = KernelNameToArray.ToDictionary(kvp => kvp.Value, kvp => kvp.Key);
    }
}
