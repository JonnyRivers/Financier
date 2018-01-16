using System;

namespace Financier.Desktop.ViewModels
{
    public interface ITransactionItemViewModel
    {
        int TransactionId { get; }
        string CreditAccountName { get; }
        string DebitAccountName { get; }
        DateTime At { get; }
        decimal Amount { get; }
        decimal Balance { get; }
    }
}
