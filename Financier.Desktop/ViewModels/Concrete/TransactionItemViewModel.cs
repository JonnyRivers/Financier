using Financier.Services;
using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.ViewModels
{
    public class TransactionItemViewModelFactory : ITransactionItemViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;
        private readonly IAccountLinkViewModelFactory m_accountLinkViewModelFactory;

        public TransactionItemViewModelFactory(
            ILoggerFactory loggerFactory,
            IAccountLinkViewModelFactory accountLinkViewModelFactory)
        {
            m_loggerFactory = loggerFactory;
            m_accountLinkViewModelFactory = accountLinkViewModelFactory;
        }

        public ITransactionItemViewModel Create(Transaction transaction)
        {
            return new TransactionItemViewModel(
                m_loggerFactory,
                m_accountLinkViewModelFactory,
                transaction);
        }
    }

    public class TransactionItemViewModel : BaseViewModel, ITransactionItemViewModel
    {
        private readonly ILogger<TransactionItemViewModel> m_logger;
        private readonly IAccountLinkViewModelFactory m_accountLinkViewModelFactory;

        public TransactionItemViewModel(
            ILoggerFactory loggerFactory,
            IAccountLinkViewModelFactory accountLinkViewModelFactory,
            Transaction transaction)
        {
            m_logger = loggerFactory.CreateLogger<TransactionItemViewModel>();
            m_accountLinkViewModelFactory = accountLinkViewModelFactory;

            TransactionId = transaction.TransactionId;
            CreditAccount = m_accountLinkViewModelFactory.Create(transaction.CreditAccount);
            DebitAccount = m_accountLinkViewModelFactory.Create(transaction.DebitAccount);
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
