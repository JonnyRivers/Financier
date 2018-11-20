using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IDatabaseConnectionViewService
    {
        bool OpenDatabaseConnectionListView(out DatabaseConnection databaseConnection);
        bool OpenDatabaseConnectionCreateView(out DatabaseConnection newDatabaseConnection);
        bool OpenDatabaseConnectionEditView(int databaseConnectionId, out DatabaseConnection updatedDatabaseConnection);
    }
}
