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
    /// Interaction logic for PosterizeWindow.xaml
    /// </summary>
    public partial class PosterizeWindow : Window
    {
        public PosterizeWindow()
        {
            InitializeComponent();
        }

        private void BtnConfirm_Click(object sender, RoutedEventArgs e)
        {
            int levelsNumber = Convert.ToInt32(SliderPosterizationLevels.Value);
            (DataContext as PosterizeWindowViewModel)?.BtnConfirmLogic_Click(levelsNumber);
            this.Close();
        }
    }
}
