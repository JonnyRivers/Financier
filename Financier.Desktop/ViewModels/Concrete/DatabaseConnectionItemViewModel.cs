using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class DatabaseConnectionItemViewModel : IDatabaseConnectionItemViewModel
    {
        private ILogger<IDatabaseConnectionItemViewModel> m_logger;

        public DatabaseConnectionItemViewModel(
            ILogger<DatabaseConnectionItemViewModel> logger,
            DatabaseConnection databaseConnection)
        {
            m_logger = logger;

            DatabaseConnectionId = databaseConnection.DatabaseConnectionId;
            Name = databaseConnection.Name;
            Type = databaseConnection.Type;
        }

        public DatabaseConnection ToDatabaseConnection()
        {
            return new DatabaseConnection
            {
                DatabaseConnectionId = DatabaseConnectionId,
                Name = Name,
                Type = Type
            };
        }


        public int DatabaseConnectionId { get; }
        public string Name { get; }
        public DatabaseConnectionType Type { get; }
    }
}
