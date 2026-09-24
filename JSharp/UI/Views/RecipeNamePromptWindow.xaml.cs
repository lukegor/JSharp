using System.Windows;

namespace JSharp.UI.Views
{
    /// <summary>
    /// Interaction logic for RecipeNamePromptWindow.xaml. Code-behind stays thin:
    /// the DataContext is assigned by the dialog service.
    /// </summary>
    public partial class RecipeNamePromptWindow : Window
    {
        public RecipeNamePromptWindow()
        {
            InitializeComponent();
        }
    }
}
