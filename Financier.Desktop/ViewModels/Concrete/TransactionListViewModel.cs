using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class TransactionListViewModel : BaseViewModel, ITransactionListViewModel
    {
        private ILogger<AccountListViewModel> m_logger;
        private IAccountService m_accountService;
        private IAccountRelationshipService m_accountRelationshipService;
        private ITransactionService m_transactionService;
        private IViewService m_viewService;

        public TransactionListViewModel(
            ILogger<AccountListViewModel> logger, 
            IAccountService accountService,
            IAccountRelationshipService accountRelationshipService,
            ITransactionService transactionService,
            IViewService viewService)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_accountRelationshipService = accountRelationshipService;
            m_transactionService = transactionService;
            m_viewService = viewService;

            IEnumerable<Account> accounts = m_accountService.GetAll();

            var accountFilters = new List<ITransactionAccountFilterViewModel>();
            m_nullAccountFilter = new TransactionAccountFilterViewModel(0, "(All Accounts)", false);
            accountFilters.Add(m_nullAccountFilter);
            accountFilters.AddRange(
                accounts
                    .OrderBy(a => a.Name)
                    .Select(a =>
                        new TransactionAccountFilterViewModel(
                            a.AccountId,
                            a.Name,
                            a.LogicalAccounts.Any()))
            );
            AccountFilters = accountFilters;
            SelectedAccountFilter = m_nullAccountFilter;
            m_includeLogicalAccounts = true;

            PopulateTransactions();
        }

        private ITransactionAccountFilterViewModel m_nullAccountFilter;
        private ITransactionAccountFilterViewModel m_selectedAccountFilter;
        private bool m_includeLogicalAccounts;
        private bool m_accountFilterHasLogicalAcounts;
        private ITransactionItemViewModel m_selectedTransaction;

        public IEnumerable<ITransactionAccountFilterViewModel> AccountFilters { get; }
        public ITransactionAccountFilterViewModel SelectedAccountFilter
        {
            get { return m_selectedAccountFilter; }
            set
            {
                if (m_selectedAccountFilter != value)
                {
                    m_selectedAccountFilter = value;

                    AccountFilterHasLogicalAccounts = m_selectedAccountFilter.HasLogicalAccounts;
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

        public ObservableCollection<ITransactionItemViewModel> Transactions { get; set; }
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
            if (m_viewService.OpenTransactionCreateView())
            {
                PopulateTransactions();
            }
        }

        private void EditExecute(object obj)
        {
            if (m_viewService.OpenTransactionEditView(SelectedTransaction.TransactionId))
            {
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
                m_transactionService.Delete(SelectedTransaction.TransactionId);

                PopulateTransactions();
            }
        }

        private bool DeleteCanExecute(object obj)
        {
            return (SelectedTransaction != null);
        }

        private void PopulateTransactions()
        {
            if (SelectedAccountFilter == m_nullAccountFilter)
            {
                IEnumerable<Transaction> transactions = m_transactionService
                    .GetAll()
                    .OrderByDescending(t => t.TransactionId)
                    .Take(100);
                IEnumerable<ITransactionItemViewModel> transactionVMs = transactions
                    .Select(t =>
                        new TransactionItemViewModel(
                            t.TransactionId,
                            t.CreditAccount.Name,
                            t.DebitAccount.Name,
                            t.At,
                            t.Amount,
                            0));
                Transactions = new ObservableCollection<ITransactionItemViewModel>(transactionVMs);
            }
            else
            {
                IEnumerable<Transaction> transactions = m_transactionService
                    .GetAll(SelectedAccountFilter.AccountId, IncludeLogicalAccounts);
                
                var itemVMs = new List<ITransactionItemViewModel>();
                decimal balance = 0;
                foreach(Transaction transaction in transactions)
                {
                    if (transaction.CreditAccount.AccountId == SelectedAccountFilter.AccountId)
                    {
                        balance -= transaction.Amount;
                    }
                    else
                    {
                        balance += transaction.Amount;
                    }
                    itemVMs.Add(
                        new TransactionItemViewModel(
                            transaction.TransactionId,
                            transaction.CreditAccount.Name,
                            transaction.DebitAccount.Name,
                            transaction.At,
                            transaction.Amount,
                            balance)
                    );
                }
                Transactions = new ObservableCollection<ITransactionItemViewModel>(
                    itemVMs.OrderByDescending(t => t.TransactionId)
                );
            }
            
            OnPropertyChanged(nameof(Transactions));
        }
    }
}
