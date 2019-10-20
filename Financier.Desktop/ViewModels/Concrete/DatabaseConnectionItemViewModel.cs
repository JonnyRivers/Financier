using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class DatabaseConnectionItemViewModelFactory : IDatabaseConnectionItemViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;

        public DatabaseConnectionItemViewModelFactory(ILoggerFactory loggerFactory)
        {
            m_loggerFactory = loggerFactory;
        }

        public IDatabaseConnectionItemViewModel Create(DatabaseConnection databaseConnection)
        {
            return new DatabaseConnectionItemViewModel(
                m_loggerFactory,
                databaseConnection);
        }
    }

    public class DatabaseConnectionItemViewModel : IDatabaseConnectionItemViewModel
    {
        private ILogger<DatabaseConnectionItemViewModel> m_logger;

        public DatabaseConnectionItemViewModel(
            ILoggerFactory loggerFactory,
            DatabaseConnection databaseConnection)
        {
            m_logger = loggerFactory.CreateLogger<DatabaseConnectionItemViewModel>();

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
