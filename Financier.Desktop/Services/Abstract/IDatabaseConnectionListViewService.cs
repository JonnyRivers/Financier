using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IDatabaseConnectionListViewService
    {
        bool Show(out DatabaseConnection databaseConnection, out string password);
    }
}
