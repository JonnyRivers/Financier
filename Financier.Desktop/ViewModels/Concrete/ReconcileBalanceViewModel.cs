﻿using Financier.Desktop.Commands;
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
    public class ReconcileBalanceViewModelFactory : IReconcileBalanceViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;
        private readonly IAccountService m_accountService;
        private readonly ICurrencyService m_currencyService;
        private readonly ITransactionService m_transactionService;
        private readonly IAccountLinkViewModelFactory m_accountLinkViewModelFactory;
        private readonly IForeignAmountViewService m_foreignAmountViewService;

        public ReconcileBalanceViewModelFactory(
            ILoggerFactory loggerFactory,
            IAccountService accountService,
            ICurrencyService currencyService,
            ITransactionService transactionService,
            IAccountLinkViewModelFactory accountLinkViewModelFactory,
            IForeignAmountViewService foreignAmountViewService)
        {
            m_loggerFactory = loggerFactory;
            m_accountService = accountService;
            m_currencyService = currencyService;
            m_transactionService = transactionService;
            m_accountLinkViewModelFactory = accountLinkViewModelFactory;
            m_foreignAmountViewService = foreignAmountViewService;
        }

        public IReconcileBalanceViewModel Create(int accountId)
        {
            return new ReconcileBalanceViewModel(
                m_loggerFactory,
                m_accountService,
                m_currencyService,
                m_transactionService,
                m_accountLinkViewModelFactory,
                m_foreignAmountViewService,
                accountId);
        }
    }

    public class ReconcileBalanceViewModel : BaseViewModel, IReconcileBalanceViewModel
    {
        private readonly ILogger<ReconcileBalanceViewModel> m_logger;
        private readonly IAccountService m_accountService;
        private readonly ICurrencyService m_currencyService;
        private readonly ITransactionService m_transactionService;
        private readonly IAccountLinkViewModelFactory m_accountLinkViewModelFactory;
        private readonly IForeignAmountViewService m_foreignAmountViewService;

        private int m_accountId;
        private string m_primaryCurrencyCode;
        private string m_accountCurrencyCode;
        private decimal m_openingBalance;
        private decimal m_targetBalance;
        private IAccountLinkViewModel m_selectedCreditAccount;
        private DateTime m_at;

        private Transaction m_newTransaction;

        public ReconcileBalanceViewModel(
            ILoggerFactory loggerFactory,
            IAccountService accountService,
            ICurrencyService currencyService,
            ITransactionService transactionService,
            IAccountLinkViewModelFactory accountLinkViewModelFactory,
            IForeignAmountViewService foreignAmountViewService,
            int accountId)
        {
            m_logger = loggerFactory.CreateLogger<ReconcileBalanceViewModel>();
            m_accountService = accountService;
            m_currencyService = currencyService;
            m_transactionService = transactionService;
            m_accountLinkViewModelFactory = accountLinkViewModelFactory;
            m_foreignAmountViewService = foreignAmountViewService;

            m_accountId = accountId;
            m_primaryCurrencyCode = m_currencyService.GetPrimary().ShortName;
            m_accountCurrencyCode = m_accountService.Get(m_accountId).Currency.ShortName;

            m_openingBalance = m_targetBalance = m_accountService.GetBalance(m_accountId, true);
            m_at = DateTime.Now;

            IEnumerable<AccountLink> accountLinks = m_accountService.GetAllAsLinks().Where(al => al.Type == AccountType.Income);
            IEnumerable<IAccountLinkViewModel> accountLinkViewModels =
                accountLinks.Select(al => m_accountLinkViewModelFactory.Create(al));
            
            Accounts = new ObservableCollection<IAccountLinkViewModel>(accountLinkViewModels.OrderBy(alvm => alvm.Name));
            m_selectedCreditAccount = Accounts.FirstOrDefault();
        }

        public ObservableCollection<IAccountLinkViewModel> Accounts { get; }

        public decimal Balance
        {
            get { return m_targetBalance; }
            set
            {
                if (m_targetBalance != value)
                {
                    m_targetBalance = value;

                    OnPropertyChanged();
                }
            }
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

        public ICommand LookupForeignBalanceCommand 
            => new RelayCommand(LookupForeignBalanceExecute, LookupForeignBalanceCanExecute);
        public ICommand OKCommand => new RelayCommand(OKExecute, OKCanExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        private void LookupForeignBalanceExecute(object obj)
        {
            decimal exchangedAmount;
            if(m_foreignAmountViewService.Show(
                Balance,
                m_primaryCurrencyCode,
                m_accountCurrencyCode,
                out exchangedAmount))
            {
                Balance = exchangedAmount;
            }
        }

        private bool LookupForeignBalanceCanExecute(object obj)
        {
            return (m_primaryCurrencyCode != m_accountCurrencyCode);
        }

        private void OKExecute(object obj)
        {
            m_newTransaction = new Transaction
            {
                DebitAccount = m_accountService.GetAsLink(m_accountId),
                CreditAccount = m_accountService.GetAsLink(SelectedCreditAccount.AccountId),
                Amount = m_targetBalance - m_openingBalance,
                At = m_at
            };
            m_transactionService.Create(m_newTransaction);
        }

        private bool OKCanExecute(object obj)
        {
            return (SelectedCreditAccount != null);
        }

        private void CancelExecute(object obj)
        {

        }

        public Transaction ToTransaction()
        {
            return m_newTransaction;
        }
    }
}
