using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class DatabaseConnectionListViewModel : BaseViewModel, IDatabaseConnectionListViewModel
    {
        private ILogger<DatabaseConnectionListViewModel> m_logger;

        public DatabaseConnectionListViewModel(
            ILogger<DatabaseConnectionListViewModel> logger
        )
        {
            m_logger = logger;
        }

        public DatabaseConnection SelectedDatabaseConnection => throw new System.NotImplementedException();
    }
}
