using Financier.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountDetailsViewModelFactory
    {
        IAccountDetailsViewModel Create();
        IAccountDetailsViewModel Create(int accountId);
    }

    public interface IAccountDetailsViewModel
    {
        Account ToAccount();

        IEnumerable<AccountType> AccountTypes { get; }
        IEnumerable<Currency> Currencies { get; }
        
        string Name { get; set; }
        AccountType SelectedAccountType { get; set; }
        Currency SelectedCurrency { get; set; }

        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
