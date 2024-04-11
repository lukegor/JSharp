using JSharp.Resources;
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
            string? fileName1 = this.CbImage1.SelectedValue.ToString();
            string? fileName2 = this.CbImage2.SelectedValue.ToString();

            string? stringOperation = CbOperation.SelectedItem.ToString();
            string? stringOverflowHandlingMethod = CbOverflowHandling.SelectedItem.ToString();

            if (fileName1 == null || fileName2 == null || stringOperation == null || stringOverflowHandlingMethod == null)
            {
                throw new NullReferenceException("Something is null and it shouldn't");
            }

            OperationType operation = (OperationType)Enum.Parse(typeof(OperationType), stringOperation);
            //if (operation == OperationType.BLEND)
            //{
            //    if (TxtBlendFactor.Text == null || double.TryParse(TxtBlendFactor.Text, out double blendFactor))
            //    {

            //    }
            //    else
            //    {
            //        throw new ArgumentException();
            //    }
            //}

            PixelOverflowHandlingType method = PixelOverflowHandlingHelper.GetStringToEnumMapping(stringOverflowHandlingMethod);
            //method = (PixelOverflowHandlingType)Enum.Parse(typeof(PixelOverflowHandlingType), stringOverflowHandlingMethod);

            bool shouldCreateNewWindow = (bool)ChkCreateNewWindow.IsChecked;

            (DataContext as ImageCalculatorWindowViewModel)?.BtnConfirm_Click(fileName1, fileName2, operation, method, shouldCreateNewWindow);
        }

        private void BtnCancel_Click(object sender, RoutedEventArgs e)
        {
            this.Close();
        }

        private void CbOperation_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (CbOperation.SelectedValue != null && CbOperation.SelectedValue.ToString() == OperationType.BLEND.ToString())
            {
                TxtBlendFactor.Visibility = Visibility.Visible;
            }
            else
            {
                TxtBlendFactor.Visibility = Visibility.Collapsed;
            }
        }
    }
}
