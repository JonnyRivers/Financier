using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.ViewModels
{
    public class DatabaseConnectionPasswordViewModelFactory : IDatabaseConnectionPasswordViewModelFactory
    {
        private readonly ILogger<DatabaseConnectionPasswordViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public DatabaseConnectionPasswordViewModelFactory(ILogger<DatabaseConnectionPasswordViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IDatabaseConnectionPasswordViewModel Create(string userId)
        {
            return m_serviceProvider.CreateInstance<DatabaseConnectionPasswordViewModel>(userId);
        }
    }

    public class DatabaseConnectionPasswordViewModel : IDatabaseConnectionPasswordViewModel
    {
        private ILogger<DatabaseConnectionPasswordViewModel> m_logger;

        public DatabaseConnectionPasswordViewModel(
            ILogger<DatabaseConnectionPasswordViewModel> logger,
            string userId)
        {
            m_logger = logger;

            UserId = userId;
            Password = string.Empty;
        }

        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
