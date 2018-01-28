using Financier.Desktop.ViewModels;
using System.Windows;

namespace Financier.Desktop.Views
{
    /// <summary>
    /// Interaction logic for AccountTransactionListWindow.xaml
    /// </summary>
    public partial class AccountTransactionListWindow : Window
    {
        public AccountTransactionListWindow(IAccountTransactionListViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
