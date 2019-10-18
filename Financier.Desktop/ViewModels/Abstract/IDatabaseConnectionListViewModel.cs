using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IDatabaseConnectionListViewModelFactory
    {
        IDatabaseConnectionListViewModel Create();
    }

    public interface IDatabaseConnectionListViewModel
    {
        IDatabaseConnectionItemViewModel SelectedDatabaseConnection { get; }
        string Password { get; }

        ICommand ConnectCommand { get; }
        ICommand CreateCommand { get; }
        ICommand EditCommand { get; }
        ICommand DeleteCommand { get; }
    }
}
