using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IDatabaseConnectionItemViewModel
    {
        DatabaseConnection ToDatabaseConnection();

        int DatabaseConnectionId { get; }
        string Name { get; }
        DatabaseConnectionType Type { get; }
    }
}
