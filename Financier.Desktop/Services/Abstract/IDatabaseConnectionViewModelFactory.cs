using Financier.Desktop.ViewModels;

namespace Financier.Desktop.Services
{
    public interface IDatabaseConnectionViewModelFactory
    {
        IDatabaseConnectionListViewModel CreateDatabaseConnectionListViewModel();
    }
}
