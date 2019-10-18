using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IDatabaseConnectionCreateViewService
    {
        bool Show(out DatabaseConnection newDatabaseConnection);
    }
}
