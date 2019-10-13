using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.ViewModels
{
    public class TransactionItemViewModel : BaseViewModel, ITransactionItemViewModel
    {
        private ILogger<TransactionItemViewModel> m_logger;
        private IAccountLinkViewModelFactory m_acountLinkViewModelFactory;

        public TransactionItemViewModel(
            ILogger<TransactionItemViewModel> logger,
            IAccountLinkViewModelFactory acountLinkViewModelFactory,
            Transaction transaction)
        {
            m_logger = logger;
            m_acountLinkViewModelFactory = acountLinkViewModelFactory;

            TransactionId = transaction.TransactionId;
            CreditAccount = m_acountLinkViewModelFactory.Create(transaction.CreditAccount);
            DebitAccount = m_acountLinkViewModelFactory.Create(transaction.DebitAccount);
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
