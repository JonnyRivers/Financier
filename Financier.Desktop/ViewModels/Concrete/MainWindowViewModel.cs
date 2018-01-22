using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class MainWindowViewModel : BaseViewModel, IMainWindowViewModel
    {
        private ILogger<AccountListViewModel> m_logger;

        public MainWindowViewModel(
            ILogger<AccountListViewModel> logger,
            IAccountListViewModel accountListViewModel,
            IBudgetListViewModel budgetListViewModel,
            ITransactionListViewModel transactionListViewModel
        )
        {
            m_logger = logger;

            AccountListViewModel = accountListViewModel;
            BudgetListViewModel = budgetListViewModel;
            TransactionListViewModel = transactionListViewModel;
        }

        public IAccountListViewModel AccountListViewModel { get; }
        public IBudgetListViewModel BudgetListViewModel { get; }
        public ITransactionListViewModel TransactionListViewModel { get; }
    }
}
