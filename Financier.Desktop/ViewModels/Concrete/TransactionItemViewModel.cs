using System;

namespace Financier.Desktop.ViewModels
{
    public class TransactionItemViewModel : ITransactionItemViewModel
    {
        public TransactionItemViewModel(
            int transactionId, 
            string creditAccountName, 
            string debitAccountName,
            DateTime at,
            decimal amount, 
            decimal balance
            )
        {
            TransactionId = transactionId;
            CreditAccountName = creditAccountName;
            DebitAccountName = debitAccountName;
            At = at;
            Amount = amount;
            Balance = balance;
        }

        public int TransactionId { get; }
        public string CreditAccountName { get; }
        public string DebitAccountName { get; }
        public DateTime At { get; }
        public decimal Amount { get; }
        public decimal Balance { get; }
    }
}
