using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Desktop.ViewModels
{
    public class TransactionViewModel : BaseViewModel, ITransactionViewModel
    {
        public TransactionViewModel(int transactionId, string creditAccountName, string debitAccountName, decimal amount, DateTime at)
        {
            TransactionId = transactionId;
            CreditAccountName = creditAccountName;
            DebitAccountName = debitAccountName;
            Amount = amount;
            At = at;
        }

        public int TransactionId { get; set; }
        public string CreditAccountName { get; set; }
        public string DebitAccountName { get; set; }
        public decimal Amount { get; set; }
        public DateTime At { get; set; }
    }
}
