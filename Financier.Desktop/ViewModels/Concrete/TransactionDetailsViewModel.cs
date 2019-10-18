using Financier.Desktop.Commands;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class TransactionDetailsViewModelFactory : ITransactionDetailsViewModelFactory
    {
        private readonly ILogger<TransactionDetailsViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public TransactionDetailsViewModelFactory(
            ILogger<TransactionDetailsViewModelFactory> logger,
            IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public ITransactionDetailsViewModel Create(Transaction hint)
        {
            return m_serviceProvider.CreateInstance<TransactionCreateViewModel>(hint);
        }

        public ITransactionDetailsViewModel Create(int transactionId)
        {
            return m_serviceProvider.CreateInstance<TransactionEditViewModel>(transactionId);
        }
    }

    public abstract class TransactionDetailsBaseViewModel : BaseViewModel, ITransactionDetailsViewModel
    {
        protected IAccountService m_accountService;
        protected ITransactionService m_transactionService;
        protected IAccountLinkViewModelFactory m_accountLinkViewModelFactory;

        protected int m_transactionId;

        protected TransactionDetailsBaseViewModel(
            IAccountService accountService,
            ITransactionService transactionService,
            IAccountLinkViewModelFactory accountLinkViewModelFactory,
            int transactionId)
        {
            m_accountService = accountService;
            m_transactionService = transactionService;
            m_accountLinkViewModelFactory = accountLinkViewModelFactory;

            m_transactionId = transactionId;

            IEnumerable<AccountLink> accountLinks = m_accountService.GetAllAsLinks();
            IEnumerable<IAccountLinkViewModel> accountLinkViewModels =
                accountLinks.Select(al => m_accountLinkViewModelFactory.Create(al));

            Accounts = new ObservableCollection<IAccountLinkViewModel>(accountLinkViewModels.OrderBy(alvm => alvm.Name));
        }

        public Transaction ToTransaction()
        {
            var transaction = new Transaction
            {
                TransactionId = m_transactionId,
                CreditAccount = SelectedCreditAccount.ToAccountLink(),
                DebitAccount = SelectedDebitAccount.ToAccountLink(),
                Amount = Amount,
                At = At,
            };

            return transaction;
        }

        protected IAccountLinkViewModel m_selectedCreditAccount;
        protected IAccountLinkViewModel m_selectedDebitAccount;
        protected decimal m_amount;
        protected DateTime m_at;

        public ObservableCollection<IAccountLinkViewModel> Accounts { get; }

        public IAccountLinkViewModel SelectedCreditAccount
        {
            get { return m_selectedCreditAccount; }
            set
            {
                if (m_selectedCreditAccount != value)
                {
                    m_selectedCreditAccount = value;

                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public IAccountLinkViewModel SelectedDebitAccount
        {
            get { return m_selectedDebitAccount; }
            set
            {
                if (m_selectedDebitAccount != value)
                {
                    m_selectedDebitAccount = value;

                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public decimal Amount
        {
            get { return m_amount; }
            set
            {
                if (m_amount != value)
                {
                    m_amount = value;

                    OnPropertyChanged();
                }
            }
        }

        public DateTime At
        {
            get { return m_at; }
            set
            {
                if (m_at != value)
                {
                    m_at = value;

                    OnPropertyChanged();
                }
            }
        }

        public ICommand OKCommand => new RelayCommand(OKExecute, OKCanExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        protected abstract void OKExecute(object obj);

        private bool OKCanExecute(object obj)
        {
            return (SelectedCreditAccount != null) && (SelectedDebitAccount != null);
        }

        private void CancelExecute(object obj)
        {

        }
    }

    public class TransactionCreateViewModel : TransactionDetailsBaseViewModel
    {
        private ILogger<TransactionCreateViewModel> m_logger;

        public TransactionCreateViewModel(
            ILogger<TransactionCreateViewModel> logger,
            IAccountService accountService,
            ITransactionService transactionService,
            IAccountLinkViewModelFactory accountLinkViewModelFactory,
            Transaction hint) : base(accountService, transactionService, accountLinkViewModelFactory, 0)
        {
            m_logger = logger;

            if (hint != null)
            {
                m_selectedCreditAccount = Accounts.Single(a => a.AccountId == hint.CreditAccount.AccountId);
                m_selectedDebitAccount = Accounts.Single(a => a.AccountId == hint.DebitAccount.AccountId);
            }
            else
            {
                m_selectedCreditAccount =
                    Accounts
                        .FirstOrDefault(a => a.Type == AccountType.Capital || a.Type == AccountType.Income);
                m_selectedDebitAccount =
                    Accounts
                        .FirstOrDefault(a => a.Type == AccountType.Asset || a.Type == AccountType.Expense);
            }

            m_amount = 0m;
            m_at = DateTime.Now;
        }

        protected override void OKExecute(object obj)
        {
            Transaction transaction = ToTransaction();

            m_transactionService.Create(transaction);
            m_transactionId = transaction.TransactionId;
        }
    }

    public class TransactionEditViewModel : TransactionDetailsBaseViewModel
    {
        private ILogger<TransactionEditViewModel> m_logger;

        public TransactionEditViewModel(
            ILogger<TransactionEditViewModel> logger,
            IAccountService accountService,
            ITransactionService transactionService,
            IAccountLinkViewModelFactory accountLinkViewModelFactory,
            int transactionId) : base(accountService, transactionService, accountLinkViewModelFactory, transactionId)
        {
            m_logger = logger;

            Transaction transaction = m_transactionService.Get(m_transactionId);

            m_selectedCreditAccount = Accounts.Single(a => a.AccountId == transaction.CreditAccount.AccountId);
            m_selectedDebitAccount = Accounts.Single(a => a.AccountId == transaction.DebitAccount.AccountId);
            m_amount = transaction.Amount;
            m_at = transaction.At;
        }

        protected override void OKExecute(object obj)
        {
            Transaction transaction = ToTransaction();

            m_transactionService.Update(transaction);
        }
    }
}
