using Financier.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountEditViewModel
    {
        IEnumerable<Entities.AccountType> AccountTypes { get; }
        IEnumerable<Currency> Currencies { get; }

        int AccountId { get; set; }

        string Name { get; set; }
        Entities.AccountType SelectedAccountType { get; set; }
        Currency SelectedCurrency { get; set; }

        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
