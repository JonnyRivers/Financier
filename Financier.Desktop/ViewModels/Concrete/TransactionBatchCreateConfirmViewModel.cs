using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class TransactionBatchCreateConfirmViewModel : BaseViewModel, ITransactionBatchCreateConfirmViewModel
    {
        private ILogger<TransactionBatchCreateConfirmViewModel> m_logger;
        private IViewModelFactory m_viewModelFactory;

        private ObservableCollection<ITransactionItemViewModel> m_transactions;

        public TransactionBatchCreateConfirmViewModel(
            ILogger<TransactionBatchCreateConfirmViewModel> logger,
            IViewModelFactory viewModelFactory,
            IEnumerable<Transaction> transactions)
        {
            m_logger = logger;
            m_viewModelFactory = viewModelFactory;

            IEnumerable<ITransactionItemViewModel> transactionViewModels =
                transactions.Select(t => m_viewModelFactory.CreateTransactionItemViewModel(t));

            Transactions = new ObservableCollection<ITransactionItemViewModel>(transactionViewModels);
        }

        public ObservableCollection<ITransactionItemViewModel> Transactions
        {
            get { return m_transactions; }
            private set
            {
                if (m_transactions != value)
                {
                    m_transactions = value;

                    OnPropertyChanged();
                }
            }
        }

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        private void OKExecute(object obj)
        {

        }

        private void CancelExecute(object obj)
        {

        }
    }
}
