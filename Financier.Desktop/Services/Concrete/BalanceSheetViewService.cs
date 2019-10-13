using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class BalanceSheetViewService : IBalanceSheetViewService
    {
        private readonly ILogger<BalanceSheetViewService> m_logger;
        private readonly IBalanceSheetViewModelFactory m_viewModelFactory;

        public BalanceSheetViewService(ILogger<BalanceSheetViewService> logger, IBalanceSheetViewModelFactory viewModelFactory)
        {
            m_logger = logger;
            m_viewModelFactory = viewModelFactory;
        }

        public void Show()
        {
            IBalanceSheetViewModel viewModel = m_viewModelFactory.Create();
            var window = new BalanceSheetWindow(viewModel);
            window.Show();
        }
    }
}
