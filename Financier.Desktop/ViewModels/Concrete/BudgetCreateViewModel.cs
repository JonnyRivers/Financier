using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class BudgetCreateViewModel : BudgetDetailsBaseViewModel
    {
        private ILogger<BudgetCreateViewModel> m_logger;

        public BudgetCreateViewModel(
            ILogger<BudgetCreateViewModel> logger, 
            IBudgetService budgetService,
            IViewModelFactory viewModelFactory) : base(budgetService, viewModelFactory, 0)
        {
            m_logger = logger;

            Name = "New Budget";
            SelectedPeriod = BudgetPeriod.Fortnightly;
        }

        protected override void OKExecute(object obj)
        {
            Budget budget = ToBudget();

            m_budgetService.Create(budget);
            m_budgetId = budget.BudgetId;
        }
    }
}
