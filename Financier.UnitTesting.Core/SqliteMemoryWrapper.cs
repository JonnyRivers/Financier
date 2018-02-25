using Financier.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace Financier.UnitTesting
{
    public class SqliteMemoryWrapper : IDisposable
    {
        private SqliteConnection m_connection;

        public FinancierDbContext DbContext { get; }

        public SqliteMemoryWrapper()
        {
            m_connection = new SqliteConnection("DataSource=:memory:");
            m_connection.Open();
            var dbContextOptionsBuilder = new DbContextOptionsBuilder<FinancierDbContext>()
                .UseSqlite(m_connection);
            DbContext = new FinancierDbContext(dbContextOptionsBuilder.Options);
            DbContext.Database.EnsureCreated();
        }

        public void Dispose()
        {
            m_connection.Close();
            DbContext.Dispose();
        }
    }
}
