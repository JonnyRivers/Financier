namespace Financier.Desktop.ViewModels
{
    public class MainWindowViewModel : BaseViewModel, IMainWindowViewModel
    {
        public MainWindowViewModel(
            IAccountListViewModel accountListViewModel,
            ITransactionListViewModel transactionListViewModel
        )
        {
            AccountListViewModel = accountListViewModel;
            TransactionListViewModel = transactionListViewModel;
        }

        public IAccountListViewModel AccountListViewModel { get; }
        public ITransactionListViewModel TransactionListViewModel { get; }
    }
}
