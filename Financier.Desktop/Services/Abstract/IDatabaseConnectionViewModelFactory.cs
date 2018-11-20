using Financier.Desktop.ViewModels;
using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IDatabaseConnectionViewModelFactory
    {
        IDatabaseConnectionDetailsViewModel CreateDatabaseConnectionCreateViewModel();
        IDatabaseConnectionDetailsViewModel CreateDatabaseConnectionEditViewModel(int databaseConnectionId);
        IDatabaseConnectionItemViewModel CreateDatabaseConnectionItemViewModel(DatabaseConnection databaseConnection);
        IDatabaseConnectionListViewModel CreateDatabaseConnectionListViewModel();
    }
}
