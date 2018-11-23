using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
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
