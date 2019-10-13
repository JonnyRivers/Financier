using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class BudgetListViewService : IBudgetListViewService
    {
        private readonly ILogger<BudgetListViewService> m_logger;
        private readonly IBudgetListViewModelFactory m_viewModelFactory;

        public BudgetListViewService(ILogger<BudgetListViewService> logger, IBudgetListViewModelFactory viewModelFactory)
        {
            m_logger = logger;
            m_viewModelFactory = viewModelFactory;
        }

        public void Show()
        {
            IBudgetListViewModel viewModel = m_viewModelFactory.Create();
            var window = new BudgetListWindow(viewModel);
            window.Show();
        }
    }
}
