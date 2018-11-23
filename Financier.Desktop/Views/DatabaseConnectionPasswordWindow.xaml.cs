using Financier.Desktop.ViewModels;
using System.Windows;

namespace Financier.Desktop.Views
{
    /// <summary>
    /// Interaction logic for DatabaseConnectionPasswordWindow.xaml
    /// </summary>
    public partial class DatabaseConnectionPasswordWindow : Window
    {
        public DatabaseConnectionPasswordWindow(IDatabaseConnectionPasswordViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }

        private void OnOK(object sender, RoutedEventArgs e)
        {
            IDatabaseConnectionPasswordViewModel viewModel = (IDatabaseConnectionPasswordViewModel)DataContext;
            viewModel.Password = passwordBox.Password;

            DialogResult = true;
        }

        private void OnCancel(object sender, RoutedEventArgs e)
        {
            DialogResult = false;
        }
    }
}
