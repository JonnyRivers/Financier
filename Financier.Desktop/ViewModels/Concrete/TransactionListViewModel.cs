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
    public class TransactionListViewModelFactory : ITransactionListViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;
        private readonly ITransactionService m_transactionService;
        private readonly ITransactionItemViewModelFactory m_transactionItemViewModelFactory;
        private readonly IDeleteConfirmationViewService m_deleteConfirmationViewService;
        private readonly ITransactionCreateViewService m_transactionCreateViewService;
        private readonly ITransactionEditViewService m_transactionEditViewService;

        public TransactionListViewModelFactory(
            ILoggerFactory loggerFactory,
            ITransactionService transactionService,
            ITransactionItemViewModelFactory transactionItemViewModelFactory,
            IDeleteConfirmationViewService deleteConfirmationViewService,
            ITransactionCreateViewService transactionCreateViewService,
            ITransactionEditViewService transactionEditViewService)
        {
            m_loggerFactory = loggerFactory;
            m_transactionService = transactionService;
            m_transactionItemViewModelFactory = transactionItemViewModelFactory;
            m_deleteConfirmationViewService = deleteConfirmationViewService;
            m_transactionCreateViewService = transactionCreateViewService;
            m_transactionEditViewService = transactionEditViewService;
        }

        public ITransactionListViewModel Create()
        {
            return new TransactionListViewModel(
                m_loggerFactory,
                m_transactionService,
                m_transactionItemViewModelFactory,
                m_deleteConfirmationViewService,
                m_transactionCreateViewService,
                m_transactionEditViewService);
        }
    }

    public class TransactionListViewModel : BaseViewModel, ITransactionListViewModel
    {
        // Dependencies
        private readonly ILogger<TransactionListViewModel> m_logger;
        private readonly ITransactionService m_transactionService;
        private readonly ITransactionItemViewModelFactory m_transactionItemViewModelFactory;
        private readonly IDeleteConfirmationViewService m_deleteConfirmationViewService;
        private readonly ITransactionCreateViewService m_transactionCreateViewService;
        private readonly ITransactionEditViewService m_transactionEditViewService;

        // Private data
        private ObservableCollection<ITransactionItemViewModel> m_transactions;
        private ITransactionItemViewModel m_selectedTransaction;

        public TransactionListViewModel(
            ILoggerFactory loggerFactory,
            ITransactionService transactionService,
            ITransactionItemViewModelFactory transactionItemViewModelFactory,
            IDeleteConfirmationViewService deleteConfirmationViewService,
            ITransactionCreateViewService transactionCreateViewService,
            ITransactionEditViewService transactionEditViewService)
        {
            m_logger = loggerFactory.CreateLogger<TransactionListViewModel>();
            m_transactionService = transactionService;
            m_transactionItemViewModelFactory = transactionItemViewModelFactory;
            m_deleteConfirmationViewService = deleteConfirmationViewService;
            m_transactionCreateViewService = transactionCreateViewService;
            m_transactionEditViewService = transactionEditViewService;

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
                    .Select(t => m_transactionItemViewModelFactory.Create(t))
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
            if (m_transactionCreateViewService.Show(hint, out newTransaction))
            {
                ITransactionItemViewModel newTransactionViewModel 
                    = m_transactionItemViewModelFactory.Create(newTransaction);
                Transactions.Add(newTransactionViewModel);
            }
        }

        private void EditExecute(object obj)
        {
            Transaction updatedTransaction;
            if (m_transactionEditViewService.Show(SelectedTransaction.TransactionId, out updatedTransaction))
            {
                Transactions.Remove(SelectedTransaction);
                SelectedTransaction = m_transactionItemViewModelFactory.Create(updatedTransaction);
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
