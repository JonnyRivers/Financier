using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class DatabaseConnectionViewService : IDatabaseConnectionViewService
    {
        private readonly ILogger<DatabaseConnectionViewService> m_logger;
        private readonly IDatabaseConnectionViewModelFactory m_viewModelFactory;

        public DatabaseConnectionViewService(ILogger<DatabaseConnectionViewService> logger, IDatabaseConnectionViewModelFactory viewModelFactory)
        {
            m_logger = logger;
            m_viewModelFactory = viewModelFactory;
        }

        public bool OpenDatabaseConnectionCreateView(out DatabaseConnection newDatabaseConnection)
        {
            throw new System.NotImplementedException();
        }

        public bool OpenDatabaseConnectionEditView(int databaseConnectionId, out DatabaseConnection updatedDatabaseConnection)
        {
            throw new System.NotImplementedException();
        }

        public bool OpenDatabaseConnectionListView(out DatabaseConnection databaseConnection)
        {
            databaseConnection = null;

            var viewModel = m_viewModelFactory.CreateDatabaseConnectionListViewModel();
            var connectionWindow = new DatabaseConnectionListWindow(viewModel);
            bool? result = connectionWindow.ShowDialog();

            if (result.HasValue && result.Value)
            {
                databaseConnection = viewModel.SelectedDatabaseConnection.ToDatabaseConnection();
                return true;
            }

            return false;
        }
    }
}
