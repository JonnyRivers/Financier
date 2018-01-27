using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface ITransactionListViewModel
    {
        ObservableCollection<IAccountLinkViewModel> AccountFilters { get; }
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
