using System;
using Financier.Desktop.ViewModels;
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

        public IDatabaseConnectionListViewModel CreateDatabaseConnectionListViewModel()
        {
            return m_serviceProvider.CreateInstance<DatabaseConnectionListViewModel>();
        }
    }
}
