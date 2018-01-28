using Financier.Desktop.ViewModels;
using System.Windows;

namespace Financier.Desktop.Views
{
    /// <summary>
    /// Interaction logic for TransactionListWindow.xaml
    /// </summary>
    public partial class TransactionListWindow : Window
    {
        public TransactionListWindow(ITransactionListViewModel viewModel)
        {
            InitializeComponent();

            DataContext = viewModel;
        }
    }
}
