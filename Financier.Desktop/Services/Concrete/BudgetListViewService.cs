﻿using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class BudgetListViewService : IBudgetListViewService
    {
        private readonly ILogger<BudgetListViewService> m_logger;
        private readonly IBudgetListViewModelFactory m_budgetListViewModelFactory;

        public BudgetListViewService(ILoggerFactory loggerFactory, IBudgetListViewModelFactory budgetListViewModelFactory)
        {
            m_logger = loggerFactory.CreateLogger<BudgetListViewService>();
            m_budgetListViewModelFactory = budgetListViewModelFactory;
        }

        public void Show()
        {
            IBudgetListViewModel viewModel = m_budgetListViewModelFactory.Create();
            var window = new BudgetListWindow(viewModel);
            window.Show();
        }
    }
}
