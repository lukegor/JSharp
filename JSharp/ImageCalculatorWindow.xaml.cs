using JSharp.Models;
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

            double blendFactor1 = -1;
            double blendFactor2 = -1;

            if (fileName1 == null || fileName2 == null || stringOperation == null)
            {
                throw new NullReferenceException("Something is null and it shouldn't");
            }

            OperationData operationData;
            OperationType operation = (OperationType)Enum.Parse(typeof(OperationType), stringOperation);
            if (operation == OperationType.BLEND)
            {
                if (TxtBlendFactor.Text != null && double.TryParse(TxtBlendFactor.Text, out blendFactor1))
                {
                    operationData = new OperationData(operation, blendFactor1);
                }
                else
                {
                    throw new ArgumentException();
                }
            }
            else
            {
                operationData = new OperationData(operation);
            }

            bool shouldCreateNewWindow = (bool)ChkCreateNewWindow.IsChecked;

            (DataContext as ImageCalculatorWindowViewModel)?.BtnConfirm_Click(fileName1, fileName2, operationData, shouldCreateNewWindow);
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
                Txb1.Visibility = Visibility.Visible;
            }
            else
            {
                TxtBlendFactor.Visibility = Visibility.Collapsed;
                Txb1.Visibility = Visibility.Collapsed;
            }

            if (CbOperation.SelectedValue != null && CbOperation.SelectedValue.ToString() == OperationType.NOT.ToString())
            {
                CbImage2.Visibility = Visibility.Collapsed;
                TxbImage2.Visibility = Visibility.Collapsed;

                CbImage2.SelectedItem = null;
            }
            else
            {
                CbImage2.Visibility = Visibility.Visible;
                TxbImage2.Visibility = Visibility.Visible;
            }
        }
    }
}
