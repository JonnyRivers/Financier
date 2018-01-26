using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface ITransactionListViewModel
    {
        IEnumerable<IAccountLinkViewModel> AccountFilters { get; }
        IAccountLinkViewModel SelectedAccountFilter { get; }
        bool IncludeLogicalAccounts { get; }
        bool AccountFilterHasLogicalAccounts { get; }

        ObservableCollection<ITransactionItemViewModel> Transactions { get; }
        ITransactionItemViewModel SelectedTransaction { get; }

        ICommand CreateCommand { get; }
        ICommand EditCommand { get; }
        ICommand DeleteCommand { get; }
    }
}
