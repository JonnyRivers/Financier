using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class AccountRelationshipListViewService : IAccountRelationshipListViewService
    {
        private readonly ILogger<AccountRelationshipListViewService> m_logger;
        private readonly IAccountRelationshipListViewModelFactory m_viewModelFactory;

        public AccountRelationshipListViewService(ILogger<AccountRelationshipListViewService> logger, IAccountRelationshipListViewModelFactory viewModelFactory)
        {
            m_logger = logger;
            m_viewModelFactory = viewModelFactory;
        }

        public void Show()
        {
            IAccountRelationshipListViewModel viewModel = m_viewModelFactory.Create();
            var window = new AccountRelationshipListWindow(viewModel);
            window.Show();
        }
    }
}
