﻿using Financier.Desktop.Commands;
using Financier.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class BudgetEditViewModel : IBudgetEditViewModel
    {
        private ILogger<AccountLinkViewModel> m_logger;
        private IBudgetService m_budgetService;
        private int m_budgetId;

        public BudgetEditViewModel(ILogger<AccountLinkViewModel> logger, IBudgetService budgetService)
        {
            m_logger = logger;
            m_budgetService = budgetService;
            
            Periods = Enum.GetValues(typeof(BudgetPeriod)).Cast<BudgetPeriod>();

            TransactionListViewModel =
                IoC.ServiceProvider.Instance.GetRequiredService<IBudgetTransactionListViewModel>();
        }

        public void SetupForCreate()
        {
            m_budgetId = 0;

            Name = "New Budget";
            SelectedPeriod = BudgetPeriod.Fortnightly;
            TransactionListViewModel.SetupForCreate();
        }

        public void SetupForEdit(int budgetId)
        {
            m_budgetId = budgetId;

            Budget budget = m_budgetService.Get(m_budgetId);

            Name = budget.Name;
            SelectedPeriod = budget.Period;
            TransactionListViewModel.SetupForEdit(budget);
        }

        public Budget ToBudget()
        {
            IBudgetTransactionItemViewModel initialTransaction = TransactionListViewModel.Transactions
                .Single(t => t.Type == BudgetTransactionType.Initial);
            IBudgetTransactionItemViewModel surplusTransaction = TransactionListViewModel.Transactions
                .Single(t => t.Type == BudgetTransactionType.Surplus);
            IEnumerable<IBudgetTransactionItemViewModel> regularTransactions =
                TransactionListViewModel.Transactions.Where(t => t.Type == BudgetTransactionType.Regular);

            Budget budget = new Budget
            {
                BudgetId = m_budgetId,
                Name = Name,
                Period = SelectedPeriod,
                InitialTransaction = initialTransaction.ToBudgetTransaction(),
                Transactions = regularTransactions.Select(t => t.ToBudgetTransaction()),
                SurplusTransaction = surplusTransaction.ToBudgetTransaction(),
            };

            return budget;
        }

        public IEnumerable<BudgetPeriod> Periods { get; }

        public int BudgetId
        {
            get { return m_budgetId; }
        }

        public string Name { get; set; }
        public BudgetPeriod SelectedPeriod { get; set; }

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        public IBudgetTransactionListViewModel TransactionListViewModel { get; set; }

        private void OKExecute(object obj)
        {
            Budget budget = this.ToBudget();

            if (m_budgetId != 0)
            {
                m_budgetService.Update(budget);
            }
            else
            {
                m_budgetService.Create(budget);
                m_budgetId = budget.BudgetId;
            }
        }

        private void CancelExecute(object obj)
        {

        }
    }
}
