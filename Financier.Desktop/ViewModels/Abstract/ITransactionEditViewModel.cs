using Financier.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface ITransactionEditViewModel
    {
        void SetupForCreate();
        void SetupForEdit(int transactionId);

        Transaction ToTransaction();

        ObservableCollection<IAccountLinkViewModel> Accounts { get; }

        int TransactionId { get; }

        IAccountLinkViewModel SelectedCreditAccount { get; set; }
        IAccountLinkViewModel SelectedDebitAccount { get; set; }
        decimal Amount { get; set; }
        DateTime At { get; set; }

        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
