using System;
using System.Collections.Generic;

namespace Financier.Services
{
    public class LocalDatabaseConnectionService : IDatabaseConnectionService
    {
        private List<DatabaseConnection> m_databaseConnetions;

        public LocalDatabaseConnectionService()
        {
            m_databaseConnetions = new List<DatabaseConnection>();
        }

        public void Create(DatabaseConnection databaseConnection)
        {
            throw new NotImplementedException();
        }

        public void Delete(int databaseConnectionId)
        {
            throw new NotImplementedException();
        }

        public DatabaseConnection Get(int databaseConnectionId)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<DatabaseConnection> GetAll()
        {
            return m_databaseConnetions;
        }

        public void Update(DatabaseConnection databaseConnection)
        {
            throw new NotImplementedException();
        }
    }
}
