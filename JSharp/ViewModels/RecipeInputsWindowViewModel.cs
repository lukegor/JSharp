using CommunityToolkit.Mvvm.ComponentModel;
using JSharp.Domain.Recipes;

namespace JSharp.ViewModels
{
    /// <summary>
    /// Pairing viewmodel for <see cref="JSharp.UI.Views.RecipeInputsWindow"/>.
    /// Holds the prefilled default texts per declared input name; the window
    /// builds one <c>TextBox</c> per entry in code and writes the edited texts
    /// back into <see cref="Values"/> on OK. Exists to satisfy the R5
    /// window/viewmodel pairing rule; not bound via XAML.
    /// </summary>
    internal sealed class RecipeInputsWindowViewModel : ObservableObject
    {
        public IReadOnlyList<string> Names { get; }

        public IReadOnlyDictionary<string, string> Defaults { get; }

        public IReadOnlyDictionary<string, string> Descriptions { get; }

        public Dictionary<string, string> Values { get; } = new(StringComparer.Ordinal);

        public RecipeInputsWindowViewModel(IReadOnlyDictionary<string, InputDeclaration> declared)
        {
            Names = declared.Keys.ToList();
            Dictionary<string, string> defaults = new(StringComparer.Ordinal);
            foreach (KeyValuePair<string, InputDeclaration> kv in declared)
            {
                string text = DefaultText(kv.Value);
                defaults[kv.Key] = text;
            }

            Defaults = defaults;
            Dictionary<string, string> descriptions = new(StringComparer.Ordinal);
            foreach (KeyValuePair<string, InputDeclaration> kv in declared)
            {
                descriptions[kv.Key] = kv.Value.Description ?? string.Empty;
            }

            Descriptions = descriptions;
            foreach (KeyValuePair<string, string> kv in defaults)
            {
                Values[kv.Key] = kv.Value;
            }
        }

        internal static string DefaultText(InputDeclaration decl) => decl.Default.ValueKind switch
        {
            System.Text.Json.JsonValueKind.String => decl.Default.GetString() ?? string.Empty,
            System.Text.Json.JsonValueKind.True => "true",
            System.Text.Json.JsonValueKind.False => "false",
            _ => decl.Default.GetRawText(),
        };
    }
}
