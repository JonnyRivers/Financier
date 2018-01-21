using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class BudgetTransactionListViewModel : BaseViewModel, IBudgetTransactionListViewModel
    {
        private ILogger<BudgetTransactionListViewModel> m_logger;
        private IBudgetService m_budgetService;
        private IViewService m_viewService;

        private int m_budgetId;
        private IBudgetTransactionItemViewModel m_selectedTransaction;

        public BudgetTransactionListViewModel(
            ILogger<BudgetTransactionListViewModel> logger, 
            IBudgetService budgetService,
            IViewService viewService)
        {
            m_logger = logger;
            m_budgetService = budgetService;
            m_viewService = viewService;

            // TODO: this is madness - this gets thrown away & is invalid with no data
            // We need the account service to solve this
            var transactions = new List<IBudgetTransactionItemViewModel>();
            BudgetTransaction initialTransaction = new BudgetTransaction
            {
                CreditAccount = new AccountLink { AccountId = 1 },
                DebitAccount = new AccountLink { AccountId = 2 },
                Amount = 0
            };
            BudgetTransaction surplusTransaction = new BudgetTransaction
            {
                CreditAccount = new AccountLink { AccountId = 2 },
                DebitAccount = new AccountLink { AccountId = 3 },
                Amount = 0
            };
            transactions.Add(CreateItemViewModel(initialTransaction, BudgetTransactionType.Initial));
            transactions.Add(CreateItemViewModel(surplusTransaction, BudgetTransactionType.Surplus));

            Transactions = new ObservableCollection<IBudgetTransactionItemViewModel>(transactions);
        }

        public void Setup(Budget budget)
        {
            m_budgetId = budget.BudgetId;

            var transactions = new List<IBudgetTransactionItemViewModel>();
            transactions.Add(CreateItemViewModel(budget.InitialTransaction, BudgetTransactionType.Initial));
            transactions.AddRange(
                budget.Transactions.Select(t => CreateItemViewModel(t, BudgetTransactionType.Regular))
            );
            transactions.Add(CreateItemViewModel(budget.SurplusTransaction, BudgetTransactionType.Surplus));

            Transactions = new ObservableCollection<IBudgetTransactionItemViewModel>(transactions);
        }

        public ObservableCollection<IBudgetTransactionItemViewModel> Transactions { get; set; }
        public IBudgetTransactionItemViewModel SelectedTransaction
        {
            get { return m_selectedTransaction; }
            set
            {
                if (m_selectedTransaction != value)
                {
                    m_selectedTransaction = value;

                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand CreateCommand => new RelayCommand(CreateExecute);
        public ICommand DeleteCommand => new RelayCommand(DeleteExecute, DeleteCanExecute);

        private void CreateExecute(object obj)
        {
            IBudgetTransactionItemViewModel initialTransactionViewModel = 
                Transactions
                    .Single(t => t.Type == BudgetTransactionType.Initial);

            var newTransaction = new BudgetTransaction
            {
                CreditAccount = initialTransactionViewModel.SelectedDebitAccount.ToAccountLink(),
                // TODO: a sensible DebitAccount?  First account with a prepayment to expense link? Fallback?
                DebitAccount = initialTransactionViewModel.SelectedCreditAccount.ToAccountLink(),
                Amount = 0m
            };

            Transactions.Insert(
                Transactions.Count - 1,// we maintain that the surplus transaction is last, so - 1
                CreateItemViewModel(newTransaction, BudgetTransactionType.Regular)
            );
        }

        private void DeleteExecute(object obj)
        {
            if (m_viewService.OpenBudgetDeleteConfirmationView())
            {
                Transactions.Remove(SelectedTransaction);
                SelectedTransaction = null;// is this necessary?
            }
        }

        private bool DeleteCanExecute(object obj)
        {
            return (
                SelectedTransaction != null && 
                SelectedTransaction.Type == BudgetTransactionType.Regular
            );
        }

        private static IBudgetTransactionItemViewModel CreateItemViewModel(BudgetTransaction budgetTransaction, BudgetTransactionType type)
        {
            IBudgetTransactionItemViewModel transactionViewModel = IoC.ServiceProvider.Instance.GetRequiredService<IBudgetTransactionItemViewModel>();
            transactionViewModel.Setup(budgetTransaction, type);

            return transactionViewModel;
        }
    }
}
