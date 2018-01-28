using Financier.Desktop.ViewModels;
using System.Windows;

namespace Financier.Desktop.Views
{
    /// <summary>
    /// Interaction logic for AccountListWindow.xaml
    /// </summary>
    public partial class AccountListWindow : Window
    {
        public AccountListWindow(IAccountListViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
