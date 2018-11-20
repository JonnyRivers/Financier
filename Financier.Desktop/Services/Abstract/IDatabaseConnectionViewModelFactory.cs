using Financier.Desktop.ViewModels;
using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IDatabaseConnectionViewModelFactory
    {
        IDatabaseConnectionItemViewModel CreateDatabaseConnectionItemViewModel(DatabaseConnection databaseConnection);
        IDatabaseConnectionListViewModel CreateDatabaseConnectionListViewModel();
    }
}
