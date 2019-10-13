using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class AccountTreeViewService : IAccountTreeViewService
    {
        private readonly ILogger<AccountTreeViewService> m_logger;
        private readonly IAccountTreeViewModelFactory m_viewModelFactory;

        public AccountTreeViewService(ILogger<AccountTreeViewService> logger, IAccountTreeViewModelFactory viewModelFactory)
        {
            m_logger = logger;
            m_viewModelFactory = viewModelFactory;
        }

        public void Show()
        {
            IAccountTreeViewModel viewModel = m_viewModelFactory.Create();
            var window = new AccountTreeWindow(viewModel);
            window.Show();
        }
    }
}
