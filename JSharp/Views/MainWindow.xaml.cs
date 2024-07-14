using Emgu.CV.Structure;
using Emgu.CV;
using Microsoft.Win32;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using System.Drawing;
using Emgu.CV.CvEnum;
using JSharp.Utility;
using JSharp.Resources;
using System.Windows.Interop;
using System.Globalization;
using Emgu.Util;
using Emgu.CV.Util;
using JSharp.ViewModels;
using Emgu.CV.Features2D;

namespace JSharp
{
    /// <summary>
    /// Interaction logic for MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }

        private void About_Click(object sender, RoutedEventArgs e)
        {
            MessageBox.Show($"JSharp{Environment.NewLine}{Strings.Image_Processing_Program}{Environment.NewLine}{Environment.NewLine}{Strings.Author}:{Environment.NewLine}Łukasz Górski{Environment.NewLine}{Environment.NewLine}Copyright © 2024 Łukasz Górski", $"{UIStrings.About}", MessageBoxButton.OK, MessageBoxImage.Information);
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            Environment.Exit(Environment.ExitCode);
            //BETTER FOR FUTURE App.Current.Shutdown();
        }

        private void Window_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (Keyboard.Modifiers == ModifierKeys.Control)
            {
                if (e.Key == Key.S)
                {
                    // Ctrl+S was pressed
                    (DataContext as MainWindowViewModel)?.Save();

                    e.Handled = true;
                }
            }
        }

        private void RadioButton_Checked(object sender, RoutedEventArgs e)
        {
            (DataContext as MainWindowViewModel)?.UpdateCheckedRadioButton(sender);
            var radioButton = sender as RadioButton;

            if (radioButton.Name == Constants.RadioBtnNone)
            {
                //uncheck
                radioButton.IsChecked = false;

                var window = App.Current.Windows.OfType<NewImageWindow>().FirstOrDefault(x => x.DataContext == MainWindowViewModel.FocusedImage);
                var vm = window?.DataContext as NewImageWindowViewModel;

                if (vm != null)
                {
                    // Reset points
                    vm.Points[0] = null;
                    vm.Points[1] = null;

                    // Clear previous highlights and lines
                    window?.highlightCanvas.Children.Clear();
                }
                return;
            }

            HighlightSelectedButton(radioButton);
        }

        private void RadioButtonClear_Checked(object sender, RoutedEventArgs e)
        {
            var radioButton = sender as RadioButton;

            List<NewImageWindow> list = App.Current.Windows.OfType<NewImageWindow>().ToList();
            foreach (var window in list)
            {
                window?.highlightCanvas.Children.Clear();
                (window?.DataContext as NewImageWindowViewModel).Points[0] = null;
                (window?.DataContext as NewImageWindowViewModel).Points[1] = null;
            }
            (this.DataContext as MainWindowViewModel).Descriptor = string.Empty;

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
            var radioButton = sender as RadioButton;
            radioButton.Background.Opacity = 1.0;
        }
    }
}