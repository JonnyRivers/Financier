using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IBudgetListViewModel
    {
        ObservableCollection<IBudgetItemViewModel> Budgets { get; }
        IBudgetItemViewModel SelectedBudget { get; }

        ICommand CreateCommand { get; }
        ICommand EditCommand { get; }
        ICommand DeleteCommand { get; }
    }
}
