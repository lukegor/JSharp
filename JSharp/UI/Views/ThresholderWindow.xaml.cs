using System.Windows;
using JSharp.ViewModels;

namespace JSharp.UI.Views
{
    /// <summary>
    /// Interaction logic for ThresholderWindow.xaml
    /// </summary>
    public partial class ThresholderWindow : Window
    {
        public bool? Result { get; set; }

        public ThresholderWindow()
        {
            InitializeComponent();
            Result = false;
        }

        private void Window_Closing(object sender, System.ComponentModel.CancelEventArgs e)
        {
            (DataContext as ThresholderWindowViewModel)?.OnClosing(this.Result);
        }
    }
}
