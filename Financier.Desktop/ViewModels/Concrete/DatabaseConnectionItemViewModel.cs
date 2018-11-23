using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class DatabaseConnectionItemViewModel : IDatabaseConnectionItemViewModel
    {
        private ILogger<DatabaseConnectionItemViewModel> m_logger;

        public DatabaseConnectionItemViewModel(
            ILogger<DatabaseConnectionItemViewModel> logger,
            DatabaseConnection databaseConnection)
        {
            m_logger = logger;

            DatabaseConnectionId = databaseConnection.DatabaseConnectionId;
            Name = databaseConnection.Name;
            Type = databaseConnection.Type;

            Server = databaseConnection.Server;
            Database = databaseConnection.Database;
            UserId = databaseConnection.UserId;
        }

        public int DatabaseConnectionId { get; }
        public string Name { get; set; }
        public DatabaseConnectionType Type { get; set; }

        public string Server { get; set; }
        public string Database { get; set; }
        public string UserId { get; set; }

        // TODO - this is duplicated with DatabaseConnectionDetailsBaseViewModel
        public DatabaseConnection ToDatabaseConnection()
        {
            return new DatabaseConnection
            {
                DatabaseConnectionId = DatabaseConnectionId,
                Name = Name,
                Type = Type,
                Server = Server,
                Database = Database,
                UserId = UserId
            };
        }
    }
}
