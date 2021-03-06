﻿using System.Collections.Generic;

namespace Financier.Services
{
    public interface IDatabaseConnectionService
    {
        void Create(DatabaseConnection databaseConnection);
        void Delete(int databaseConnectionId);
        DatabaseConnection Get(int databaseConnectionId);
        DatabaseConnection Get(string databaseConnectionName);
        IEnumerable<DatabaseConnection> GetAll();
        void Update(DatabaseConnection databaseConnection);
    }
}
