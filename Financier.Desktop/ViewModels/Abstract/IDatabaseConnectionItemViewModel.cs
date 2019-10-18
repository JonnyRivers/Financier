using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IDatabaseConnectionItemViewModelFactory
    {
        IDatabaseConnectionItemViewModel Create(DatabaseConnection databaseConnection);
    }

    public interface IDatabaseConnectionItemViewModel
    {
        int DatabaseConnectionId { get; }
        string Name { get; }
        DatabaseConnectionType Type { get; }

        string Server { get; }
        string Database { get; }
        string UserId { get; }

        DatabaseConnection ToDatabaseConnection();
    }
}
