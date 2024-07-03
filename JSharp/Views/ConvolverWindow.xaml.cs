using JSharp.Resources;
using JSharp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
    /// Interaction logic for ConvolverWindow.xaml
    /// </summary>
    public partial class ConvolverWindow : Window
    {
        public ConvolverWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Enable entering only numbers, minus '-', and like backspacing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KernelInputCell_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            System.Diagnostics.Debug.WriteLine(e.Key.ToString());
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) ||
                (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) ||
                e.Key == Key.Back || e.Key == Key.Delete ||
                e.Key == Key.Subtract || e.Key == Key.OemMinus))
            {
                e.Handled = true;
            }
        }

        private void KernelInputCell_TextChanged(object sender, TextChangedEventArgs e)
        {
            (DataContext as ConvolverWindowViewModel)?.KernelInputCell_TextChanged();
        }

        internal void CurrentKernelText_TextChanged(string currentKernel)
        {
            if (currentKernel == Kernels.Canny)
            {
                for (int i = 0; i <= 2; i++)
                {
                    for (int j = 0; j <= 2; j++)
                    {
                        string cellName = $"Cell{i}{j}";
                        TextBox tb = FindName(cellName) as TextBox;
                        if (tb != null)
                        {
                            tb.Visibility = Visibility.Hidden;
                        }
                    }
                }
            }
            else
            {
                for (int i = 0; i <= 2; i++)
                {
                    for (int j = 0; j <= 2; j++)
                    {
                        string cellName = $"Cell{i}{j}";
                        TextBox tb = FindName(cellName) as TextBox;
                        if (tb != null)
                        {
                            tb.Visibility = Visibility.Visible;
                        }
                    }
                }
            }
        }
    }
}
