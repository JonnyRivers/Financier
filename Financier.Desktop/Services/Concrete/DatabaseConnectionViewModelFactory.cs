using System;
using Financier.Desktop.ViewModels;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class DatabaseConnectionViewModelFactory : IDatabaseConnectionViewModelFactory
    {
        private readonly ILogger<DatabaseConnectionViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;// this is dumb

        public DatabaseConnectionViewModelFactory(ILogger<DatabaseConnectionViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IDatabaseConnectionDetailsViewModel CreateDatabaseConnectionCreateViewModel()
        {
            return m_serviceProvider.CreateInstance<DatabaseConnectionCreateViewModel>();
        }

        public IDatabaseConnectionDetailsViewModel CreateDatabaseConnectionEditViewModel(int databaseConnectionId)
        {
            return m_serviceProvider.CreateInstance<DatabaseConnectionEditViewModel>(databaseConnectionId);
        }

        public IDatabaseConnectionItemViewModel CreateDatabaseConnectionItemViewModel(DatabaseConnection databaseConnection)
        {
            return m_serviceProvider.CreateInstance<DatabaseConnectionItemViewModel>(databaseConnection);
        }

        public IDatabaseConnectionListViewModel CreateDatabaseConnectionListViewModel()
        {
            return m_serviceProvider.CreateInstance<DatabaseConnectionListViewModel>();
        }
    }
}
