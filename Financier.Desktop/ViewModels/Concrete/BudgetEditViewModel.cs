using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class BudgetEditViewModel : BudgetDetailsBaseViewModel
    {
        private ILogger<BudgetEditViewModel> m_logger;

        public BudgetEditViewModel(
            ILogger<BudgetEditViewModel> logger, 
            IBudgetService budgetService,
            IViewModelFactory viewModelFactory,
            int budgetId) : base(budgetService, viewModelFactory, budgetId)
        {
            m_logger = logger;
            
            Budget budget = m_budgetService.Get(m_budgetId);

            Name = budget.Name;
            SelectedPeriod = budget.Period;
        }

        protected override void OKExecute(object obj)
        {
            Budget budget = this.ToBudget();

            m_budgetService.Update(budget);
        }
    }
}
