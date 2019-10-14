using Financier.Services;
using System;

namespace Financier.Desktop.ViewModels
{
    public interface ITransactionItemViewModelFactory
    {
        ITransactionItemViewModel Create(Transaction transaction);
    }

    public interface ITransactionItemViewModel
    {
        int TransactionId { get; }
        IAccountLinkViewModel CreditAccount { get; }
        IAccountLinkViewModel DebitAccount { get; }
        DateTime At { get; }
        decimal Amount { get; }
    }
}
