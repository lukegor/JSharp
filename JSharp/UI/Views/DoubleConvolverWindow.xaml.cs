﻿using System.Windows;
using System.Windows.Controls;
using System.Windows.Input;
using JSharp.ViewModels;

namespace JSharp.UI.Views
{
    /// <summary>
    /// Interaction logic for DoubleConvolverWindow.xaml
    /// </summary>
    public partial class DoubleConvolverWindow : Window
    {
        public DoubleConvolverWindow()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Enable entering only numbers and like backspacing
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void KernelInputCell_PreviewKeyDown(object sender, KeyEventArgs e)
        {
            if (!((e.Key >= Key.D0 && e.Key <= Key.D9) || (e.Key >= Key.NumPad0 && e.Key <= Key.NumPad9) || e.Key == Key.Back || e.Key == Key.Delete))
            {
                e.Handled = true;
            }
        }

        private void KernelInputCell_TextChanged(object sender, TextChangedEventArgs e)
        {
            (DataContext as DoubleConvolverWindowViewModel)?.KernelInputCell_TextChanged();
        }
    }
}
