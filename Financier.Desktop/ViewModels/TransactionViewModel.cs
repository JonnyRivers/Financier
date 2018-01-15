using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Desktop.ViewModels
{
    public class TransactionViewModel : BaseViewModel, ITransactionViewModel
    {
        public TransactionViewModel(string creditAccountName, string debitAccountName, decimal amount, DateTime at)
        {
            CreditAccountName = creditAccountName;
            DebitAccountName = debitAccountName;
            Amount = amount;
            At = at;
        }

        public string CreditAccountName { get; set; }
        public string DebitAccountName { get; set; }
        public decimal Amount { get; set; }
        public DateTime At { get; set; }
    }
}
