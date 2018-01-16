namespace Financier.Desktop.ViewModels
{
    public class AccountEditViewModel : IAccountEditViewModel
    {
        public AccountEditViewModel(
            IAccountOverviewViewModel accountOverviewViewModel,
            ITransactionListViewModel transactionListViewModel
            
        )
        {
            AccountOverviewViewModel = accountOverviewViewModel;
            TransactionListViewModel = transactionListViewModel;
        }

        public IAccountOverviewViewModel AccountOverviewViewModel { get; }
        public ITransactionListViewModel TransactionListViewModel { get; }
    }
}
