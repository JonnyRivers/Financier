using System;
using System.Collections.Generic;

namespace Financier.Services
{
    public class LocalDatabaseConnectionService : IDatabaseConnectionService
    {
        public void Create(DatabaseConnection connection)
        {
            throw new NotImplementedException();
        }

        public void Delete(int budgetId)
        {
            throw new NotImplementedException();
        }

        public DatabaseConnection Get(int connectionId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DatabaseConnection> GetAll()
        {
            throw new NotImplementedException();
        }

        public void Update(DatabaseConnection connection)
        {
            throw new NotImplementedException();
        }
    }
}
