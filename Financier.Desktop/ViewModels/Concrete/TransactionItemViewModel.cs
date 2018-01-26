using Financier.Services;
using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.ViewModels
{
    public class TransactionItemViewModel : ITransactionItemViewModel
    {
        private ILogger<TransactionItemViewModel> m_logger;

        public TransactionItemViewModel(
            ILogger<TransactionItemViewModel> logger)
        {
            m_logger = logger;
        }

        public void Setup(Transaction transaction, decimal balance)
        {
            TransactionId = transaction.TransactionId;
            CreditAccountName = transaction.CreditAccount.Name;
            DebitAccountName = transaction.DebitAccount.Name;
            At = transaction.At;
            Amount = transaction.Amount;
            Balance = balance;
        }

        public int TransactionId { get; private set; }
        public string CreditAccountName { get; private set; }
        public string DebitAccountName { get; private set; }
        public DateTime At { get; private set; }
        public decimal Amount { get; private set; }
        public decimal Balance { get; private set; }
    }
}
