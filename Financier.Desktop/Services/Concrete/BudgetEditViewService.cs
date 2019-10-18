using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class BudgetEditViewService : IBudgetEditViewService
    {
        private readonly ILogger<BudgetEditViewService> m_logger;
        private readonly IBudgetDetailsViewModelFactory m_budgetDetailsViewModelFactory;

        public BudgetEditViewService(ILogger<BudgetEditViewService> logger, IBudgetDetailsViewModelFactory budgetDetailsViewModelFactory)
        {
            m_logger = logger;
            m_budgetDetailsViewModelFactory = budgetDetailsViewModelFactory;
        }

        public bool Show(int budgetId, out Budget budget)
        {
            budget = null;

            IBudgetDetailsViewModel viewModel = m_budgetDetailsViewModelFactory.Create(budgetId);
            var window = new BudgetDetailsWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                budget = viewModel.ToBudget();
                return true;
            }

            return false;
        }
    }
}
