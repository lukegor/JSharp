using JSharp.ViewModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
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
