using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class DatabaseConnectionEditViewModel : DatabaseConnectionDetailsBaseViewModel
    {
        private readonly ILogger<DatabaseConnectionEditViewModel> m_logger;

        public DatabaseConnectionEditViewModel(
            ILogger<DatabaseConnectionEditViewModel> logger,
            IDatabaseConnectionService databaseConnectionService,
            int databaseConnectionId) : base(databaseConnectionService, databaseConnectionId)
        {
            m_logger = logger;

            DatabaseConnection databaseConnection = m_databaseConnectionService.Get(m_databaseConnectionId);

            Name = databaseConnection.Name;
            SelectedType = databaseConnection.Type;
            Server = databaseConnection.Server;
            Database = databaseConnection.Database;
            UserId = databaseConnection.UserId;
        }

        protected override void OKExecute(object obj)
        {
            DatabaseConnection databaseConnection = ToDatabaseConnection();

            m_databaseConnectionService.Update(databaseConnection);
        }
    }
}
