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
    public class AccountTreeViewModel : BaseViewModel, IAccountTreeViewModel
    {
        private ILogger<AccountTreeViewModel> m_logger;
        private IAccountService m_accountService;
        private IAccountRelationshipService m_accountRelationshipService;
        private ITransactionService m_transactionService;
        private ITransactionRelationshipService m_transactionRelationshipService;
        private IViewModelFactory m_viewModelFactory;
        private IViewService m_viewService;

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

            PopulateAccountTreeItems();
        }

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

            AccountItems = new ObservableCollection<IAccountTreeItemViewModel>(
                physicalAccountVMs.OrderBy(a => a.Name)
            );
        }
    }
}
