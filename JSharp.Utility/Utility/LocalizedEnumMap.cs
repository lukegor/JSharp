using System.Diagnostics;
using System.Globalization;
using System.Resources;

namespace JSharp.Utility.Utility;

/// <summary>
/// Culture-robust mapping between enum values and localized display names.
/// Entry tables are keyed by resource name (culture-free); display names resolve
/// per call against the current UI culture with invariant fallback, so a
/// mid-process culture change can never break lookups.
/// </summary>
public sealed class LocalizedEnumMap<TEnum> where TEnum : struct, Enum
{
    private readonly ResourceManager _resources;
    private readonly Func<CultureInfo?> _currentCulture;
    private readonly (string ResourceKey, TEnum Value)[] _entries;

    public LocalizedEnumMap(
        ResourceManager resources,
        (string ResourceKey, TEnum Value)[] entries,
        Func<CultureInfo?>? currentCulture = null)
    {
        _resources = resources;
        _entries = entries;
        _currentCulture = currentCulture ?? (() => CultureInfo.CurrentUICulture);
    }

    public bool TryParse(string? displayName, out TEnum value)
    {
        if (displayName is not null)
        {
            foreach ((string resourceKey, TEnum entryValue) in _entries)
            {
                if (displayName == Display(resourceKey) || displayName == Invariant(resourceKey))
                {
                    value = entryValue;
                    return true;
                }
            }
        }

        value = default;
        return false;
    }

    public TEnum Parse(string displayName)
    {
        ArgumentNullException.ThrowIfNull(displayName);
        if (TryParse(displayName, out TEnum value))
        {
            return value;
        }

        throw new InvalidOperationException("Invalid value for the given enum type.");
    }

    public string Display(TEnum value)
    {
        foreach ((string resourceKey, TEnum entryValue) in _entries)
        {
            if (EqualityComparer<TEnum>.Default.Equals(entryValue, value))
            {
                return Display(resourceKey);
            }
        }

        throw new UnreachableException("Enum mapping exhausted without a match - non-exhaustive switch.");
    }

    public IEnumerable<string> DisplayMany(IEnumerable<TEnum> values)
    {
        ArgumentNullException.ThrowIfNull(values);
        return values.Select(Display);
    }

    private string Display(string resourceKey) =>
        _resources.GetString(resourceKey, _currentCulture())!;

    private string Invariant(string resourceKey) =>
        _resources.GetString(resourceKey, CultureInfo.InvariantCulture)!;
}
