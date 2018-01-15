using Financier.Data;
using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Desktop.Views;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class TransactionListViewModel : BaseViewModel, ITransactionListViewModel
    {
        public TransactionListViewModel(FinancierDbContext dbContext)
        {
            m_dbContext = dbContext;

            PopulateTransactions();
        }

        private FinancierDbContext m_dbContext;

        private ITransactionItemViewModel m_selectedTransaction;

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
            IWindowFactory windowFactory = IoC.ServiceProvider.Instance.GetRequiredService<IWindowFactory>();
            Window transactionEditWindow = windowFactory.CreateTransactionCreateWindow();

            bool? result = transactionEditWindow.ShowDialog();

            if (result.HasValue && result.Value)
            {
                PopulateTransactions();
            }
        }

        private void EditExecute(object obj)
        {
            IWindowFactory windowFactory = IoC.ServiceProvider.Instance.GetRequiredService<IWindowFactory>();
            Window transactionEditWindow = windowFactory.CreateTransactionEditWindow(SelectedTransaction.TransactionId);

            bool? result = transactionEditWindow.ShowDialog();

            if(result.HasValue && result.Value)
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
            // TODO: build a proper confirm dialog
            MessageBoxResult confirmResult = MessageBox.Show(
                "Are you sure you want to delete this transaction?  This cannot be undone.", 
                "Really delete transaction?", 
                MessageBoxButton.YesNo
            );

            if(confirmResult == MessageBoxResult.Yes)
            {
                Transaction transaction = m_dbContext.Transactions.Single(t => t.TransactionId == SelectedTransaction.TransactionId);
                m_dbContext.Transactions.Remove(transaction);
                m_dbContext.SaveChanges();

                PopulateTransactions();
            }
        }

        private bool DeleteCanExecute(object obj)
        {
            return (SelectedTransaction != null);
        }

        private void PopulateTransactions()
        {
            IEnumerable<ITransactionItemViewModel> transactionVMs = m_dbContext.Transactions
                .OrderByDescending(t => t.TransactionId)
                .Take(20)
                .Select(t =>
                    new TransactionItemViewModel(
                        t.TransactionId,
                        t.CreditAccount.Name,
                        t.DebitAccount.Name,
                        t.Amount,
                        t.At));
            Transactions = new ObservableCollection<ITransactionItemViewModel>(transactionVMs);
            OnPropertyChanged(nameof(Transactions));
        }
    }
}
