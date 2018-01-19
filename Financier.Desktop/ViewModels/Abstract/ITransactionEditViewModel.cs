using Financier.Services;
using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface ITransactionEditViewModel
    {
        IEnumerable<Account> Accounts { get; }

        int TransactionId { get; set; }

        Account SelectedCreditAccount { get; set; }
        Account SelectedDebitAccount { get; set; }
        decimal Amount { get; set; }
        DateTime At { get; set; }

        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
