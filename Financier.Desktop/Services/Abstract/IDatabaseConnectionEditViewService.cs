using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IDatabaseConnectionEditViewService
    {
        bool Show(int databaseConnectionId, out DatabaseConnection updatedDatabaseConnection);
    }
}
