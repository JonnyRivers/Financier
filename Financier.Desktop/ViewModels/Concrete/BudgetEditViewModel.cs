using Financier.Desktop.Commands;
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
            m_budgetId = 0;

            Periods = Enum.GetValues(typeof(BudgetPeriod)).Cast<BudgetPeriod>();

            Name = "New Budget";
            SelectedPeriod = BudgetPeriod.Fortnightly;

            TransactionListViewModel = IoC.ServiceProvider.Instance.GetRequiredService<IBudgetTransactionListViewModel>();
        }

        public IEnumerable<BudgetPeriod> Periods { get; }

        // TODO: Account and Transaction VMs set the ID on sub-VMs, where Budgets use a Setup() routine
        // https://github.com/JonnyRivers/Financier/issues/9
        public int BudgetId
        {
            get { return m_budgetId; }
            set
            {
                if (value != m_budgetId)
                {
                    m_budgetId = value;

                    Budget budget = m_budgetService.Get(m_budgetId);
                    Name = budget.Name;
                    SelectedPeriod = budget.Period;

                    TransactionListViewModel.Setup(budget);
                }
            }
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
            }
        }

        private void CancelExecute(object obj)
        {

        }

        private Budget ToBudget()
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
    }
}
