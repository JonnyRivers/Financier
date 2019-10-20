using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.ViewModels
{
    public class DatabaseConnectionPasswordViewModelFactory : IDatabaseConnectionPasswordViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;

        public DatabaseConnectionPasswordViewModelFactory(ILoggerFactory loggerFactory)
        {
            m_loggerFactory = loggerFactory;
        }

        public IDatabaseConnectionPasswordViewModel Create(string userId)
        {
            return new DatabaseConnectionPasswordViewModel(m_loggerFactory, userId);
        }
    }

    public class DatabaseConnectionPasswordViewModel : IDatabaseConnectionPasswordViewModel
    {
        private ILogger<DatabaseConnectionPasswordViewModel> m_logger;

        public DatabaseConnectionPasswordViewModel(
            ILoggerFactory loggerFactory,
            string userId)
        {
            m_logger = loggerFactory.CreateLogger<DatabaseConnectionPasswordViewModel>();

            UserId = userId;
            Password = string.Empty;
        }

        public string UserId { get; set; }
        public string Password { get; set; }
    }
}
