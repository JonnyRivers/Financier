using Financier.Data;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountEditViewModel
    {
        IEnumerable<AccountType> AccountTypes { get; }
        IEnumerable<Currency> Currencies { get; }

        int AccountId { get; set; }

        string Name { get; set; }
        AccountType SelectedAccountType { get; set; }
        Currency SelectedCurrency { get; set; }

        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
