using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Financier.Desktop.Services
{
    public class TransactionBatchCreateConfirmViewService : ITransactionBatchCreateConfirmViewService
    {
        private readonly ILogger<TransactionBatchCreateConfirmViewService> m_logger;
        private readonly IViewModelFactory m_viewModelFactory;

        public TransactionBatchCreateConfirmViewService(ILogger<TransactionBatchCreateConfirmViewService> logger, IViewModelFactory viewModelFactory)
        {
            m_logger = logger;
            m_viewModelFactory = viewModelFactory;
        }

        public bool Show(IEnumerable<Transaction> transactions)
        {
            ITransactionBatchCreateConfirmViewModel viewModel =
                m_viewModelFactory.CreateTransactionBatchCreateConfirmViewModel(transactions);
            var window = new TransactionBatchCreateConfirmWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue)
                return result.Value;

            return false;
        }
    }
}
