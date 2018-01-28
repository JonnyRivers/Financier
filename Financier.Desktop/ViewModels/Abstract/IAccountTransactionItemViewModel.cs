using Financier.Services;
using System;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountTransactionItemViewModel
    {
        void Setup(Transaction transaction);

        int TransactionId { get; }
        IAccountLinkViewModel CreditAccount { get; }
        IAccountLinkViewModel DebitAccount { get; }
        DateTime At { get; }
        decimal Amount { get; }
        decimal Balance { get; set; }
    }
}
