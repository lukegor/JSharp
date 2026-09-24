using System.Windows;

namespace JSharp.UI.Views
{
    /// <summary>
    /// Interaction logic for RecipeManagerWindow.xaml. Code-behind stays thin:
    /// the DataContext is assigned by the opener (MainWindowViewModel).
    /// </summary>
    public partial class RecipeManagerWindow : Window
    {
        public RecipeManagerWindow()
        {
            InitializeComponent();
        }
    }
}
