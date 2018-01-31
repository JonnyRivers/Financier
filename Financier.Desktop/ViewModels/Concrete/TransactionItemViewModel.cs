using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.ViewModels
{
    public class TransactionItemViewModel : BaseViewModel, ITransactionItemViewModel
    {
        private ILogger<TransactionItemViewModel> m_logger;
        private IViewModelFactory m_viewModelFactory;

        public TransactionItemViewModel(
            ILogger<TransactionItemViewModel> logger,
            IViewModelFactory viewModelFactory,
            Transaction transaction)
        {
            m_logger = logger;
            m_viewModelFactory = viewModelFactory;

            TransactionId = transaction.TransactionId;
            CreditAccount = m_viewModelFactory.CreateAccountLinkViewModel(transaction.CreditAccount);
            DebitAccount = m_viewModelFactory.CreateAccountLinkViewModel(transaction.DebitAccount);
            At = transaction.At;
            Amount = transaction.Amount;
        }

        public int TransactionId { get; private set; }
        public IAccountLinkViewModel CreditAccount { get; private set; }
        public IAccountLinkViewModel DebitAccount { get; private set; }
        public DateTime At { get; private set; }
        public decimal Amount { get; private set; }
    }
}
