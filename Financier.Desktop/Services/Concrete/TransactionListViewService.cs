using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class TransactionListViewService : ITransactionListViewService
    {
        private readonly ILogger<TransactionListViewService> m_logger;
        private readonly ITransactionListViewModelFactory m_viewModelFactory;

        public TransactionListViewService(ILogger<TransactionListViewService> logger, ITransactionListViewModelFactory viewModelFactory)
        {
            m_logger = logger;
            m_viewModelFactory = viewModelFactory;
        }

        public void Show()
        {
            ITransactionListViewModel viewModel = m_viewModelFactory.Create();
            var window = new TransactionListWindow(viewModel);
            window.Show();
        }
    }
}
