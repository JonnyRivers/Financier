using Financier.Desktop.Commands;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class BudgetDetailsViewModelFactory : IBudgetDetailsViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;
        private readonly IBudgetService m_budgetService;
        private readonly IBudgetTransactionListViewModelFactory m_budgetTransactionListViewModelFactory;

        public BudgetDetailsViewModelFactory(
            ILoggerFactory loggerFactory,
            IBudgetService budgetService,
            IBudgetTransactionListViewModelFactory budgetTransactionListViewModelFactory)
        {
            m_loggerFactory = loggerFactory;
            m_budgetService = budgetService;
            m_budgetTransactionListViewModelFactory = budgetTransactionListViewModelFactory;
        }

        public IBudgetDetailsViewModel Create()
        {
            return new BudgetCreateViewModel(
                m_loggerFactory,
                m_budgetService,
                m_budgetTransactionListViewModelFactory);
        }

        public IBudgetDetailsViewModel Create(int budgetId)
        {
            return new BudgetEditViewModel(
                m_loggerFactory,
                m_budgetService,
                m_budgetTransactionListViewModelFactory,
                budgetId);
        }
    }

    public abstract class BudgetDetailsBaseViewModel : IBudgetDetailsViewModel
    {
        protected readonly IBudgetService m_budgetService;
        protected readonly IBudgetTransactionListViewModelFactory m_budgetTransactionListViewModelFactory;
        protected int m_budgetId;

        public BudgetDetailsBaseViewModel(
            IBudgetService budgetService,
            IBudgetTransactionListViewModelFactory budgetTransactionListViewModelFactory,
            int budgetId)
        {
            m_budgetService = budgetService;
            m_budgetTransactionListViewModelFactory = budgetTransactionListViewModelFactory;

            m_budgetId = budgetId;

            Periods = Enum.GetValues(typeof(BudgetPeriod)).Cast<BudgetPeriod>();

            TransactionListViewModel = m_budgetTransactionListViewModelFactory.Create(m_budgetId);
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

    public class BudgetCreateViewModel : BudgetDetailsBaseViewModel
    {
        private ILogger<BudgetCreateViewModel> m_logger;

        public BudgetCreateViewModel(
            ILoggerFactory loggerFactory,
            IBudgetService budgetService,
            IBudgetTransactionListViewModelFactory budgetTransactionListViewModelFactory) : base(budgetService, budgetTransactionListViewModelFactory, 0)
        {
            m_logger = loggerFactory.CreateLogger<BudgetCreateViewModel>();

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

    public class BudgetEditViewModel : BudgetDetailsBaseViewModel
    {
        private ILogger<BudgetEditViewModel> m_logger;

        public BudgetEditViewModel(
            ILoggerFactory loggerFactory,
            IBudgetService budgetService,
            IBudgetTransactionListViewModelFactory budgetTransactionListViewModelFactory,
            int budgetId) : base(budgetService, budgetTransactionListViewModelFactory, budgetId)
        {
            m_logger = loggerFactory.CreateLogger<BudgetEditViewModel>();

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
