using System.Windows;
using System.Windows.Controls;
using JSharp.Domain.Recipes;
using JSharp.ViewModels;

namespace JSharp.UI.Views
{
    /// <summary>
    /// Minimal modal prompt for Tier-1 recipe inputs. Built in code (no XAML):
    /// one prefilled <see cref="TextBox"/> per declared input, OK/Cancel.
    /// OK returns every input's raw text; Cancel (or window close) returns null
    /// so the caller aborts the run. Typed parsing happens later in the binder;
    /// unparseable values surface as preflight errors quoting the expected type.
    /// Paired with <see cref="RecipeInputsWindowViewModel"/> (R5); the window
    /// builds TextBoxes in code and writes results back into the VM.
    /// </summary>
    internal sealed class RecipeInputsWindow : Window
    {
        private readonly Dictionary<string, TextBox> _boxes = new(StringComparer.Ordinal);
        private readonly RecipeInputsWindowViewModel _viewModel;

        private RecipeInputsWindow(RecipeInputsWindowViewModel viewModel)
        {
            _viewModel = viewModel;
            DataContext = viewModel;
            Title = "Recipe inputs";
            Width = 360;
            SizeToContent = SizeToContent.Height;
            WindowStartupLocation = WindowStartupLocation.CenterScreen;
            ResizeMode = ResizeMode.NoResize;

            StackPanel root = new() { Margin = new Thickness(12) };
            foreach (string name in viewModel.Names)
            {
                string def = viewModel.Defaults[name];
                string description = viewModel.Descriptions.TryGetValue(name, out string? d) ? d : string.Empty;
                string label = string.IsNullOrWhiteSpace(description)
                    ? $"{name} [{def}]:"
                    : $"{name} [{def}]: {description}";
                root.Children.Add(new TextBlock { Text = label, TextWrapping = TextWrapping.Wrap });
                TextBox box = new() { Text = def, Margin = new Thickness(0, 2, 0, 8) };
                _boxes[name] = box;
                root.Children.Add(box);
            }

            StackPanel buttons = new()
            {
                Orientation = Orientation.Horizontal,
                HorizontalAlignment = HorizontalAlignment.Right,
            };
            Button ok = new() { Content = "OK", Width = 75, Margin = new Thickness(0, 0, 8, 0), IsDefault = true };
            ok.Click += (_, _) => { DialogResult = true; };
            Button cancel = new() { Content = "Cancel", Width = 75, IsCancel = true };
            buttons.Children.Add(ok);
            buttons.Children.Add(cancel);
            root.Children.Add(buttons);

            Content = new ScrollViewer
            {
                Content = root,
                MaxHeight = 400,
                VerticalScrollBarVisibility = ScrollBarVisibility.Auto,
            };
        }

        public static IReadOnlyDictionary<string, string>? Prompt(
            IReadOnlyDictionary<string, InputDeclaration> declared)
        {
            RecipeInputsWindowViewModel viewModel = new(declared);
            RecipeInputsWindow window = new(viewModel);
            if (Application.Current?.MainWindow is not null)
            {
                window.Owner = Application.Current.MainWindow;
            }

            if (window.ShowDialog() != true)
            {
                return null;
            }

            Dictionary<string, string> texts = new(StringComparer.Ordinal);
            foreach (KeyValuePair<string, TextBox> kv in window._boxes)
            {
                texts[kv.Key] = kv.Value.Text;
                viewModel.Values[kv.Key] = kv.Value.Text;
            }

            return texts;
        }
    }
}
