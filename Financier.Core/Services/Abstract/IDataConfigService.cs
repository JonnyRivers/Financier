using System.Collections.Generic;

namespace Financier.Services
{
    public interface IDataConfigService
    {
        IEnumerable<string> GetDatabases();
        string GetCurrentDatabase();
        string GetConnectionString(string databaseName);
    }
}
