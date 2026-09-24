using System.Globalization;
using JSharp.Shared.Resources;

namespace JSharp.Domain.Operations;

/// <summary>
/// Locale-invariant canonical names for filter.edge-detection kernels.
/// Recipes persist these exact strings: recording maps the current-culture
/// display name back via TryToInvariant, and play/validation accept only
/// the invariant trio with ordinal equality.
/// </summary>
public static class EdgeKernelNames
{
    public static string SobelNS =>
        Kernels.ResourceManager.GetString("SobelNS", CultureInfo.InvariantCulture)!;

    public static string SobelEW =>
        Kernels.ResourceManager.GetString("SobelEW", CultureInfo.InvariantCulture)!;

    public static string Canny =>
        Kernels.ResourceManager.GetString("Canny", CultureInfo.InvariantCulture)!;

    public static bool IsInvariant(string? name) =>
        name == SobelNS || name == SobelEW || name == Canny;

    /// <summary>
    /// Maps a kernel name to its invariant canonical form. Accepts the
    /// invariant trio (ordinal) and the current-culture display values.
    /// Returns false for anything else; the caller preserves today's
    /// downstream rejection behavior for unknown names.
    /// </summary>
    public static bool TryToInvariant(string? name, out string invariant)
    {
        if (name is not null && IsInvariant(name))
        {
            invariant = name;
            return true;
        }

        if (name == Kernels.SobelNS)
        {
            invariant = SobelNS;
            return true;
        }

        if (name == Kernels.SobelEW)
        {
            invariant = SobelEW;
            return true;
        }

        if (name == Kernels.Canny)
        {
            invariant = Canny;
            return true;
        }

        invariant = string.Empty;
        return false;
    }
}
