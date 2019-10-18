using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class ReconcileBalanceViewService : IReconcileBalanceViewService
    {
        private readonly ILogger<ReconcileBalanceViewService> m_logger;
        private readonly IReconcileBalanceViewModelFactory m_reconcileBalanceViewModelFactory;

        public ReconcileBalanceViewService(
            ILogger<ReconcileBalanceViewService> logger,
            IReconcileBalanceViewModelFactory reconcileBalanceViewModelFactory)
        {
            m_logger = logger;
            m_reconcileBalanceViewModelFactory = reconcileBalanceViewModelFactory;
        }

        public bool Show(int accountId, out Transaction transaction)
        {
            transaction = null;

            IReconcileBalanceViewModel viewModel = m_reconcileBalanceViewModelFactory.Create(accountId);
            var window = new ReconcileBalanceWindow(viewModel);
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
