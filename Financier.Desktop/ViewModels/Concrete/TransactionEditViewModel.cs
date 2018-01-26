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
        private IConversionService m_conversionService;
        private ITransactionService m_transactionService;

        public TransactionEditViewModel(
            ILogger<TransactionEditViewModel> logger, 
            IAccountService accountService,
            IConversionService conversionService,
            ITransactionService transactionService)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_conversionService = conversionService;
            m_transactionService = transactionService;

            IEnumerable<AccountLink> accountLinks = m_accountService.GetAllAsLinks();
            IEnumerable<IAccountLinkViewModel> accountLinkViewModels = 
                accountLinks.Select(al => m_conversionService.AccountLinkToViewModel(al));
            
            Accounts = new ObservableCollection<IAccountLinkViewModel>(accountLinkViewModels);
        }

        public void SetupForCreate()
        {
            m_transactionId = 0;

            if (m_transactionService.Any())
            {
                Transaction mostRecentTransaction = m_transactionService.GetMostRecent();

                SelectedCreditAccount = Accounts.Single(a => a.AccountId == mostRecentTransaction.CreditAccount.AccountId);
                SelectedDebitAccount = Accounts.Single(a => a.AccountId == mostRecentTransaction.DebitAccount.AccountId);
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

        public void SetupForEdit(int budgetId)
        {
            m_transactionId = budgetId;

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
                m_transactionService.Update(transaction);
            }
            else
            {
                m_transactionService.Create(transaction);
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
