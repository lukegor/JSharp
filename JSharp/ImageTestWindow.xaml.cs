using JSharp.Utility;
using JSharp.ViewModels;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace JSharp
{
    /// <summary>
    /// Interaction logic for ImageTestWindow.xaml
    /// </summary>
    public partial class ImageTestWindow : Window
    {
        public ImageTestWindow()
        {
            InitializeComponent();
            this.imageControl.Source = MainWindowViewModel.FocusedImage.MatImage.Clone().MatToBitmapSource();
        }
    }
}
