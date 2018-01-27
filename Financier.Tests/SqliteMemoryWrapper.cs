using Financier.Entities;
using Microsoft.Data.Sqlite;
using Microsoft.EntityFrameworkCore;
using System;

namespace Financier.Core.Tests
{
    internal class SqliteMemoryWrapper : IDisposable
    {
        private SqliteConnection m_connection;

        internal FinancierDbContext DbContext { get; }

        internal SqliteMemoryWrapper()
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
