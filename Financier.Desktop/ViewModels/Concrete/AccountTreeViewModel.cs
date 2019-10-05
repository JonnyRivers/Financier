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
    public class AccountTreeViewModelFactory : IAccountTreeViewModelFactory
    {
        private readonly ILogger<AccountRelationshipListViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public AccountTreeViewModelFactory(ILogger<AccountRelationshipListViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IAccountTreeViewModel Create()
        {
            return m_serviceProvider.CreateInstance<AccountTreeViewModel>();
        }
    }

    public class AccountTreeViewModel : BaseViewModel, IAccountTreeViewModel
    {
        private ILogger<AccountTreeViewModel> m_logger;
        private IAccountService m_accountService;
        private IAccountRelationshipService m_accountRelationshipService;
        private ITransactionService m_transactionService;
        private ITransactionRelationshipService m_transactionRelationshipService;
        private IViewModelFactory m_viewModelFactory;
        private IViewService m_viewService;

        private bool m_showAssets;
        private bool m_showLiabilities;
        private bool m_showIncome;
        private bool m_showExpenses;
        private bool m_showCapital;

        public AccountTreeViewModel(
            ILogger<AccountTreeViewModel> logger,
            IAccountService accountService,
            IAccountRelationshipService accountRelationshipService,
            ITransactionService transactionService,
            ITransactionRelationshipService transactionRelationshipService,
            IViewModelFactory viewModelFactory,
            IViewService viewService)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_accountRelationshipService = accountRelationshipService;
            m_transactionService = transactionService;
            m_transactionRelationshipService = transactionRelationshipService;
            m_viewModelFactory = viewModelFactory;
            m_viewService = viewService;

            m_showAssets = true;
            m_showLiabilities = true;
            m_showIncome = false;
            m_showExpenses = false;
            m_showCapital = false;

            PopulateAccountTreeItems();
        }

        private List<IAccountTreeItemViewModel> m_unfilteredAccountsItems;
        private ObservableCollection<IAccountTreeItemViewModel> m_accountItems;
        private IAccountTreeItemViewModel m_selectedItem;

        public ObservableCollection<IAccountTreeItemViewModel> AccountItems
        {
            get { return m_accountItems; }
            set
            {
                if (m_accountItems != value)
                {
                    m_accountItems = value;

                    OnPropertyChanged();
                }
            }
        }

        public IAccountTreeItemViewModel SelectedItem
        {
            get { return m_selectedItem; }
            set
            {
                if (m_selectedItem != value)
                {
                    m_selectedItem = value;

                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand CreateCommand => new RelayCommand(CreateExecute);
        public ICommand EditCommand => new RelayCommand(EditExecute, EditCanExecute);
        public ICommand EditTransactionsCommand => new RelayCommand(EditTransactionsExecute, EditTransactionsCanExecute);
        public ICommand PayCreditCardCommand => new RelayCommand(PayCreditCardExecute, PayCreditCardCanExecute);

        public bool ShowAssets
        {
            get { return m_showAssets; }
            set
            {
                if (m_showAssets != value)
                {
                    m_showAssets = value;

                    FilterAccounts();
                }
            }
        }
        public bool ShowLiabilities
        {
            get { return m_showLiabilities; }
            set
            {
                if (m_showLiabilities != value)
                {
                    m_showLiabilities = value;

                    FilterAccounts();
                }
            }
        }
        public bool ShowIncome
        {
            get { return m_showIncome; }
            set
            {
                if (m_showIncome != value)
                {
                    m_showIncome = value;

                    FilterAccounts();
                }
            }
        }
        public bool ShowExpenses
        {
            get { return m_showExpenses; }
            set
            {
                if (m_showExpenses != value)
                {
                    m_showExpenses = value;

                    FilterAccounts();
                }
            }
        }
        public bool ShowCapital
        {
            get { return m_showCapital; }
            set
            {
                if (m_showCapital != value)
                {
                    m_showCapital = value;

                    FilterAccounts();
                }
            }
        }

        private void CreateExecute(object obj)
        {
            Account newAccount;
            if (m_viewService.OpenAccountCreateView(out newAccount))
            {
                PopulateAccountTreeItems();
            }
        }

        private void EditExecute(object obj)
        {
            Account updatedAccount;
            if (m_viewService.OpenAccountEditView(SelectedItem.AccountId, out updatedAccount))
            {
                PopulateAccountTreeItems();
            }
        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedItem != null);
        }

        private void EditTransactionsExecute(object obj)
        {
            m_viewService.OpenAccountTransactionsEditView(SelectedItem.AccountId);

            PopulateAccountTreeItems();
        }

        private bool EditTransactionsCanExecute(object obj)
        {
            return (SelectedItem != null);
        }

        private void PayCreditCardExecute(object obj)
        {
            IEnumerable<Payment> pendingCreditCardPayments =
                m_transactionService.GetPendingCreditCardPayments(SelectedItem.AccountId);

            if (pendingCreditCardPayments.Any())
            {
                IEnumerable<Transaction> paymentTransactions = pendingCreditCardPayments.Select(p => p.PaymentTransaction);
                if (m_viewService.OpenTransactionBatchCreateConfirmView(paymentTransactions))
                {
                    m_transactionService.CreateMany(paymentTransactions);

                    var newTransactionRelationships = new List<TransactionRelationship>();
                    foreach (Payment pendingCreditCardPayment in pendingCreditCardPayments)
                    {
                        var transactionRelationship = new TransactionRelationship
                        {
                            SourceTransaction = pendingCreditCardPayment.OriginalTransaction,
                            DestinationTransaction = pendingCreditCardPayment.PaymentTransaction,
                            Type = TransactionRelationshipType.CreditCardPayment
                        };

                        newTransactionRelationships.Add(transactionRelationship);
                    }

                    m_transactionRelationshipService.CreateMany(newTransactionRelationships);
                }

                PopulateAccountTreeItems();
            }
            else
            {
                m_viewService.OpenNoPendingCreditCardTransactionsView(SelectedItem.Name);
            }
        }

        private bool PayCreditCardCanExecute(object obj)
        {
            return (SelectedItem != null && SelectedItem.SubType == AccountSubType.CreditCard);
        }

        private void PopulateAccountTreeItems()
        {
            IEnumerable<AccountRelationship> accountRelationships = m_accountRelationshipService.GetAll();
            
            IDictionary<int, List<int>> logicalAccountIdsByParentAccountId = 
                accountRelationships
                    .Where(ar => ar.Type == AccountRelationshipType.PhysicalToLogical)
                    .GroupBy(ar => ar.SourceAccount.AccountId)
                    .ToDictionary(
                        g => g.Key, 
                        g => new List<int>(g.Select(ar => ar.DestinationAccount.AccountId)));
            var logicalAccountIds = new HashSet<int>(
                accountRelationships
                    .Where(ar => ar.Type == AccountRelationshipType.PhysicalToLogical)
                    .Select(ar => ar.DestinationAccount.AccountId));

            IEnumerable<Account> accounts = m_accountService.GetAll();
            IEnumerable<Transaction> transactions = m_transactionService.GetAll();

            IDictionary<int, IAccountTreeItemViewModel> logicalAccountVMsById =
                accounts
                    .Where(a => logicalAccountIds.Contains(a.AccountId))
                    .ToDictionary(
                        a => a.AccountId,
                        a => m_viewModelFactory.CreateAccountTreeItemViewModel(a, transactions));

            var physicalAccountVMs = new List<IAccountTreeItemViewModel>();
            foreach(Account physicalAccount in accounts.Where(a => !logicalAccountIds.Contains(a.AccountId)))
            {
                if (logicalAccountIdsByParentAccountId.ContainsKey(physicalAccount.AccountId))
                {
                    List<int> childAccountIds = logicalAccountIdsByParentAccountId[physicalAccount.AccountId];
                    IEnumerable<IAccountTreeItemViewModel> childAccountVMs =
                        childAccountIds
                            .Select(id => logicalAccountVMsById[id])
                            .OrderBy(a => a.Name);
                    physicalAccountVMs.Add(m_viewModelFactory.CreateAccountTreeItemViewModel(physicalAccount, transactions, childAccountVMs));
                }
                else
                {
                    physicalAccountVMs.Add(m_viewModelFactory.CreateAccountTreeItemViewModel(physicalAccount, transactions));
                }
            }

            m_unfilteredAccountsItems = physicalAccountVMs.OrderBy(a => a.Name).ToList();

            FilterAccounts();
        }

        private void FilterAccounts()
        {
            var visibleAccountTypes = new HashSet<AccountType>();
            if (ShowAssets)
                visibleAccountTypes.Add(AccountType.Asset);
            if (ShowLiabilities)
                visibleAccountTypes.Add(AccountType.Liability);
            if (ShowIncome)
                visibleAccountTypes.Add(AccountType.Income);
            if (ShowExpenses)
                visibleAccountTypes.Add(AccountType.Expense);
            if (ShowCapital)
                visibleAccountTypes.Add(AccountType.Capital);

            AccountItems = new ObservableCollection<IAccountTreeItemViewModel>(
                m_unfilteredAccountsItems.Where(a => visibleAccountTypes.Contains(a.Type))
            );
        }
    }
}
