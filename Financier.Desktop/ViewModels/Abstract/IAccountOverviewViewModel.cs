using Financier.Data;
using System.Collections.Generic;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountOverviewViewModel
    {
        IEnumerable<AccountType> AccountTypes { get; }
        IEnumerable<Currency> Currencies { get; }

        string Name { get; set; }
        AccountType SelectedAccountType { get; set; }
        Currency SelectedCurrency { get; set; }

        ICommand ApplyCommand { get; }
    }
}
