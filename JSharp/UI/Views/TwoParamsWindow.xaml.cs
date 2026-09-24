using System.Windows;

namespace JSharp.UI.Views
{
    /// <summary>
    /// Interaction logic for TwoParamsWindow.xaml
    /// </summary>
    public partial class TwoParamsWindow : Window
    {
        public TwoParamsWindow(string title)
        {
            InitializeComponent();
            this.Title = title;
        }
    }
}
