using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class AccountTreeViewService : IAccountTreeViewService
    {
        private readonly ILogger<AccountTreeViewService> m_logger;
        private readonly IAccountTreeViewModelFactory m_accountTreeViewModelFactory;

        public AccountTreeViewService(ILoggerFactory loggerFactory, IAccountTreeViewModelFactory accountTreeViewModelFactory)
        {
            m_logger = loggerFactory.CreateLogger<AccountTreeViewService>();
            m_accountTreeViewModelFactory = accountTreeViewModelFactory;
        }

        public void Show()
        {
            IAccountTreeViewModel viewModel = m_accountTreeViewModelFactory.Create();
            var window = new AccountTreeWindow(viewModel);
            window.Show();
        }
    }
}
