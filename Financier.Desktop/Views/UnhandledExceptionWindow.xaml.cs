﻿using Financier.Desktop.ViewModels;
using System.Windows;

namespace Financier.Desktop.Views
{
    /// <summary>
    /// Interaction logic for AccountDetailsWindow.xaml
    /// </summary>
    public partial class UnhandledExceptionWindow : Window
    {
        public UnhandledExceptionWindow(IExceptionViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            DialogResult = true;
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
