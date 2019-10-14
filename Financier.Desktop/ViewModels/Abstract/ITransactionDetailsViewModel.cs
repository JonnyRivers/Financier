using Financier.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface ITransactionDetailsViewModelFactory
    {
        ITransactionDetailsViewModel Create(Transaction hint);
        ITransactionDetailsViewModel Create(int transactionId);
    }

    public interface ITransactionDetailsViewModel
    {
        Transaction ToTransaction();

        ObservableCollection<IAccountLinkViewModel> Accounts { get; }

        IAccountLinkViewModel SelectedCreditAccount { get; set; }
        IAccountLinkViewModel SelectedDebitAccount { get; set; }
        decimal Amount { get; set; }
        DateTime At { get; set; }

        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
