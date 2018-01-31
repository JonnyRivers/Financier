using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountTransactionListViewModel
    {
        bool HasLogicalAcounts { get; }
        bool ShowLogicalAccounts { get; set; }

        ObservableCollection<IAccountTransactionItemViewModel> Transactions { get; }
        IAccountTransactionItemViewModel SelectedTransaction { get; }

        ICommand CreateCommand { get; }
        ICommand EditCommand { get; }
        ICommand DeleteCommand { get; }
    }
}
