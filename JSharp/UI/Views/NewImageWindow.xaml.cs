using JSharp.Utility.Utility;
using JSharp.ViewModels;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Shapes;

namespace JSharp.UI.Views
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

        private NewImageWindowViewModel viewModel = null!;

        private MainWindowViewModel MainVm => viewModel.Owner;

        // Event that invokes just after UI loaded
        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            viewModel = (NewImageWindowViewModel)DataContext;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            viewModel.NewImageWindow_Closing();
        }

        private void Window_Activated(object sender, EventArgs e)
        {
            viewModel.Window_Activated();
        }

        private void Window_KeyDown(object sender, KeyEventArgs e)
        {
            // Check if Ctrl key is pressed
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                var imageVm = (NewImageWindowViewModel)DataContext;
                // Handle zoom in (Ctrl+)
                if (e.Key == Key.Add || e.Key == Key.OemPlus)
                {
                    imageVm.ScaleZoom(true);
                }
                // Handle zoom out (Ctrl-)
                else if (e.Key == Key.Subtract || e.Key == Key.OemMinus)
                {
                    imageVm.ScaleZoom(false);
                }
                // Open image copy (Ctrl... + Shift + D)
                else if ((Keyboard.Modifiers & ModifierKeys.Shift) == ModifierKeys.Shift && e.Key == Key.D)
                {
                    MainWindowViewModel mainWindowViewModel = MainVm;
                    mainWindowViewModel.DisplayImage(MainVm.FocusedImageVm!.MatImage, MainVm.FocusedImageVm!.FileName);
                }
                // Undo last operation on this image (Ctrl+Z)
                else if (e.Key == Key.Z)
                {
                    imageVm.Undo();
                }
            }
        }

        private void Window_PreviewMouseWheel(object sender, MouseWheelEventArgs e)
        {
            // Check if Ctrl key is pressed
            if ((Keyboard.Modifiers & ModifierKeys.Control) == ModifierKeys.Control)
            {
                Zoom(e.Delta);
            }
        }

        private void imageControl_MouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            if (sender is Image image)
            {
                var mainWindowVm = MainVm;
                // Get the position where the user clicked on the image
                if (string.IsNullOrEmpty(mainWindowVm.SelectedButtonTag) == false
                    && mainWindowVm.SelectedButtonTag == Tools.ProfileLine)
                {
                    if (viewModel.Points[0] != null && viewModel.Points[1] != null)
                    {
                        // Reset points if two points are already selected
                        viewModel.ResetPoints();

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
                    if (viewModel.Points[0] == null)
                    {
                        viewModel.Points[0] = clickedPoint;
                        mainWindowVm.UpdateDescriptor();
                    }
                    else if (viewModel.Points[1] == null)
                    {
                        viewModel.Points[1] = clickedPoint;
                        mainWindowVm.UpdateDescriptor();
                        // Both points are selected, draw line
                        DrawLine(viewModel.Points[0]!.Value, viewModel.Points[1]!.Value);
                    }

                    DrawHighlight(clickedPoint);

                    //profileLineButton.Background = Brushes.DarkGray;
                }
                else if (string.IsNullOrEmpty(mainWindowVm.SelectedButtonTag) == false
                    && mainWindowVm.SelectedButtonTag == Tools.Rectangle)
                {
                    if (viewModel.Points[0] != null && viewModel.Points[1] != null)
                    {
                        // Reset points if two points are already selected
                        viewModel.ResetPoints();

                        // Clear previous highlights and lines
                        highlightCanvas.Children.Clear();
                    }

                    Point clickedPoint = e.GetPosition(image);

                    if (viewModel.Points[0] == null)
                    {
                        viewModel.Points[0] = clickedPoint;
                        mainWindowVm.UpdateDescriptor();
                    }
                    else if (viewModel.Points[1] == null)
                    {
                        viewModel.Points[1] = clickedPoint;
                        mainWindowVm.UpdateDescriptor();
                        // Both points are selected, draw line
                        DrawRectangle(viewModel.Points[0]!.Value, viewModel.Points[1]!.Value);
                    }
                    DrawHighlight(clickedPoint);
                }
            }
        }

        private void DrawRectangle(Point startPoint, Point endPoint)
        {
            // Calculate the width and height of the rectangle
            double width = Math.Abs(endPoint.X - startPoint.X);
            double height = Math.Abs(endPoint.Y - startPoint.Y);

            // Determine the top-left corner coordinates of the rectangle
            double x = Math.Min(startPoint.X, endPoint.X);
            double y = Math.Min(startPoint.Y, endPoint.Y);

            Rectangle rectangle = new Rectangle()
            {
                Width = width,
                Height = height,
                Stroke = Brushes.Black, // Set the stroke color
                StrokeThickness = 2,   // Set the thickness of the stroke
                Fill = Brushes.Transparent, // Set the fill color (or use Brushes.Transparent for no fill)
                Margin = new Thickness(x, y, 0, 0) // Set the margin to position the rectangle
            };

            // Add the rectangle to the canvas or any other container
            highlightCanvas.Children.Add(rectangle);
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

        private void imageControl_MouseMove(object sender, MouseEventArgs e)
        {
            if (sender is Image image)
            {
                viewModel.MousePosition = e.GetPosition(image);
            }
        }

        /// <summary>
        /// Clears drawings, highlights and lines
        /// </summary>
        public void ClearCanvas()
        {
            this.highlightCanvas.Children.Clear();
        }

        private void Zoom(int delta)
        {
            bool zoomIn = delta > 0;

            // Adjust the zoom scale
            if (zoomIn)
            {
                viewModel.ScaleZoom(true);
            }
            else
            {
                viewModel.ScaleZoom(false);
            }
        }
    }
}
