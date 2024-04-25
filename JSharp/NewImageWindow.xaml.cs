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

        private void imageControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            // Get the position where the user clicked on the image
            if (sender is Image image && MainWindowViewModel.SelectedButton != null && MainWindowViewModel.SelectedButton.Name == Constants.RadioBtn1Name)
            {
                if (MainWindowViewModel.points[0] != null && MainWindowViewModel.points[1] != null)
                {
                    // Reset points if two points are already selected
                    MainWindowViewModel.points[0] = null;
                    MainWindowViewModel.points[1] = null;

                    // Clear previous highlights and lines
                    highlightCanvas.Children.Clear();
                }

                Point clickedPoint = e.GetPosition(image);
                //var window = App.Current.Windows.OfType<NewImageWindow>().FirstOrDefault(x => x.DataContext == this);
                //double scaleX = imageControl.ActualWidth / imageControl.Source.Width;
                //double scaleY = imageControl.ActualHeight / imageControl.Source.Height;
                //Point actualClickedPoint = new Point(clickedPoint.X / scaleX, clickedPoint.Y / scaleY);
                //if (clickedPoint.X != actualClickedPoint.X || clickedPoint.Y != actualClickedPoint.Y)
                //{
                //    throw new Exception($"Original clicked point: {clickedPoint.X},{clickedPoint.Y}\nNormalized point: {actualClickedPoint.X},{actualClickedPoint.Y}\n");
                //}
                if (MainWindowViewModel.points[0] == null)
                {
                    MainWindowViewModel.points[0] = clickedPoint;
                }
                else if (MainWindowViewModel.points[1] == null)
                {
                    MainWindowViewModel.points[1] = clickedPoint;
                    // Both points are selected, draw line
                    DrawLine((Point)MainWindowViewModel.points[0], (Point)MainWindowViewModel.points[1]);
                }

                DrawHighlight(clickedPoint);

                //profileLineButton.Background = Brushes.DarkGray;
            }
        }

        private void DrawHighlight(Point point)
        {
            // Create a highlight ellipse
            System.Windows.Shapes.Rectangle highlightEllipse = new System.Windows.Shapes.Rectangle
            {
                Width = 10,
                Height = 10,
                Stroke = Brushes.Yellow,
                StrokeThickness = 2,
                Fill = Brushes.Yellow
            };
            // Position the highlight relative to the image
            Canvas.SetLeft(highlightEllipse, point.X - highlightEllipse.Width / 2);
            Canvas.SetTop(highlightEllipse, point.Y - highlightEllipse.Height / 2);
            // Add the highlight to the canvas
            highlightCanvas.Children.Add(highlightEllipse);
        }

        private void DrawLine(Point startPoint, Point endPoint)
        {
            Line line = new Line
            {
                X1 = startPoint.X,
                Y1 = startPoint.Y,
                X2 = endPoint.X,
                Y2 = endPoint.Y,
                Stroke = Brushes.Red,
                StrokeThickness = 2
            };
            highlightCanvas.Children.Add(line);
        }
    }
}
