﻿using Financier.Desktop.Commands;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class TransactionEditViewModel : BaseViewModel, ITransactionEditViewModel
    {
        public TransactionEditViewModel(
            ILogger<TransactionEditViewModel> logger, 
            IAccountService accountService,
            ITransactionService transactionService)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_transactionService = transactionService;
            m_transactionId = 0;

            Accounts = m_accountService.GetAll().OrderBy(a => a.Name).ToList();

            // TODO: Provide sensible defaults to new TransactionEditViewModel instances
            // https://github.com/JonnyRivers/Financier/issues/19
            SelectedCreditAccount = Accounts.First();
            SelectedDebitAccount = Accounts.First();
            Amount = 0m;
            At = DateTime.Now;
        }

        private ILogger<TransactionEditViewModel> m_logger;
        private IAccountService m_accountService;
        private ITransactionService m_transactionService;

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

                    Transaction transaction = m_transactionService.Get(m_transactionId);

                    SelectedCreditAccount = Accounts.Single(a => a.AccountId == transaction.CreditAccount.AccountId);
                    SelectedDebitAccount = Accounts.Single(a => a.AccountId == transaction.DebitAccount.AccountId);
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
                Transaction transaction = m_transactionService.Get(m_transactionId);
                transaction.CreditAccount.AccountId = SelectedCreditAccount.AccountId;
                transaction.DebitAccount.AccountId = SelectedDebitAccount.AccountId;
                transaction.Amount = Amount;
                transaction.At = At;

                m_transactionService.Update(transaction);
            }
            else
            {
                var transaction = new Transaction
                {
                    CreditAccount = new AccountLink { AccountId = SelectedCreditAccount.AccountId },
                    DebitAccount = new AccountLink { AccountId = SelectedDebitAccount.AccountId },
                    Amount = Amount,
                    At = At,
                };

                m_transactionService.Create(transaction);
            }
        }

        private void CancelExecute(object obj)
        {

        }
    }
}
