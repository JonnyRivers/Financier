using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Financier.Desktop.ViewModels
{
    public interface ITransactionViewModel
    {
        string CreditAccountName { get; set; }
        string DebitAccountName { get; set; }
        decimal Amount { get; set; }
        DateTime At { get; set; }
    }
}
