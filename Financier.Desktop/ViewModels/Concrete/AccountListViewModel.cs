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
    public class AccountListViewModel : BaseViewModel, IAccountListViewModel
    {
        private ILogger<AccountListViewModel> m_logger;
        private IAccountService m_accountService;
        private ITransactionService m_transactionService;
        private ITransactionRelationshipService m_transactionRelationshipService;
        private IViewModelFactory m_viewModelFactory;
        private IViewService m_viewService;

        public AccountListViewModel(
            ILogger<AccountListViewModel> logger,
            IAccountService accountService,
            ITransactionService transactionService,
            ITransactionRelationshipService transactionRelationshipService,
            IViewModelFactory viewModelFactory,
            IViewService viewService)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_transactionService = transactionService;
            m_transactionRelationshipService = transactionRelationshipService;
            m_viewModelFactory = viewModelFactory;
            m_viewService = viewService;

            PopulateAccounts();
        }

        private ObservableCollection<IAccountItemViewModel> m_accounts;
        private IAccountItemViewModel m_selectedAccount;

        public ObservableCollection<IAccountItemViewModel> Accounts
        {
            get { return m_accounts; }
            set
            {
                if (m_accounts != value)
                {
                    m_accounts = value;

                    OnPropertyChanged();
                }
            }
        }

        public IAccountItemViewModel SelectedAccount
        {
            get { return m_selectedAccount; }
            set
            {
                if (m_selectedAccount != value)
                {
                    m_selectedAccount = value;

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
                IAccountItemViewModel newAccountViewModel = m_viewModelFactory.CreateAccountItemViewModel(newAccount);
                Accounts.Add(newAccountViewModel);
                // TODO: Is there a better way to maintain ObservableCollection<T> sorting?
                // https://github.com/JonnyRivers/Financier/issues/29
                Accounts = new ObservableCollection<IAccountItemViewModel>(Accounts.OrderBy(b => b.Name));
            }
        }

        private void EditExecute(object obj)
        {
            Account updatedAccount;
            if (m_viewService.OpenAccountEditView(SelectedAccount.AccountId, out updatedAccount))
            {
                Accounts.Remove(SelectedAccount);
                SelectedAccount = m_viewModelFactory.CreateAccountItemViewModel(updatedAccount);
                Accounts.Add(SelectedAccount);

                // TODO: Is there a better way to maintain ObservableCollection<T> sorting?
                // https://github.com/JonnyRivers/Financier/issues/29
                Accounts = new ObservableCollection<IAccountItemViewModel>(Accounts.OrderBy(b => b.Name));
            }
        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedAccount != null);
        }

        private void EditTransactionsExecute(object obj)
        {
            m_viewService.OpenAccountTransactionsEditView(SelectedAccount.AccountId);
        }

        private bool EditTransactionsCanExecute(object obj)
        {
            return (SelectedAccount != null);
        }

        private void PayCreditCardExecute(object obj)
        {
            IEnumerable<Payment> pendingCreditCardPayments = 
                m_transactionService.GetPendingCreditCardPayments(SelectedAccount.AccountId);

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
                m_viewService.OpenNoPendingCreditCardTransactionsView(SelectedAccount.Name);
            }
        }

        private bool PayCreditCardCanExecute(object obj)
        {
            return (SelectedAccount != null && SelectedAccount.SubType == AccountSubType.CreditCard);
        }

        private void PopulateAccounts()
        {
            IEnumerable<Account> accounts = m_accountService.GetAll();
            IEnumerable<IAccountItemViewModel> accountVMs = 
                accounts.Select(a => m_viewModelFactory.CreateAccountItemViewModel(a));
            Accounts = new ObservableCollection<IAccountItemViewModel>(accountVMs.OrderBy(a => a.Name));
            OnPropertyChanged(nameof(Accounts));
        }
    }
}
