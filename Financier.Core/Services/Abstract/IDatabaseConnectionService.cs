using System.Collections.Generic;

namespace Financier.Services
{
    public interface IDatabaseConnectionService
    {
        void Create(DatabaseConnection connection);
        void Delete(int budgetId);
        DatabaseConnection Get(int connectionId);
        IEnumerable<DatabaseConnection> GetAll();
        void Update(DatabaseConnection connection);
    }
}
