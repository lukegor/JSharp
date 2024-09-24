using System.Windows;
using JSharp.ViewModels;

namespace JSharp.UI.Views
{
    /// <summary>
    /// Interaction logic for StretchContrastWindow.xaml
    /// </summary>
    public partial class StretchContrastWindow : Window
    {
        public StretchContrastWindow()
        {
            InitializeComponent();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            int q3 = Convert.ToInt32(SliderQ3.Value);
            int q4 = Convert.ToInt32(SliderQ4.Value);
            (DataContext as StretchContrastWindowViewModel)?.BtnConfirmLogic_Click(q3, q4);
            this.Close();
        }
    }
}
