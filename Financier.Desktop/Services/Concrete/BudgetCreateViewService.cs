using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Windows;

namespace Financier.Desktop.Services
{
    public class BudgetCreateViewService : IBudgetCreateViewService
    {
        private readonly ILogger<BudgetCreateViewService> m_logger;
        private readonly IBudgetDetailsViewModelFactory m_budgetDetailsViewModelFactory;

        public BudgetCreateViewService(ILogger<BudgetCreateViewService> logger, IBudgetDetailsViewModelFactory budgetDetailsViewModelFactory)
        {
            m_logger = logger;
            m_budgetDetailsViewModelFactory = budgetDetailsViewModelFactory;
        }

        public bool Show(out Budget budget)
        {
            budget = null;

            IBudgetDetailsViewModel viewModel = m_budgetDetailsViewModelFactory.Create();
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
