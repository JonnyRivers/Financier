using Financier.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IBudgetTransactionListViewModel
    {
        void SetupForCreate();
        void SetupForEdit(Budget budget);

        ObservableCollection<IBudgetTransactionItemViewModel> Transactions { get; set; }
        IBudgetTransactionItemViewModel SelectedTransaction { get; set; }

        ICommand CreateCommand { get; }
        ICommand DeleteCommand { get; }
    }
}
