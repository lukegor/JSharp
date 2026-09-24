using System.Windows;
using JSharp.UI.Views;
using JSharp.ViewModels;
using JSharp.ViewModels.Abstractions;

namespace JSharp.Services
{
    /// <summary>
    /// Owns every parameter-dialog window: maps viewmodel types to windows. Modal dialogs
    /// return the viewmodel's Result (null = cancelled); modeless live-preview windows close
    /// on request and hand a completed Result to the confirm callback.
    /// </summary>
    internal sealed class WpfDialogService : IDialogService
    {
        private delegate Window WindowFactory();

        private static readonly Dictionary<Type, WindowFactory> Windows = new()
        {
            [typeof(TwoParamsWindowViewModel)] = () => new TwoParamsWindow(JSharp.Utility.Utility.Constants.CannySettingsWindowTitle),
            [typeof(ImageCalculatorWindowViewModel)] = () => new ImageCalculatorWindow(),
            [typeof(InpaintWindowViewModel)] = () => new InpaintWindow(),
            [typeof(PosterizeWindowViewModel)] = () => new PosterizeWindow(),
            [typeof(SimpleThresholderWindowViewModel)] = () => new SimpleThresholderWindow(),
            [typeof(ThresholderWindowViewModel)] = () => new ThresholderWindow(),
            [typeof(MedianWindowViewModel)] = () => new MedianWindow(),
            [typeof(PyramidWindowViewModel)] = () => new PyramidWindow(),
            [typeof(StretchContrastWindowViewModel)] = () => new StretchContrastWindow(),
            [typeof(AnalyzeParticlesWindowViewModel)] = () => new AnalyzeParticlesWindow(),
            [typeof(ConvolverWindowViewModel)] = () => new ConvolverWindow(),
            [typeof(DoubleConvolverWindowViewModel)] = () => new DoubleConvolverWindow(),
            [typeof(StandardMorphologicalWindowViewModel)] = () => new StandardMorphologicalWindow(),
            [typeof(RecipeNamePromptWindowViewModel)] = () => new RecipeNamePromptWindow(),
        };

        internal static bool IsRegistered(Type viewModelType) => Windows.ContainsKey(viewModelType);

        internal static Window CreateWindowFor(Type viewModelType)
        {
            if (!Windows.TryGetValue(viewModelType, out WindowFactory? factory))
            {
                throw new InvalidOperationException($"No window is registered for '{viewModelType.Name}'.");
            }

            return factory();
        }

        public (bool Confirmed, TResult Result) ShowDialog<TViewModel, TResult>(TViewModel viewModel)
            where TViewModel : DialogViewModel<TResult>
        {
            Window window = CreateWindowFor(typeof(TViewModel));
            window.Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            window.DataContext = viewModel;

            EventHandler closeHandler = (_, _) => window.Close();
            viewModel.CloseRequested += closeHandler;
            try
            {
                window.ShowDialog();
                return (Confirmed: viewModel.HasResult, Result: viewModel.Result!);
            }
            finally
            {
                viewModel.CloseRequested -= closeHandler;
            }
        }

        public void Show<TViewModel, TResult>(TViewModel viewModel, Func<TResult, Task> onConfirm, Action? onDismissed = null)
            where TViewModel : DialogViewModel<TResult>
        {
            Window window = CreateWindowFor(typeof(TViewModel));
            window.Owner = Application.Current?.Windows.OfType<Window>().FirstOrDefault(w => w.IsActive);
            window.DataContext = viewModel;

            // Modeless: Show() returns immediately, so the result cannot be read here.
            // CloseRequested closes the window; Closed (user confirm/cancel/X) detaches
            // and hands a completed Result to onConfirm.
            EventHandler? closeHandler = null;
            EventHandler? closedHandler = null;
            closeHandler = (_, _) => window.Close();
            closedHandler = (_, _) =>
            {
                window.Closed -= closedHandler;
                viewModel.CloseRequested -= closeHandler;
                if (viewModel.HasResult)
                {
                    // Faults surface through TaskScheduler.UnobservedTaskException (logged in App).
                    _ = OnConfirmAsync(onConfirm, viewModel.Result!);
                }
                else
                {
                    onDismissed?.Invoke();
                }
            };

            viewModel.CloseRequested += closeHandler;
            window.Closed += closedHandler;
            window.Show();
        }

        private static async Task OnConfirmAsync<TResult>(Func<TResult, Task> onConfirm, TResult result)
        {
            await onConfirm(result).ConfigureAwait(false);
        }
    }
}
