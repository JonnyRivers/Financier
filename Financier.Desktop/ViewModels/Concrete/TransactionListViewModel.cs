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
    public class TransactionListViewModel : BaseViewModel, ITransactionListViewModel
    {
        // Dependencies
        private ILogger<AccountListViewModel> m_logger;
        private IAccountService m_accountService;
        private IAccountRelationshipService m_accountRelationshipService;
        private IConversionService m_conversionService;
        private IMessageService m_messageService;
        private ITransactionService m_transactionService;
        private IViewService m_viewService;

        // Private data
        private ObservableCollection<IAccountLinkViewModel> m_accountFilters;
        private IAccountLinkViewModel m_nullAccountFilter;
        private IAccountLinkViewModel m_selectedAccountFilter;
        private bool m_includeLogicalAccounts;
        private bool m_accountFilterHasLogicalAcounts;
        private List<Transaction> m_allTransactions;
        private ObservableCollection<ITransactionItemViewModel> m_transactions;
        private ITransactionItemViewModel m_selectedTransaction;

        public TransactionListViewModel(
            ILogger<AccountListViewModel> logger,
            IAccountService accountService,
            IAccountRelationshipService accountRelationshipService,
            IConversionService conversionService,
            IMessageService messageService,
            ITransactionService transactionService,
            IViewService viewService)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_accountRelationshipService = accountRelationshipService;
            m_conversionService = conversionService;
            m_messageService = messageService;
            m_transactionService = transactionService;
            m_viewService = viewService;

            SetupAccountFilters();

            m_messageService.Register<AccountCreateMessage>(OnAccountCreated);
            m_messageService.Register<AccountUpdateMessage>(OnAccountUpdated);
            m_messageService.Register<TransactionCreateMessage>(OnTransactionCreated);

            SetupTransactions();

            // The set here populates the transactions
            SelectedAccountFilter = m_nullAccountFilter;
        }

        private void SetupAccountFilters()
        {
            IEnumerable<AccountLink> accountLinks = m_accountService.GetAllAsLinks();

            var accountFilters = new List<IAccountLinkViewModel>();
            var nullAccountLink = new AccountLink
            {
                AccountId = 0,
                Name = "(All Accounts)",
                LogicalAccountIds = new int[0]
            };
            m_nullAccountFilter = m_conversionService.AccountLinkToViewModel(nullAccountLink);
            accountFilters.Add(m_nullAccountFilter);
            accountFilters.AddRange(
                accountLinks
                    .OrderBy(a => a.Name)
                    .Select(a => m_conversionService.AccountLinkToViewModel(a))
            );
            AccountFilters = new ObservableCollection<IAccountLinkViewModel>(accountFilters);
            m_includeLogicalAccounts = true;
        }

        private void SetupTransactions()
        {
            m_allTransactions = m_transactionService.GetAll().ToList();
        }

        private IEnumerable<Transaction> GetFilteredTransactions()
        {
            if (SelectedAccountFilter != m_nullAccountFilter)
            {
                var relevantAccountIds = new List<int>();
                relevantAccountIds.Add(SelectedAccountFilter.AccountId);
                if (IncludeLogicalAccounts)
                {
                    relevantAccountIds.AddRange(SelectedAccountFilter.LogicalAccountIds);
                }
                IEnumerable<Transaction> accountTransactions =
                    m_allTransactions
                        .Where(t => relevantAccountIds.Contains(t.CreditAccount.AccountId) ||
                                    relevantAccountIds.Contains(t.DebitAccount.AccountId));

                return accountTransactions;
            }

            IEnumerable<Transaction> recentTransactions = m_allTransactions
                    .OrderByDescending(t => t.TransactionId)
                    .Take(100);

            return recentTransactions;
        }

        private void PopulateTransactionBalances(IEnumerable<ITransactionItemViewModel> transactionViewModels)
        {
            HashSet<int> relevantAccountIds = new HashSet<int>(SelectedAccountFilter.LogicalAccountIds);
            relevantAccountIds.Add(SelectedAccountFilter.AccountId);

            decimal balance = 0;
            foreach (ITransactionItemViewModel transactionViewModel in transactionViewModels)
            {
                if (relevantAccountIds.Contains(transactionViewModel.CreditAccount.AccountId))
                {
                    balance -= transactionViewModel.Amount;
                }
                else if (relevantAccountIds.Contains(transactionViewModel.DebitAccount.AccountId))
                {
                    balance += transactionViewModel.Amount;
                }
                else
                {
                    throw new InvalidOperationException("Encountered unrelated transaction when calculating balance");
                }
                transactionViewModel.Balance = balance;
            }
        }

        private void PopulateTransactions()
        {
            IEnumerable<Transaction> filteredTransactions = GetFilteredTransactions();
            List<ITransactionItemViewModel> transactionVMs =
                filteredTransactions
                    .Select(t => m_conversionService.TransactionToItemViewModel(t))
                    .ToList();
            if (SelectedAccountFilter != m_nullAccountFilter)
            {
                PopulateTransactionBalances(transactionVMs);
            }
            Transactions = new ObservableCollection<ITransactionItemViewModel>(
                transactionVMs.OrderByDescending(t => t.TransactionId));
        }

        public ObservableCollection<IAccountLinkViewModel> AccountFilters
        {
            get { return m_accountFilters; }
            private set
            {
                if (m_accountFilters != value)
                {
                    m_accountFilters = value;
                    
                    OnPropertyChanged();
                }
            }
        }
        public IAccountLinkViewModel SelectedAccountFilter
        {
            get { return m_selectedAccountFilter; }
            set
            {
                if (m_selectedAccountFilter != value)
                {
                    m_selectedAccountFilter = value;

                    AccountFilterHasLogicalAccounts = m_selectedAccountFilter.LogicalAccountIds.Any();
                    OnPropertyChanged();

                    PopulateTransactions();
                }
            }
        }
        public bool IncludeLogicalAccounts
        {
            get { return m_includeLogicalAccounts; }
            set
            {
                if (m_includeLogicalAccounts != value)
                {
                    m_includeLogicalAccounts = value;

                    OnPropertyChanged();

                    PopulateTransactions();
                }
            }
        }
        public bool AccountFilterHasLogicalAccounts
        {
            get { return m_accountFilterHasLogicalAcounts; }
            set
            {
                if (m_accountFilterHasLogicalAcounts != value)
                {
                    m_accountFilterHasLogicalAcounts = value;

                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ITransactionItemViewModel> Transactions
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
        public ITransactionItemViewModel SelectedTransaction
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
        public ICommand EditCommand => new RelayCommand(EditExecute, EditCanExecute);
        public ICommand DeleteCommand => new RelayCommand(DeleteExecute, DeleteCanExecute);

        private void CreateExecute(object obj)
        {
            m_viewService.OpenTransactionCreateView();
            // OnTransactionCreated() is called if a transaction is created
        }

        private void EditExecute(object obj)
        {
            if (m_viewService.OpenTransactionEditView(SelectedTransaction.TransactionId))
            {
                Transaction existingTransaction =
                    m_allTransactions
                        .Single(t => t.TransactionId == SelectedTransaction.TransactionId);
                m_allTransactions.Remove(existingTransaction);

                Transaction updtedTransaction = m_transactionService.Get(SelectedTransaction.TransactionId);
                m_allTransactions.Add(updtedTransaction);

                PopulateTransactions();
            }
        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedTransaction != null);
        }

        private void DeleteExecute(object obj)
        {
            if(m_viewService.OpenTransactionDeleteConfirmationView())
            {
                // Update model
                Transaction transaction = m_transactionService.Get(SelectedTransaction.TransactionId);
                m_transactionService.Delete(SelectedTransaction.TransactionId);
                m_messageService.Send(new TransactionDeleteMessage(transaction));

                // Update view model
                Transactions.Remove(SelectedTransaction);
            }
        }

        private bool DeleteCanExecute(object obj)
        {
            return (SelectedTransaction != null);
        }

        private void OnAccountCreated(AccountCreateMessage message)
        {
            Account newAccount = message.Account;

            var newAccountLink = new AccountLink
            {
                AccountId = newAccount.AccountId,
                Name = newAccount.Name,
                Type = newAccount.Type,
                LogicalAccountIds = newAccount.LogicalAccounts.Select(a => a.AccountId).ToList()
            };

            List<IAccountLinkViewModel> accountFilters = 
                AccountFilters
                    .Where(alvm => alvm != m_nullAccountFilter)
                    .ToList();
            IAccountLinkViewModel newAccountLinkViewModel = 
                m_conversionService.AccountLinkToViewModel(newAccountLink);
            accountFilters.Add(newAccountLinkViewModel);

            var newAccountFilters = new List<IAccountLinkViewModel>();
            newAccountFilters.Add(m_nullAccountFilter);
            newAccountFilters.AddRange(accountFilters.OrderBy(alvm => alvm.Name));
            AccountFilters = new ObservableCollection<IAccountLinkViewModel>(newAccountFilters);
        }

        private void OnAccountUpdated(AccountUpdateMessage message)
        {
            Account updatedAccount = message.Account;

            IAccountLinkViewModel updatedAccountViewModel = 
                AccountFilters
                    .Single(alvm => alvm.AccountId == updatedAccount.AccountId);

            updatedAccountViewModel.LogicalAccountIds = updatedAccount.LogicalAccounts.Select(a => a.AccountId).ToList();
            updatedAccountViewModel.Name = updatedAccount.Name;
            updatedAccountViewModel.Type = updatedAccount.Type;
        }

        private void OnTransactionCreated(TransactionCreateMessage message)
        {
            m_allTransactions.Add(message.Transaction);

            PopulateTransactions();
        }
    }
}
