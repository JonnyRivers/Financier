﻿using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class TransactionListViewModel : BaseViewModel, ITransactionListViewModel
    {
        private ILogger<AccountListViewModel> m_logger;
        private IAccountService m_accountService;
        private IAccountRelationshipService m_accountRelationshipService;
        private IConversionService m_conversionService;
        private ITransactionService m_transactionService;
        private IViewService m_viewService;

        public TransactionListViewModel(
            ILogger<AccountListViewModel> logger, 
            IAccountService accountService,
            IAccountRelationshipService accountRelationshipService,
            IConversionService conversionService,
            ITransactionService transactionService,
            IViewService viewService)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_accountRelationshipService = accountRelationshipService;
            m_conversionService = conversionService;
            m_transactionService = transactionService;
            m_viewService = viewService;

            IEnumerable<AccountLink> accountLinks = m_accountService.GetAllAsLinks();

            var accountFilters = new List<IAccountLinkViewModel>();
            var nullAccountLink = new AccountLink { AccountId = 0, Name = "(All Accounts)" };
            m_nullAccountFilter = m_conversionService.AccountLinkToViewModel(nullAccountLink);
            accountFilters.Add(m_nullAccountFilter);
            accountFilters.AddRange(
                accountLinks
                    .OrderBy(a => a.Name)
                    .Select(a => m_conversionService.AccountLinkToViewModel(a))
            );
            AccountFilters = accountFilters;
            SelectedAccountFilter = m_nullAccountFilter;
            m_includeLogicalAccounts = true;

            PopulateTransactions();
        }

        private IAccountLinkViewModel m_nullAccountFilter;
        private IAccountLinkViewModel m_selectedAccountFilter;
        private bool m_includeLogicalAccounts;
        private bool m_accountFilterHasLogicalAcounts;
        private ITransactionItemViewModel m_selectedTransaction;

        public IEnumerable<IAccountLinkViewModel> AccountFilters { get; }
        public IAccountLinkViewModel SelectedAccountFilter
        {
            get { return m_selectedAccountFilter; }
            set
            {
                if (m_selectedAccountFilter != value)
                {
                    m_selectedAccountFilter = value;

                    AccountFilterHasLogicalAccounts = m_selectedAccountFilter.HasLogicalAccounts;
                    OnPropertyChanged();
                    PopulateTransactions();
                }
            }
        }
        public bool IncludeLogicalAccounts
        {
            get { return m_includeLogicalAccounts; }
            set
            {
                if (m_includeLogicalAccounts != value)
                {
                    m_includeLogicalAccounts = value;

                    OnPropertyChanged();
                    PopulateTransactions();
                }
            }
        }
        public bool AccountFilterHasLogicalAccounts
        {
            get { return m_accountFilterHasLogicalAcounts; }
            set
            {
                if (m_accountFilterHasLogicalAcounts != value)
                {
                    m_accountFilterHasLogicalAcounts = value;

                    OnPropertyChanged();
                }
            }
        }

        public ObservableCollection<ITransactionItemViewModel> Transactions { get; set; }
        public ITransactionItemViewModel SelectedTransaction
        {
            get { return m_selectedTransaction; }
            set
            {
                if (m_selectedTransaction != value)
                {
                    m_selectedTransaction = value;
                    
                    OnPropertyChanged();
                    CommandManager.InvalidateRequerySuggested();
                }
            }
        }

        public ICommand CreateCommand => new RelayCommand(CreateExecute);
        public ICommand EditCommand => new RelayCommand(EditExecute, EditCanExecute);
        public ICommand DeleteCommand => new RelayCommand(DeleteExecute, DeleteCanExecute);

        private void CreateExecute(object obj)
        {
            if (m_viewService.OpenTransactionCreateView())
            {
                PopulateTransactions();
            }
        }

        private void EditExecute(object obj)
        {
            if (m_viewService.OpenTransactionEditView(SelectedTransaction.TransactionId))
            {
                PopulateTransactions();
            }
        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedTransaction != null);
        }

        private void DeleteExecute(object obj)
        {
            if(m_viewService.OpenTransactionDeleteConfirmationView())
            {
                m_transactionService.Delete(SelectedTransaction.TransactionId);

                PopulateTransactions();
            }
        }

        private bool DeleteCanExecute(object obj)
        {
            return (SelectedTransaction != null);
        }

        private void PopulateTransactions()
        {
            if (SelectedAccountFilter == m_nullAccountFilter)
            {
                IEnumerable<Transaction> transactions = m_transactionService
                    .GetAll()
                    .OrderByDescending(t => t.TransactionId)
                    .Take(100)
                    .ToList();
                IEnumerable<ITransactionItemViewModel> transactionVMs =
                    transactions.Select(t => m_conversionService.TransactionToItemViewModel(t, 0));
                Transactions = new ObservableCollection<ITransactionItemViewModel>(transactionVMs);
            }
            else
            {
                IEnumerable<Transaction> transactions = m_transactionService
                    .GetAll(SelectedAccountFilter.AccountId, IncludeLogicalAccounts);
                
                var itemVMs = new List<ITransactionItemViewModel>();
                decimal balance = 0;
                foreach(Transaction transaction in transactions)
                {
                    if (transaction.CreditAccount.AccountId == SelectedAccountFilter.AccountId)
                    {
                        balance -= transaction.Amount;
                    }
                    else
                    {
                        balance += transaction.Amount;
                    }
                    itemVMs.Add(m_conversionService.TransactionToItemViewModel(transaction, balance));
                }
                Transactions = new ObservableCollection<ITransactionItemViewModel>(
                    itemVMs.OrderByDescending(t => t.TransactionId)
                );
            }
            
            OnPropertyChanged(nameof(Transactions));
        }
    }
}
