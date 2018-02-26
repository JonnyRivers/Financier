using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public abstract class BudgetDetailsBaseViewModel : IBudgetDetailsViewModel
    {
        protected readonly IBudgetService m_budgetService;
        protected readonly IViewModelFactory m_viewModelFactory;
        protected int m_budgetId;

        public BudgetDetailsBaseViewModel(
            IBudgetService budgetService,
            IViewModelFactory viewModelFactory,
            int budgetId)
        {
            m_budgetService = budgetService;
            m_viewModelFactory = viewModelFactory;

            m_budgetId = budgetId;

            Periods = Enum.GetValues(typeof(BudgetPeriod)).Cast<BudgetPeriod>();

            TransactionListViewModel = m_viewModelFactory.CreateBudgetTransactionListViewModel(m_budgetId);
        }

        public IEnumerable<BudgetPeriod> Periods { get; }

        public string Name { get; set; }
        public BudgetPeriod SelectedPeriod { get; set; }

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

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        public IBudgetTransactionListViewModel TransactionListViewModel { get; set; }

        protected abstract void OKExecute(object obj);

        private void CancelExecute(object obj)
        {

        }
    }
}
