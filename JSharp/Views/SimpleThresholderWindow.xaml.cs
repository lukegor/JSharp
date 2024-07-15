using JSharp.Resources;
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
    /// Interaction logic for SimpleThresholderWindow.xaml
    /// </summary>
    public partial class SimpleThresholderWindow : Window
    {
        public SimpleThresholderWindow()
        {
            InitializeComponent();
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (DataContext as SimpleThresholderWindowViewModel)?.OnClosing();
        }

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cb)
            {
                var vm = this.DataContext as SimpleThresholderWindowViewModel;
                if (cb.SelectedValue == Thresholding.ThresholdingOtsu)
                {
                    //vm.FromValue =
                }
                else
                {
                    //
                }
            }
        }
    }
}
