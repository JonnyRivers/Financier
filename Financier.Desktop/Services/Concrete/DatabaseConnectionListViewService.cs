using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class DatabaseConnectionListViewService : IDatabaseConnectionListViewService
    {
        private readonly ILogger<DatabaseConnectionListViewService> m_logger;
        private readonly IDatabaseConnectionListViewModelFactory m_databaseConnectionListViewModelFactory;

        public DatabaseConnectionListViewService(
            ILoggerFactory loggerFactory,
            IDatabaseConnectionListViewModelFactory databaseConnectionListViewModelFactory)
        {
            m_logger = loggerFactory.CreateLogger<DatabaseConnectionListViewService>();
            m_databaseConnectionListViewModelFactory = databaseConnectionListViewModelFactory;
        }

        public bool Show(out DatabaseConnection databaseConnection, out string password)
        {
            databaseConnection = null;
            password = string.Empty;

            var viewModel = m_databaseConnectionListViewModelFactory.Create();
            var connectionWindow = new DatabaseConnectionListWindow(viewModel);
            bool? result = connectionWindow.ShowDialog();

            if (result.HasValue && result.Value)
            {
                databaseConnection = viewModel.SelectedDatabaseConnection.ToDatabaseConnection();
                password = viewModel.Password;
                return true;
            }

            return false;
        }
    }
}
