using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IDatabaseConnectionListViewModel
    {
        DatabaseConnection SelectedDatabaseConnection { get; }
    }
}
