using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class DatabaseConnectionCreateViewService : IDatabaseConnectionCreateViewService
    {
        private readonly ILogger<DatabaseConnectionCreateViewService> m_logger;
        private readonly IDatabaseConnectionDetailsViewModelFactory m_databaseConnectionDetailsViewModelFactory;

        public DatabaseConnectionCreateViewService(
            ILogger<DatabaseConnectionCreateViewService> logger,
            IDatabaseConnectionDetailsViewModelFactory databaseConnectionDetailsViewModelFactory)
        {
            m_logger = logger;
            m_databaseConnectionDetailsViewModelFactory = databaseConnectionDetailsViewModelFactory;
        }

        public bool Show(out DatabaseConnection newDatabaseConnection)
        {
            newDatabaseConnection = null;

            IDatabaseConnectionDetailsViewModel viewModel = m_databaseConnectionDetailsViewModelFactory.Create();
            var window = new DatabaseConnectionDetailsWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                newDatabaseConnection = viewModel.ToDatabaseConnection();
                return true;
            }

            return false;
        }
    }
}
