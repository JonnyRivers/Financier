using Financier.Desktop.ViewModels;
using System.Windows;

namespace Financier.Desktop.Views
{
    /// <summary>
    /// Interaction logic for AccountRelationshipDetailsWindow.xaml
    /// </summary>
    public partial class AccountRelationshipDetailsWindow : Window
    {
        public AccountRelationshipDetailsWindow(IAccountRelationshipDetailsViewModel viewModel)
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
