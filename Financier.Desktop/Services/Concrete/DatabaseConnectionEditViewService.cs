using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class DatabaseConnectionEditViewService : IDatabaseConnectionEditViewService
    {
        private readonly ILogger<DatabaseConnectionEditViewService> m_logger;
        private readonly IDatabaseConnectionDetailsViewModelFactory m_databaseConnectionDetailsViewModelFactory;

        public DatabaseConnectionEditViewService(
            ILoggerFactory loggerFactory,
            IDatabaseConnectionDetailsViewModelFactory databaseConnectionDetailsViewModelFactory)
        {
            m_logger = loggerFactory.CreateLogger<DatabaseConnectionEditViewService>();
            m_databaseConnectionDetailsViewModelFactory = databaseConnectionDetailsViewModelFactory;
        }

        public bool Show(int databaseConnectionId, out DatabaseConnection updatedDatabaseConnection)
        {
            updatedDatabaseConnection = null;

            IDatabaseConnectionDetailsViewModel viewModel = m_databaseConnectionDetailsViewModelFactory.Create(databaseConnectionId);
            var window = new DatabaseConnectionDetailsWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                updatedDatabaseConnection = viewModel.ToDatabaseConnection();
                return true;
            }

            return false;
        }
    }
}
