using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class AccountCreateViewService : IAccountCreateViewService
    {
        private readonly ILogger<AccountCreateViewService> m_logger;
        private readonly IAccountDetailsViewModelFactory m_accountDetailsViewModelFactory;

        public AccountCreateViewService(
            ILoggerFactory loggerFactory, 
            IAccountDetailsViewModelFactory accountDetailsViewModelFactory)
        {
            m_logger = loggerFactory.CreateLogger<AccountCreateViewService>();
            m_accountDetailsViewModelFactory = accountDetailsViewModelFactory;
        }

        public bool Show(out Account account)
        {
            account = null;

            IAccountDetailsViewModel viewModel = m_accountDetailsViewModelFactory.Create();
            var window = new AccountDetailsWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue && result.Value)
            {
                account = viewModel.ToAccount();
                return true;
            }

            return false;
        }
    }
}
