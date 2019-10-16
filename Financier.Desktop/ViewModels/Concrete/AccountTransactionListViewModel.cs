using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class AccountTransactionListViewModelFactory : IAccountTransactionListViewModelFactory
    {
        private readonly ILogger<AccountTransactionListViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public AccountTransactionListViewModelFactory(
            ILogger<AccountTransactionListViewModelFactory> logger, 
            IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IAccountTransactionListViewModel Create(int accountId)
        {
            return m_serviceProvider.CreateInstance<AccountTransactionListViewModel>(accountId);
        }
    }

    public class AccountTransactionListViewModel : BaseViewModel, IAccountTransactionListViewModel
    {
        // Dependencies
        private ILogger<AccountTransactionListViewModel> m_logger;
        private IAccountService m_accountService;
        private ITransactionService m_transactionService;
        private IAccountTransactionItemViewModelFactory m_accountTransactionItemViewModelFactory;
        private IDeleteConfirmationViewService m_deleteConfirmationViewService;
        private ITransactionCreateViewService m_transactionCreateViewService;
        private ITransactionEditViewService m_transactionEditViewService;
        private IReconcileBalanceViewService m_reconcileBalanceViewService;

        // Private data
        private int m_accountId;
        private List<int> m_logicalAccountIds;
        private bool m_hasLogicalAccounts;
        private bool m_showLogicalAccounts;
        private ObservableCollection<IAccountTransactionItemViewModel> m_transactions;
        private IAccountTransactionItemViewModel m_selectedTransaction;

        public AccountTransactionListViewModel(
            ILogger<AccountTransactionListViewModel> logger,
            IAccountService accountService,
            ITransactionService transactionService,
            IAccountTransactionItemViewModelFactory accountTransactionItemViewModelFactory,
            IDeleteConfirmationViewService deleteConfirmationViewService,
            ITransactionCreateViewService transactionCreateViewService,
            ITransactionEditViewService transactionEditViewService,
            IReconcileBalanceViewService reconcileBalanceViewService,
            int accountId)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_transactionService = transactionService;
            m_accountTransactionItemViewModelFactory = accountTransactionItemViewModelFactory;
            m_deleteConfirmationViewService = deleteConfirmationViewService;
            m_transactionCreateViewService = transactionCreateViewService;
            m_transactionEditViewService = transactionEditViewService;
            m_reconcileBalanceViewService = reconcileBalanceViewService;

            m_accountId = accountId;
            m_logicalAccountIds = new List<int>(m_accountService.GetLogicalAccountIds(m_accountId));
            m_hasLogicalAccounts = m_logicalAccountIds.Any();
            ShowLogicalAccounts = m_hasLogicalAccounts;

            PopulateTransactions();
        }

        private void PopulateTransactions()
        {
            var relevantAccountIds = new List<int>();
            relevantAccountIds.Add(m_accountId);
            if(m_showLogicalAccounts)
            {
                relevantAccountIds.AddRange(m_logicalAccountIds);
            }

            IEnumerable<Transaction> transactions = m_transactionService.GetAll(relevantAccountIds);
            List<IAccountTransactionItemViewModel> transactionViewModels =
                transactions
                    .Select(t => m_accountTransactionItemViewModelFactory.Create(t))
                    .ToList();
            Transactions = new ObservableCollection<IAccountTransactionItemViewModel>(transactionViewModels);

            PopulateTransactionBalances();
        }

        private void PopulateTransactionBalances()
        {
            var relevantAccountIds = new List<int>();
            relevantAccountIds.Add(m_accountId);
            if (m_showLogicalAccounts)
            {
                relevantAccountIds.AddRange(m_logicalAccountIds);
            }

            decimal balance = 0;
            foreach (IAccountTransactionItemViewModel transactionViewModel in Transactions.OrderBy(t => t.At))
            {
                // For physical<->logical trasactions, we add *and* subtract
                if (relevantAccountIds.Contains(transactionViewModel.DebitAccount.AccountId))
                    balance += transactionViewModel.Amount;
                if (relevantAccountIds.Contains(transactionViewModel.CreditAccount.AccountId))
                    balance -= transactionViewModel.Amount;
                transactionViewModel.Balance = balance;
            }
        }

        public bool HasLogicalAcounts => m_hasLogicalAccounts;

        public bool ShowLogicalAccounts
        {
            get { return m_showLogicalAccounts; }
            set
            {
                if (m_showLogicalAccounts != value)
                {
                    m_showLogicalAccounts = value;

                    OnPropertyChanged();

                    PopulateTransactions();
                }
            }
        }

        public ObservableCollection<IAccountTransactionItemViewModel> Transactions
        {
            get { return m_transactions; }
            set
            {
                if (m_transactions != value)
                {
                    m_transactions = value;

                    OnPropertyChanged();
                }
            }
        }
        public IAccountTransactionItemViewModel SelectedTransaction
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
        public ICommand ReconcileBalanceCommand => new RelayCommand(ReconcileBalanceExecute);
        public ICommand EditCommand => new RelayCommand(EditExecute, EditCanExecute);
        public ICommand DeleteCommand => new RelayCommand(DeleteExecute, DeleteCanExecute);

        private void CreateExecute(object obj)
        {
            Transaction hint = null;
            if (SelectedTransaction != null)
            {
                hint = m_transactionService.Get(SelectedTransaction.TransactionId);
            }
            else if(Transactions.Any())
            {
                hint = m_transactionService.Get(
                    Transactions.OrderByDescending(t => t.At).First().TransactionId);
            }
            else
            {
                AccountLink thisAccountLink = m_accountService.GetAsLink(m_accountId);
                hint = new Transaction
                {
                    CreditAccount = thisAccountLink,
                    DebitAccount = thisAccountLink,
                    Amount = 0,
                    At = DateTime.Now
                };
            }

            Transaction newTransaction;
            if (m_transactionCreateViewService.Show(hint, out newTransaction))
            {
                IAccountTransactionItemViewModel newTransactionViewModel
                    = m_accountTransactionItemViewModelFactory.Create(newTransaction);
                Transactions.Add(newTransactionViewModel);
                PopulateTransactionBalances();
            }
        }

        private void ReconcileBalanceExecute(object obj)
        {
            Transaction newTransaction;
            if (m_reconcileBalanceViewService.Show(m_accountId, out newTransaction))
            {
                Transactions.Add(m_accountTransactionItemViewModelFactory.Create(newTransaction));
                PopulateTransactionBalances();
            }
        }

        private void EditExecute(object obj)
        {
            Transaction updatedTransaction;
            if (m_transactionEditViewService.Show(SelectedTransaction.TransactionId, out updatedTransaction))
            {
                Transactions.Remove(SelectedTransaction);
                SelectedTransaction = m_accountTransactionItemViewModelFactory.Create(updatedTransaction);
                Transactions.Add(SelectedTransaction);
                PopulateTransactionBalances();
            }
        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedTransaction != null);
        }

        private void DeleteExecute(object obj)
        {
            if(m_deleteConfirmationViewService.Show("transaction"))
            {
                // Update model
                m_transactionService.Delete(SelectedTransaction.TransactionId);

                // Update view model
                Transactions.Remove(SelectedTransaction);
                PopulateTransactionBalances();
            }
        }

        private bool DeleteCanExecute(object obj)
        {
            return (SelectedTransaction != null);
        }
    }
}
