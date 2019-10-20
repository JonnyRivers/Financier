using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class AccountEditViewService : IAccountEditViewService
    {
        private readonly ILogger<AccountEditViewService> m_logger;
        private readonly IAccountDetailsViewModelFactory m_accountDetailsViewModelFactory;

        public AccountEditViewService(
            ILoggerFactory loggerFactory,
            IAccountDetailsViewModelFactory accountDetailsViewModelFactory)
        {
            m_logger = loggerFactory.CreateLogger<AccountEditViewService>();
            m_accountDetailsViewModelFactory = accountDetailsViewModelFactory;
        }

        public bool Show(int accountId, out Account account)
        {
            account = null;

            IAccountDetailsViewModel viewModel = m_accountDetailsViewModelFactory.Create(accountId);
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
