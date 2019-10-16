using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class ReconcileBalanceViewService : IReconcileBalanceViewService
    {
        private readonly ILogger<ReconcileBalanceViewService> m_logger;
        private readonly IViewModelFactory m_viewModelFactory;

        public ReconcileBalanceViewService(ILogger<ReconcileBalanceViewService> logger, IViewModelFactory viewModelFactory)
        {
            m_logger = logger;
            m_viewModelFactory = viewModelFactory;
        }

        public bool Show(int accountId, out Transaction transaction)
        {
            transaction = null;

            IReconcileBalanceViewModel viewModel = m_viewModelFactory.CreateReconcileBalanceViewModel(accountId);
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
