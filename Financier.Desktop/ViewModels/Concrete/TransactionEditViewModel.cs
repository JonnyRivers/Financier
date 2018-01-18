using Financier.Data;
using Financier.Desktop.Commands;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class TransactionEditViewModel : BaseViewModel, ITransactionEditViewModel
    {
        public TransactionEditViewModel(FinancierDbContext dbContext)
        {
            m_dbContext = dbContext;
            m_transactionId = 0;

            Accounts = m_dbContext.Accounts.OrderBy(a => a.Name).ToList();

            // TODO: take selections from last transaction?
            SelectedCreditAccount = Accounts.First();// TODO: handle no accounts
            SelectedDebitAccount = Accounts.First();// TODO: handle no accounts
            Amount = 0m;
            At = DateTime.Now;
        }

        private FinancierDbContext m_dbContext;

        private int m_transactionId;
        private Account m_selectedCreditAccount;
        private Account m_selectedDebitAccount;
        private decimal m_amount;
        private DateTime m_at;

        public IEnumerable<Account> Accounts { get; }

        public int TransactionId
        {
            get { return m_transactionId; }
            set
            {
                if (value != m_transactionId)
                {
                    m_transactionId = value;

                    Transaction transaction = m_dbContext.Transactions.Single(t => t.TransactionId == m_transactionId);

                    SelectedCreditAccount = Accounts.Single(a => a.AccountId == transaction.CreditAccountId);
                    SelectedDebitAccount = Accounts.Single(a => a.AccountId == transaction.DebitAccountId);
                    Amount = transaction.Amount;
                    At = transaction.At;
                }
            }
        }

        public Account SelectedCreditAccount
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

        public Account SelectedDebitAccount
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
                    CommandManager.InvalidateRequerySuggested();
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
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        private void OKExecute(object obj)
        {
            if (m_transactionId != 0)
            {
                Transaction transaction = m_dbContext.Transactions.Single(t => t.TransactionId == m_transactionId);
                transaction.CreditAccountId = SelectedCreditAccount.AccountId;
                transaction.DebitAccountId = SelectedDebitAccount.AccountId;
                transaction.Amount = Amount;
                transaction.At = At;
                
            }
            else
            {
                var transaction = new Transaction
                {
                    CreditAccountId = SelectedCreditAccount.AccountId,
                    DebitAccountId = SelectedDebitAccount.AccountId,
                    Amount = Amount,
                    At = At,
                };
                m_dbContext.Transactions.Add(transaction);
            }

            m_dbContext.SaveChanges();
        }

        private void CancelExecute(object obj)
        {

        }
    }
}
