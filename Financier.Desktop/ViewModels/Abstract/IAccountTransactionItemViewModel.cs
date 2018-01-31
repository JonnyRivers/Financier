using System;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountTransactionItemViewModel
    {
        int TransactionId { get; }
        IAccountLinkViewModel CreditAccount { get; }
        IAccountLinkViewModel DebitAccount { get; }
        DateTime At { get; }
        decimal Amount { get; }
        decimal Balance { get; set; }
    }
}
