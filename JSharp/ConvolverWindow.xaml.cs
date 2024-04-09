﻿using JSharp.ViewModels;
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
    /// Interaction logic for ConvolverWindow.xaml
    /// </summary>
    public partial class ConvolverWindow : Window
    {
        public ConvolverWindow()
        {
            InitializeComponent();
        }

        private void KernelInputCell_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Delete))
            {
                e.Handled = true;
            }
        }

        private void KernelInputCell_TextChanged(object sender, TextChangedEventArgs e)
        {
            (DataContext as ConvolverWindowViewModel)?.KernelInputCell_TextChanged();
        }
    }
}
