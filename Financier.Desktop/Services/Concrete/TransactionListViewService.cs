using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class TransactionListViewService : ITransactionListViewService
    {
        private readonly ILogger<TransactionListViewService> m_logger;
        private readonly ITransactionListViewModelFactory m_transactionListViewModelFactory;

        public TransactionListViewService(
            ILogger<TransactionListViewService> logger, 
            ITransactionListViewModelFactory transactionListViewModelFactory)
        {
            m_logger = logger;
            m_transactionListViewModelFactory = transactionListViewModelFactory;
        }

        public void Show()
        {
            ITransactionListViewModel viewModel = m_transactionListViewModelFactory.Create();
            var window = new TransactionListWindow(viewModel);
            window.Show();
        }
    }
}
