using System.Collections.ObjectModel;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountTreeViewModelFactory
    {
        IAccountTreeViewModel Create();
    }

    public interface IAccountTreeViewModel
    {
        ObservableCollection<IAccountTreeItemViewModel> AccountItems { get; }
        IAccountTreeItemViewModel SelectedItem { get; set; }

        ICommand CreateCommand { get; }
        ICommand EditCommand { get; }
        ICommand EditTransactionsCommand { get; }
        ICommand PayCreditCardCommand { get; }

        bool ShowAssets { get; set; }
        bool ShowLiabilities { get; set; }
        bool ShowIncome { get; set; }
        bool ShowExpenses { get; set; }
        bool ShowCapital { get; set; }
    }
}
