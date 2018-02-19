using Financier.Services;
using System;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IReconcileBalanceViewModel
    {
        Transaction ToTransaction();

        ObservableCollection<IAccountLinkViewModel> Accounts { get; }

        decimal Balance { get; set; }
        IAccountLinkViewModel SelectedCreditAccount { get; set; }
        DateTime At { get; set; }

        ICommand LookupForeignBalanceCommand { get; }
        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
