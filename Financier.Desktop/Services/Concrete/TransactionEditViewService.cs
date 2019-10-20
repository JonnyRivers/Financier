using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class TransactionEditViewService : ITransactionEditViewService
    {
        private readonly ILogger<TransactionEditViewService> m_logger;
        private readonly ITransactionDetailsViewModelFactory m_transactionDetailsViewModelFactory;

        public TransactionEditViewService(
            ILoggerFactory loggerFactory,
            ITransactionDetailsViewModelFactory transactionDetailsViewModelFactory)
        {
            m_logger = loggerFactory.CreateLogger<TransactionEditViewService>();
            m_transactionDetailsViewModelFactory = transactionDetailsViewModelFactory;
        }

        public bool Show(int transactionId, out Transaction updatedTransaction)
        {
            updatedTransaction = null;

            ITransactionDetailsViewModel viewModel = m_transactionDetailsViewModelFactory.Create(transactionId);
            var window = new TransactionDetailsWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue)
            {
                updatedTransaction = viewModel.ToTransaction();
                return result.Value;
            }

            return false;
        }
    }
}
