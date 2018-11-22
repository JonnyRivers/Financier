using Financier.Services;
using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.ViewModels
{
    public class DatabaseConnectionCreateViewModel : DatabaseConnectionDetailsBaseViewModel
    {
        private readonly ILogger<DatabaseConnectionCreateViewModel> m_logger;

        public DatabaseConnectionCreateViewModel(
            ILogger<DatabaseConnectionCreateViewModel> logger,
            IDatabaseConnectionService databaseConnectionService) : base(databaseConnectionService, 0)
        {
            m_logger = logger;

            Name = "New Connection";
            SelectedType = DatabaseConnectionType.SqlServerLocalDB;
            Server = String.Empty;
            Database = String.Empty;
            UserId = String.Empty;
        }

        protected override void OKExecute(object obj)
        {
            DatabaseConnection databaseConnection = ToDatabaseConnection();

            m_databaseConnectionService.Create(databaseConnection);
            m_databaseConnectionId = databaseConnection.DatabaseConnectionId;
        }
    }
}
