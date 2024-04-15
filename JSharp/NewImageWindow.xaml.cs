using Emgu.CV;
using Emgu.CV.Reg;
using Emgu.Util;
using JSharp.Utility;
using JSharp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics.CodeAnalysis;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Media.Media3D;
using System.Windows.Shapes;
using static SkiaSharp.HarfBuzz.SKShaper;

namespace JSharp
{
    /// <summary>
    /// Interaction logic for NewImageWindow.xaml
    /// </summary>
    public partial class NewImageWindow : Window
    {
        public NewImageWindow()
        {

            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (DataContext as NewImageWindowViewModel)?.NewImageWindow_Closing();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            (DataContext as NewImageWindowViewModel)?.Window_Activated();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if Ctrl key is pressed
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                // Handle zoom in (Ctrl+)
                if (e.Key == Key.Add || e.Key == Key.OemPlus)
                {
                    (DataContext as NewImageWindowViewModel)?.ScaleZoom(true);
                }
                // Handle zoom out (Ctrl-)
                else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
                {
                    (DataContext as NewImageWindowViewModel)?.ScaleZoom(false);
                }
                else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift && e.Key == Key.D)
                {
                    MainWindow mainWindow = Application.Current.Windows.OfType<MainWindow>().FirstOrDefault();

                    MainWindowViewModel mainWindowViewModel = mainWindow.DataContext as MainWindowViewModel;
                    mainWindowViewModel?.DisplayImage(MainWindowViewModel.FocusedImage.MatImage, MainWindowViewModel.FocusedImage.FileName);
                }
            }
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Check if Ctrl key is pressed
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                // Determine the direction of the mouse scroll
                bool zoomIn = e.Delta > 0;

                // Adjust the zoom scale
                if (zoomIn)
                {
                    (DataContext as NewImageWindowViewModel)?.ScaleZoom(true);
                }
                else
                {
                    (DataContext as NewImageWindowViewModel)?.ScaleZoom(false);
                }
            }
        }
    }
}
