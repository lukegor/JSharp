using JSharp.Utility.Utility;
using JSharp.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;

namespace JSharp.UI.Views
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
#if JSHARP_OPEN
            RecipesMenu.Visibility = Visibility.Collapsed;
#endif
            DataContextChanged += OnDataContextChanged;
        }

        private MainWindowViewModel? _exitSource;

        private void OnDataContextChanged(object sender, DependencyPropertyChangedEventArgs e)
        {
            if (_exitSource != null)
            {
                _exitSource.ExitRequested -= OnExitRequested;
            }

            _exitSource = DataContext as MainWindowViewModel;
            if (_exitSource != null)
            {
                _exitSource.ExitRequested += OnExitRequested;
            }
        }

        private void OnExitRequested(object? sender, EventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            var vm = (MainWindowViewModel)this.DataContext;
            vm.ShowAbout();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Application.Current.Shutdown();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            var vm = (MainWindowViewModel)DataContext;

            // escape-routes: Esc cancels a running operation.
            if (e.Key == Key.Escape && vm.IsBusy)
            {
                vm.CancelOperationCommand.Execute(null);
                e.Handled = true;
                return;
            }

            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                // Ctrl+S
                if (e.Key == Key.S)
                {
                    vm.Save();

                    e.Handled = true;
                }
                // Ctrl+Z: undo on the focused image whatever window has keyboard focus.
                // Silent no-op when there is nothing to undo (no image, empty history).
                else if (e.Key == Key.Z)
                {
                    vm.FocusedImageVm?.Undo();

                    e.Handled = true;
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            ((MainWindowViewModel)DataContext).UpdateCheckedRadioButton(sender);
            var radioButton = (RadioButton)sender;

            if (radioButton.Tag.ToString() == Tools.None)
            {
                //uncheck
                radioButton.IsChecked = false;

                var mainVm = (MainWindowViewModel)DataContext;
                var focusedWindow = App.Current.Windows.OfType<NewImageWindow>().FirstOrDefault(x => x.DataContext == mainVm.FocusedImageVm);
                var vm = (NewImageWindowViewModel)focusedWindow!.DataContext;
                vm.ResetPoints();

                // Clear previous highlights and lines
                focusedWindow?.highlightCanvas.Children.Clear();

                return;
            }

            HighlightSelectedButton(radioButton);
        }

        private void RadioButtonClear_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;

            IEnumerable<NewImageWindow> imageWindows = App.Current.Windows.OfType<NewImageWindow>().ToArray();
            foreach (var window in imageWindows)
            {
                window.ClearCanvas();
                var vm = (NewImageWindowViewModel)window.DataContext;
                vm.ResetPoints();
            }
            ((MainWindowViewModel)DataContext).Descriptor = string.Empty;

            //uncheck
            radioButton.IsChecked = false;
        }

        private void HighlightSelectedButton(RadioButton radioButton)
        {
            System.Windows.Media.Brush brush = radioButton.Background;
            double reducedOpacity = Math.Max(0, brush.Opacity - 0.7); // Adjust the value as needed
            // Apply the modified SolidColorBrush as the new background
            radioButton.Background.Opacity = reducedOpacity;
        }

        private void RadioButton_Unchecked(object sender, RoutedEventArgs e)
        {
            var radioButton = (RadioButton)sender;
            radioButton.Background.Opacity = 1.0;
        }
    }
}