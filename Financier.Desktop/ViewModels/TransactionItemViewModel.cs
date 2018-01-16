using System;

namespace Financier.Desktop.ViewModels
{
    public class TransactionItemViewModel : ITransactionItemViewModel
    {
        public TransactionItemViewModel(
            int transactionId, 
            string creditAccountName, 
            string debitAccountName, 
            decimal amount, 
            DateTime at)
        {
            TransactionId = transactionId;
            CreditAccountName = creditAccountName;
            DebitAccountName = debitAccountName;
            Amount = amount;
            At = at;
        }

        public int TransactionId { get; }
        public string CreditAccountName { get; }
        public string DebitAccountName { get; }
        public decimal Amount { get; }
        public DateTime At { get; }
    }
}
