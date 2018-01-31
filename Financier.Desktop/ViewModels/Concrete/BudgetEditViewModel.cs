using Financier.Desktop.Commands;
using Financier.Desktop.Services;
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
        private ILogger<BudgetEditViewModel> m_logger;
        private IBudgetService m_budgetService;
        private IViewModelFactory m_viewModelFactory;
        private int m_budgetId;

        public BudgetEditViewModel(
            ILogger<BudgetEditViewModel> logger, 
            IBudgetService budgetService,
            IViewModelFactory viewModelFactory,
            int budgetId)
        {
            m_logger = logger;
            m_budgetService = budgetService;
            m_viewModelFactory = viewModelFactory;

            Periods = Enum.GetValues(typeof(BudgetPeriod)).Cast<BudgetPeriod>();

            m_budgetId = budgetId;

            TransactionListViewModel = m_viewModelFactory.CreateBudgetTransactionListViewModel(m_budgetId);

            if (m_budgetId == 0)
            {
                Name = "New Budget";
                SelectedPeriod = BudgetPeriod.Fortnightly;
            }
            else
            {
                Budget budget = m_budgetService.Get(m_budgetId);

                Name = budget.Name;
                SelectedPeriod = budget.Period;
            }
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
