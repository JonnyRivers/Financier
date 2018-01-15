using Financier.Data;
using Financier.Desktop.Commands;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class TransactionsViewModel : BaseViewModel, ITransactionsViewModel
    {
        public TransactionsViewModel(FinancierDbContext dbContext)
        {
            IEnumerable<ITransactionViewModel> transactionVMs = dbContext.Transactions
                .OrderByDescending(t => t.TransactionId)
                .Take(20)
                .Select(t =>
                    new TransactionViewModel(
                        t.CreditAccount.Name,
                        t.DebitAccount.Name,
                        t.Amount,
                        t.At));
            Transactions = new ObservableCollection<ITransactionViewModel>(transactionVMs);
        }

        private ITransactionViewModel m_selectedTransaction;

        public ObservableCollection<ITransactionViewModel> Transactions { get; }
        public ITransactionViewModel SelectedTransaction
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
            
        }

        private void EditExecute(object obj)
        {

        }

        private bool EditCanExecute(object obj)
        {
            return (SelectedTransaction != null);
        }

        private void DeleteExecute(object obj)
        {
            
        }

        private bool DeleteCanExecute(object obj)
        {
            return (SelectedTransaction != null);
        }
    }
}
