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
    public class TransactionEditViewModel : BaseViewModel, ITransactionEditViewModel
    {
        private ILogger<TransactionEditViewModel> m_logger;
        private IAccountService m_accountService;
        private ITransactionService m_transactionService;
        private IViewModelFactory m_viewModelFactory;

        public TransactionEditViewModel(
            ILogger<TransactionEditViewModel> logger, 
            IAccountService accountService,
            ITransactionService transactionService,
            IViewModelFactory viewModelFactory)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_transactionService = transactionService;
            m_viewModelFactory = viewModelFactory;

            IEnumerable<AccountLink> accountLinks = m_accountService.GetAllAsLinks();
            IEnumerable<IAccountLinkViewModel> accountLinkViewModels = 
                accountLinks.Select(al => m_viewModelFactory.CreateAccountLinkViewModel(al));
            
            Accounts = new ObservableCollection<IAccountLinkViewModel>(accountLinkViewModels.OrderBy(alvm => alvm.Name));
        }

        public TransactionEditViewModel(
            ILogger<TransactionEditViewModel> logger,
            IAccountService accountService,
            ITransactionService transactionService,
            IViewModelFactory viewModelFactory,
            Transaction hint) : this(logger, accountService, transactionService, viewModelFactory)
        {
            m_transactionId = 0;

            if (hint != null)
            {
                SelectedCreditAccount = Accounts.Single(a => a.AccountId == hint.CreditAccount.AccountId);
                SelectedDebitAccount = Accounts.Single(a => a.AccountId == hint.DebitAccount.AccountId);
            }
            else
            {
                SelectedCreditAccount =
                    Accounts
                        .FirstOrDefault(a => a.Type == AccountType.Capital || a.Type == AccountType.Income);
                SelectedDebitAccount =
                    Accounts
                        .FirstOrDefault(a => a.Type == AccountType.Asset || a.Type == AccountType.Expense);
            }

            Amount = 0m;
            At = DateTime.Now;
        }

        public TransactionEditViewModel(
            ILogger<TransactionEditViewModel> logger,
            IAccountService accountService,
            ITransactionService transactionService,
            IViewModelFactory viewModelFactory,
            int transactionId) : this(logger, accountService, transactionService, viewModelFactory)
        {
            m_transactionId = transactionId;

            Transaction transaction = m_transactionService.Get(m_transactionId);

            SelectedCreditAccount = Accounts.Single(a => a.AccountId == transaction.CreditAccount.AccountId);
            SelectedDebitAccount = Accounts.Single(a => a.AccountId == transaction.DebitAccount.AccountId);
            Amount = transaction.Amount;
            At = transaction.At;
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

        private int m_transactionId;
        private IAccountLinkViewModel m_selectedCreditAccount;
        private IAccountLinkViewModel m_selectedDebitAccount;
        private decimal m_amount;
        private DateTime m_at;

        public ObservableCollection<IAccountLinkViewModel> Accounts { get; }

        public int TransactionId
        {
            get { return m_transactionId; }
        }

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

        private void OKExecute(object obj)
        {
            Transaction transaction = ToTransaction();

            if (m_transactionId != 0)
            {
                Transaction before = m_transactionService.Get(m_transactionId);
                m_transactionService.Update(transaction);
            }
            else
            {
                m_transactionService.Create(transaction);
                m_transactionId = transaction.TransactionId;
            }
        }

        private bool OKCanExecute(object obj)
        {
            return (SelectedCreditAccount != null) && (SelectedDebitAccount != null);
        }

        private void CancelExecute(object obj)
        {

        }
    }
}
