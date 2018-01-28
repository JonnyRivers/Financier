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
    public class TransactionListViewModel : BaseViewModel, ITransactionListViewModel
    {
        // Dependencies
        private ILogger<AccountListViewModel> m_logger;
        private IAccountService m_accountService;
        private IAccountRelationshipService m_accountRelationshipService;
        private IConversionService m_conversionService;
        private ITransactionService m_transactionService;
        private IViewService m_viewService;

        // Private data
        private ObservableCollection<ITransactionItemViewModel> m_transactions;
        private ITransactionItemViewModel m_selectedTransaction;

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

            PopulateTransactions();
        }

        private void PopulateTransactions()
        {
            IEnumerable<Transaction> recentTransactions = m_transactionService.GetAll()
                    .OrderByDescending(t => t.At)
                    .Take(100);
            List<ITransactionItemViewModel> recentTransactionViewModels =
                recentTransactions
                    .Select(t => m_conversionService.TransactionToItemViewModel(t))
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
            Transaction transaction;
            if (m_viewService.OpenTransactionCreateView(out transaction))
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
                // Update model
                Transaction transaction = m_transactionService.Get(SelectedTransaction.TransactionId);
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
