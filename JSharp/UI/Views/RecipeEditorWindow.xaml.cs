using System.Windows;

namespace JSharp.UI.Views
{
    /// <summary>
    /// Interaction logic for RecipeEditorWindow.xaml. Code-behind stays thin:
    /// the DataContext is assigned by the opener (MainWindowViewModel).
    /// </summary>
    public partial class RecipeEditorWindow : Window
    {
        public RecipeEditorWindow()
        {
            InitializeComponent();
        }
    }
}
