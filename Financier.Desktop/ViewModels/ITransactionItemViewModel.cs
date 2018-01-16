using System;

namespace Financier.Desktop.ViewModels
{
    public interface ITransactionItemViewModel
    {
        int TransactionId { get; }
        string CreditAccountName { get; }
        string DebitAccountName { get; }
        decimal Amount { get; }
        DateTime At { get; }
    }
}
