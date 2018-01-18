using Financier.Data;
using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class TransactionListViewModel : BaseViewModel, ITransactionListViewModel
    {
        private ILogger<AccountListViewModel> m_logger;
        private FinancierDbContext m_dbContext;
        private IViewService m_viewService;

        public TransactionListViewModel(ILogger<AccountListViewModel> logger, FinancierDbContext dbContext, IViewService viewService)
        {
            m_logger = logger;
            m_dbContext = dbContext;
            m_viewService = viewService;

            List<Account> accounts = m_dbContext.Accounts.ToList();
            HashSet<int> accountIdsWithLogicalRelationships = new HashSet<int>(
                m_dbContext.AccountRelationships
                    .Where(ar => ar.Type == AccountRelationshipType.PhysicalToLogical)
                    .Select(ar => ar.SourceAccountId)
            );

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
                            accountIdsWithLogicalRelationships.Contains(a.AccountId)))
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
                Transaction transaction = m_dbContext.Transactions.Single(t => t.TransactionId == SelectedTransaction.TransactionId);
                m_dbContext.Transactions.Remove(transaction);
                m_dbContext.SaveChanges();

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
                IEnumerable<ITransactionItemViewModel> transactionVMs = m_dbContext.Transactions
                .OrderByDescending(t => t.TransactionId)
                .Take(100)
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
                var relevantAccountIds = new HashSet<int>();
                relevantAccountIds.Add(SelectedAccountFilter.AccountId);
                if (AccountFilterHasLogicalAccounts && IncludeLogicalAccounts)
                {
                    IEnumerable<int> logicalAccountIds = m_dbContext.AccountRelationships
                        .Where(r => r.SourceAccountId == SelectedAccountFilter.AccountId &&
                                    r.Type == AccountRelationshipType.PhysicalToLogical)
                        .Select(r => r.DestinationAccountId);
                    foreach (int logicalAccountId in logicalAccountIds)
                        relevantAccountIds.Add(logicalAccountId);
                }

                List<Transaction> transactions = m_dbContext.Transactions
                    .Where(t => relevantAccountIds.Contains(t.CreditAccountId) ||
                                relevantAccountIds.Contains(t.DebitAccountId))
                    .OrderBy(t => t.TransactionId)
                    .ToList();
                var itemVMs = new List<ITransactionItemViewModel>();
                decimal balance = 0;
                foreach(Transaction transaction in transactions)
                {
                    if (transaction.CreditAccountId == SelectedAccountFilter.AccountId)
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
