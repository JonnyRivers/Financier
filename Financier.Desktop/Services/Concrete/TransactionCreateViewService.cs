using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class TransactionCreateViewService : ITransactionCreateViewService
    {
        private readonly ILogger<TransactionCreateViewService> m_logger;
        private readonly ITransactionDetailsViewModelFactory m_transactionDetailsViewModelFactory;

        public TransactionCreateViewService(
            ILoggerFactory loggerFactory,
            ITransactionDetailsViewModelFactory transactionDetailsViewModelFactory)
        {
            m_logger = loggerFactory.CreateLogger<TransactionCreateViewService>();
            m_transactionDetailsViewModelFactory = transactionDetailsViewModelFactory;
        }
        public bool Show(Transaction hint, out Transaction transaction)
        {
            transaction = null;

            ITransactionDetailsViewModel viewModel = m_transactionDetailsViewModelFactory.Create(hint);
            var window = new TransactionDetailsWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                transaction = viewModel.ToTransaction();
                return true;
            }

            return false;
        }
    }
}
