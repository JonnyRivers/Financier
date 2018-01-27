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
        private IConversionService m_conversionService;
        private IMessageService m_messageService;
        private IViewService m_viewService;

        public AccountListViewModel(
            ILogger<AccountListViewModel> logger,
            IAccountService accountService,
            IConversionService conversionService,
            IMessageService messageService,
            IViewService viewService)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_conversionService = conversionService;
            m_messageService = messageService;
            m_viewService = viewService;

            m_messageService.Register<TransactionCreateMessage>(OnTransactionCreated);
            m_messageService.Register<TransactionDeleteMessage>(OnTransactionDeleted);
            m_messageService.Register<TransactionUpdateMessage>(OnTransactionUpdated);

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

        private void CreateExecute(object obj)
        {
            int newAccountId = m_viewService.OpenAccountCreateView();
            if (newAccountId > 0)
            {
                Account newAccount = m_accountService.Get(newAccountId);
                IAccountItemViewModel newAccountViewModel = m_conversionService.AccountToItemViewModel(newAccount);
                Accounts.Add(newAccountViewModel);
                // TODO: Is there a better way to maintain ObservableCollection<T> sorting?
                // https://github.com/JonnyRivers/Financier/issues/29
                Accounts = new ObservableCollection<IAccountItemViewModel>(Accounts.OrderBy(b => b.Name));
            }
        }

        private void EditExecute(object obj)
        {
            if (m_viewService.OpenAccountEditView(SelectedAccount.AccountId))
            {
                Account account = m_accountService.Get(SelectedAccount.AccountId);
                SelectedAccount.Setup(account);
                // TODO: Is there a better way to maintain ObservableCollection<T> sorting?
                // https://github.com/JonnyRivers/Financier/issues/29
                Accounts = new ObservableCollection<IAccountItemViewModel>(Accounts.OrderBy(b => b.Name));
            }
        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedAccount != null);
        }

        private void PopulateAccounts()
        {
            IEnumerable<Account> accounts = m_accountService.GetAll();
            IEnumerable<IAccountItemViewModel> accountVMs = 
                accounts.Select(a => m_conversionService.AccountToItemViewModel(a));
            Accounts = new ObservableCollection<IAccountItemViewModel>(accountVMs.OrderBy(a => a.Name));
            OnPropertyChanged(nameof(Accounts));
        }

        private void OnTransactionCreated(TransactionCreateMessage message)
        {
            AddTransactionToBalances(message.Transaction);
        }

        private void OnTransactionDeleted(TransactionDeleteMessage message)
        {
            RemoveTransactionFromBalances(message.Transaction);
        }

        private void OnTransactionUpdated(TransactionUpdateMessage message)
        {
            RemoveTransactionFromBalances(message.Before);
            AddTransactionToBalances(message.After);
        }

        private void AddTransactionToBalances(Transaction transaction)
        {
            IAccountItemViewModel creditAccountBefore =
                Accounts
                    .Single(a => a.AccountId == transaction.CreditAccount.AccountId);
            IAccountItemViewModel debitAccountBefore =
                Accounts
                    .Single(a => a.AccountId == transaction.DebitAccount.AccountId);

            creditAccountBefore.Balance -= transaction.Amount;
            debitAccountBefore.Balance += transaction.Amount;
        }

        private void RemoveTransactionFromBalances(Transaction transaction)
        {
            IAccountItemViewModel creditAccountAfter =
                Accounts
                    .Single(a => a.AccountId == transaction.CreditAccount.AccountId);
            IAccountItemViewModel debitAccountAfter =
                Accounts
                    .Single(a => a.AccountId == transaction.DebitAccount.AccountId);

            creditAccountAfter.Balance += transaction.Amount;
            debitAccountAfter.Balance -= transaction.Amount;
        }
    }
}
