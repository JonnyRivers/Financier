using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IDatabaseConnectionViewService
    {
        bool OpenDatabaseConnectionListView(out DatabaseConnection databaseConnection, out string password);
        bool OpenDatabaseConnectionCreateView(out DatabaseConnection newDatabaseConnection);
        bool OpenDatabaseConnectionEditView(int databaseConnectionId, out DatabaseConnection updatedDatabaseConnection);
        bool OpenDatabaseConnectionDeleteConfirmationView();
        // TODO - move this to a separate service?
        bool OpenDatabaseConnectionPasswordView(string userId, out string password);
    }
}
