using System.Windows;
using System.Windows.Controls;
using JSharp.Resources;
using JSharp.ViewModels;

namespace JSharp.UI.Views
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
