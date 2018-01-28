using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountListViewModel
    {
        ObservableCollection<IAccountItemViewModel> Accounts { get; }
        IAccountItemViewModel SelectedAccount { get; }

        ICommand CreateCommand { get; }
        ICommand EditCommand { get; }
        ICommand EditTransactionsCommand { get; }
    }
}
