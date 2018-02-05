using System.Collections.Generic;

namespace Financier.Services
{
    public interface IRegistryService
    {
        IEnumerable<string> GetDatabases();
        string GetCurrentDatabase();
        string GetConnectionString(string databaseName);
    }
}
