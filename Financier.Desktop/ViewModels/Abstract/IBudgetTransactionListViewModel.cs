using Financier.Services;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IBudgetTransactionListViewModelFactory
    {
        IBudgetTransactionListViewModel Create(int budgetId);
    }

    public interface IBudgetTransactionListViewModel
    {
        ObservableCollection<IBudgetTransactionItemViewModel> Transactions { get; set; }
        IBudgetTransactionItemViewModel SelectedTransaction { get; set; }

        ICommand CreateCommand { get; }
        ICommand DeleteCommand { get; }
    }
}
