using System.Globalization;
using JSharp.Shared.Resources;

namespace JSharp.Utility.Utility
{
    /// <summary>
    /// Culture-robust kernel lookup. Kernel matrices are culture-free data kept
    /// in a table keyed by resource name; display names are resolved at the call
    /// boundary against the current UI culture (falling back to the invariant
    /// values). A mid-process UI culture change can therefore never break
    /// lookups the way a statically-keyed localized dictionary did.
    /// </summary>
    public static class KernelMappings
    {
        private static readonly int[,] IdentityValue = new int[,] { { 0, 0, 0 }, { 0, 1, 0 }, { 0, 0, 0 } };

        private static readonly (string ResourceKey, int[,] Array)[] KernelsByKey =
        {
            ("BoxBlur", new int[,] { { 1, 1, 1 }, { 1, 1, 1 }, { 1, 1, 1 } }),
            ("GaussianBlur", new int[,] { { 1, 2, 1 }, { 2, 4, 2 }, { 1, 2, 1 } }),
            ("SobelNS", new int[,] { { -1, -2, -1 }, { 0, 0, 0 }, { 1, 2, 1 } }),
            ("SobelEW", new int[,] { { -1, 0, 1 }, { -2, 0, 2 }, { -1, 0, 1 } }),
            ("Laplacian", new int[,] { { 0, 1, 0 }, { 1, -4, 1 }, { 0, 1, 0 } }),
            ("SharpenMask1", new int[,] { { 0, -1, 0 }, { -1, 5, -1 }, { 0, -1, 0 } }),
            ("SharpenMask2", new int[,] { { -1, -1, -1 }, { -1, 9, -1 }, { -1, -1, -1 } }),
            ("SharpenMask3", new int[,] { { 1, -2, 1 }, { -2, 5, -2 }, { 1, -2, 1 } }),
            ("PrewittN", new int[,] { { -1, -1, -1 }, { 0, 0, 0 }, { 1, 1, 1 } }),
            ("PrewittNE", new int[,] { { 0, -1, -1 }, { 1, 0, -1 }, { 1, 1, 0 } }),
            ("PrewittE", new int[,] { { 1, 0, -1 }, { 1, 0, -1 }, { 1, 0, -1 } }),
            ("PrewittSE", new int[,] { { 1, 1, 0 }, { 1, 0, -1 }, { 0, -1, -1 } }),
            ("PrewittS", new int[,] { { 1, 1, 1 }, { 0, 0, 0 }, { -1, -1, -1 } }),
            ("PrewittSW", new int[,] { { 0, 1, 1 }, { -1, 0, 1 }, { -1, -1, 0 } }),
            ("PrewittW", new int[,] { { -1, 0, 1 }, { -1, 0, 1 }, { -1, 0, 1 } }),
            ("PrewittNW", new int[,] { { -1, -1, 0 }, { -1, 0, 1 }, { 0, 1, 1 } }),
            ("Identity", IdentityValue),
        };

        public static int[,] IdentityArray => IdentityValue;

        public static bool TryGetArray(string? displayName, out int[,] array)
        {
            if (displayName is not null)
            {
                foreach ((string resourceKey, int[,] kernel) in KernelsByKey)
                {
                    if (displayName == Display(resourceKey) || displayName == Invariant(resourceKey))
                    {
                        array = kernel;
                        return true;
                    }
                }
            }

            array = null!;
            return false;
        }

        public static string GetDisplayName(int[,] array)
        {
            foreach ((string resourceKey, int[,] kernel) in KernelsByKey)
            {
                if (ArrayHelper.AreArraysEqual(array, kernel))
                {
                    return Display(resourceKey);
                }
            }

            return Kernels.Custom;
        }

        private static string Display(string resourceKey) =>
            Kernels.ResourceManager.GetString(resourceKey, Kernels.Culture ?? CultureInfo.CurrentUICulture)!;

        private static string Invariant(string resourceKey) =>
            Kernels.ResourceManager.GetString(resourceKey, CultureInfo.InvariantCulture)!;
    }
}
