using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class AccountTransactionsEditViewService : IAccountTransactionsEditViewService
    {
        private readonly ILogger<AccountTransactionsEditViewService> m_logger;
        private readonly IAccountTransactionListViewModelFactory m_accountTransactionListViewModelFactory;

        public AccountTransactionsEditViewService(
            ILogger<AccountTransactionsEditViewService> logger, 
            IAccountTransactionListViewModelFactory accountTransactionListViewModelFactory)
        {
            m_logger = logger;
            m_accountTransactionListViewModelFactory = accountTransactionListViewModelFactory;
        }

        public bool Show(int accountId)
        {
            IAccountTransactionListViewModel viewModel = m_accountTransactionListViewModelFactory.Create(accountId);
            var window = new AccountTransactionListWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue)
                return result.Value;

            return false;
        }
    }
}
