using Financier.Desktop.Commands;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class TransactionBatchCreateConfirmViewModelFactory : ITransactionBatchCreateConfirmViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;
        private readonly ITransactionItemViewModelFactory m_transactionItemViewModelFactory;

        public TransactionBatchCreateConfirmViewModelFactory(
            ILoggerFactory loggerFactory,
            ITransactionItemViewModelFactory transactionItemViewModelFactory)
        {
            m_loggerFactory = loggerFactory;
            m_transactionItemViewModelFactory = transactionItemViewModelFactory;
        }

        public ITransactionBatchCreateConfirmViewModel Create(IEnumerable<Transaction> transactions)
        {
            return new TransactionBatchCreateConfirmViewModel(
                m_loggerFactory,
                m_transactionItemViewModelFactory,
                transactions);
        }
    }

    public class TransactionBatchCreateConfirmViewModel : BaseViewModel, ITransactionBatchCreateConfirmViewModel
    {
        private readonly ILogger<TransactionBatchCreateConfirmViewModel> m_logger;
        private readonly ITransactionItemViewModelFactory m_transactionItemViewModelFactory;

        private ObservableCollection<ITransactionItemViewModel> m_transactions;

        public TransactionBatchCreateConfirmViewModel(
            ILoggerFactory loggerFactory,
            ITransactionItemViewModelFactory transactionItemViewModelFactory,
            IEnumerable<Transaction> transactions)
        {
            m_logger = loggerFactory.CreateLogger<TransactionBatchCreateConfirmViewModel>();
            m_transactionItemViewModelFactory = transactionItemViewModelFactory;

            IEnumerable<ITransactionItemViewModel> transactionViewModels =
                transactions.Select(t => m_transactionItemViewModelFactory.Create(t));

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
