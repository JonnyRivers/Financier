using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class MainWindowViewModel : BaseViewModel, IMainWindowViewModel
    {
        private ILogger<AccountListViewModel> m_logger;

        public MainWindowViewModel(
            ILogger<AccountListViewModel> logger,
            IAccountListViewModel accountListViewModel,
            ITransactionListViewModel transactionListViewModel
        )
        {
            m_logger = logger;

            AccountListViewModel = accountListViewModel;
            TransactionListViewModel = transactionListViewModel;
        }

        public IAccountListViewModel AccountListViewModel { get; }
        public ITransactionListViewModel TransactionListViewModel { get; }
    }
}
