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
    public class TransactionListViewModelFactory : ITransactionListViewModelFactory
    {
        private readonly ILogger<TransactionListViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public TransactionListViewModelFactory(ILogger<TransactionListViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public ITransactionListViewModel Create()
        {
            return m_serviceProvider.CreateInstance<TransactionListViewModel>();
        }
    }

    public class TransactionListViewModel : BaseViewModel, ITransactionListViewModel
    {
        // Dependencies
        private ILogger<TransactionListViewModel> m_logger;
        private ITransactionService m_transactionService;
        private IDeleteConfirmationViewService m_deleteConfirmationViewService;
        private IViewModelFactory m_viewModelFactory;
        private IViewService m_viewService;

        // Private data
        private ObservableCollection<ITransactionItemViewModel> m_transactions;
        private ITransactionItemViewModel m_selectedTransaction;

        public TransactionListViewModel(
            ILogger<TransactionListViewModel> logger,
            ITransactionService transactionService,
            IDeleteConfirmationViewService deleteConfirmationViewService,
            IViewModelFactory viewModelFactory,
            IViewService viewService)
        {
            m_logger = logger;
            m_transactionService = transactionService;
            m_deleteConfirmationViewService = deleteConfirmationViewService;
            m_viewModelFactory = viewModelFactory;
            m_viewService = viewService;

            PopulateTransactions();
        }

        private void PopulateTransactions()
        {
            // TODO: be flexible on how many are shown
            IEnumerable<Transaction> recentTransactions = m_transactionService.GetAll()
                .OrderByDescending(t => t.At)
                .Take(100);
            List<ITransactionItemViewModel> recentTransactionViewModels =
                recentTransactions
                    .Select(t => m_viewModelFactory.CreateTransactionItemViewModel(t))
                    .ToList();

            Transactions = new ObservableCollection<ITransactionItemViewModel>(recentTransactionViewModels);
        }

        public ObservableCollection<ITransactionItemViewModel> Transactions
        {
            get { return m_transactions; }
            set
            {
                if (m_transactions != value)
                {
                    m_transactions = value;

                    OnPropertyChanged();
                }
            }
        }
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
            Transaction hint = null;
            if (SelectedTransaction != null)
            {
                hint = m_transactionService.Get(SelectedTransaction.TransactionId);
            }
            else if(Transactions.Any())
            {
                hint = m_transactionService.Get(
                    Transactions.OrderByDescending(t => t.At).First().TransactionId);
            }
            else
            {
                hint = new Transaction
                {
                    CreditAccount = null,
                    DebitAccount = null,
                    Amount = 0,
                    At = DateTime.Now
                };
            }

            Transaction newTransaction;
            if (m_viewService.OpenTransactionCreateView(hint, out newTransaction))
            {
                ITransactionItemViewModel newTransactionViewModel 
                    = m_viewModelFactory.CreateTransactionItemViewModel(newTransaction);
                Transactions.Add(newTransactionViewModel);
            }
        }

        private void EditExecute(object obj)
        {
            Transaction updatedTransaction;
            if (m_viewService.OpenTransactionEditView(SelectedTransaction.TransactionId, out updatedTransaction))
            {
                Transactions.Remove(SelectedTransaction);
                SelectedTransaction = m_viewModelFactory.CreateTransactionItemViewModel(updatedTransaction);
                Transactions.Add(SelectedTransaction);
            }
        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedTransaction != null);
        }

        private void DeleteExecute(object obj)
        {
            if(m_deleteConfirmationViewService.Show("transaction"))
            {
                // Update model
                m_transactionService.Delete(SelectedTransaction.TransactionId);

                // Update view model
                Transactions.Remove(SelectedTransaction);
            }
        }

        private bool DeleteCanExecute(object obj)
        {
            return (SelectedTransaction != null);
        }
    }
}
