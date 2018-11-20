using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IDatabaseConnectionViewService
    {
        DatabaseConnection OpenDatabaseConnectionListView();
        bool OpenDatabaseConnectionCreateView(out DatabaseConnection newDatabaseConnection);
        bool OpenDatabaseConnectionEditView(int databaseConnectionId, out DatabaseConnection updatedDatabaseConnection);
    }
}
