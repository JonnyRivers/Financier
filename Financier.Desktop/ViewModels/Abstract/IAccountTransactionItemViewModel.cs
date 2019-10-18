using Financier.Services;
using System;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountTransactionItemViewModelFactory
    {
        IAccountTransactionItemViewModel Create(Transaction transaction);
    }

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
