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
    /// Interaction logic for ImageCalculatorWindow.xaml
    /// </summary>
    public partial class ImageCalculatorWindow : Window
    {
        public ImageCalculatorWindow()
        {
            InitializeComponent();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            string? fileName1 = this.CbImage1.SelectedItem.ToString();
            string? fileName2 = this.CbImage2.SelectedItem.ToString();

            string? stringOperation = CbOperation.SelectedItem.ToString();
            string? stringOverflowHandlingMethod = CbOverflowHandling.SelectedItem.ToString();

            if (fileName1 == null || fileName2 == null || stringOperation == null || stringOverflowHandlingMethod == null)
            {
                throw new NullReferenceException("Something is null and it shouldn't");
            }

            OperationType operation = (OperationType)Enum.Parse(typeof(OperationType), stringOperation);
            PixelOverflowHandlingType method = (PixelOverflowHandlingType)Enum.Parse(typeof(PixelOverflowHandlingType), stringOverflowHandlingMethod);

            (DataContext as ImageCalculatorWindowViewModel)?.BtnConfirm_Click(fileName1, fileName2, operation, method);
        }
    }
}
