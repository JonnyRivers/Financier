using Financier.Desktop.ViewModels;
using System.Windows;

namespace Financier.Desktop.Views
{
    /// <summary>
    /// Interaction logic for AccountCreateWindow.xaml
    /// </summary>
    public partial class AccountEditWindow : Window
    {
        public AccountEditWindow(IAccountEditViewModel viewModel)
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
