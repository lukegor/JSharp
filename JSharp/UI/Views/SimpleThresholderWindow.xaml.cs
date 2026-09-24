using JSharp.Shared.Resources;
using JSharp.ViewModels;
using System.Windows;
using System.Windows.Controls;

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

        private void ComboBox_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            if (sender is ComboBox cb)
            {
                var vm = (SimpleThresholderWindowViewModel)DataContext;
                if (cb.SelectedValue.ToString() == Thresholding.ThresholdingOtsu)
                {
                    vm.FromValue = 0;
                    fromSlider.IsEnabled = false;
                }
                else
                {
                    fromSlider.IsEnabled = true;
                }
            }
        }
    }
}
