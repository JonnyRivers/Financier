using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class DatabaseConnectionPasswordViewService : IDatabaseConnectionPasswordViewService
    {
        private readonly ILogger<DatabaseConnectionPasswordViewService> m_logger;
        private readonly IDatabaseConnectionPasswordViewModelFactory m_databaseConnectionPasswordViewModelFactory;

        public DatabaseConnectionPasswordViewService(
            ILogger<DatabaseConnectionPasswordViewService> logger,
            IDatabaseConnectionPasswordViewModelFactory databaseConnectionPasswordViewModelFactory)
        {
            m_logger = logger;
            m_databaseConnectionPasswordViewModelFactory = databaseConnectionPasswordViewModelFactory;
        }

        public bool Show(string userId, out string password)
        {
            password = null;

            var viewModel = m_databaseConnectionPasswordViewModelFactory.Create(userId);
            var passwordWindow = new DatabaseConnectionPasswordWindow(viewModel);
            bool? result = passwordWindow.ShowDialog();

            if (result.HasValue && result.Value)
            {
                password = viewModel.Password;
                return true;
            }

            return false;
        }
    }
}
