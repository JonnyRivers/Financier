using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class AccountRelationshipListViewService : IAccountRelationshipListViewService
    {
        private readonly ILogger<AccountRelationshipListViewService> m_logger;
        private readonly IAccountRelationshipListViewModelFactory m_accountRelationshipListViewModelFactory;

        public AccountRelationshipListViewService(
            ILogger<AccountRelationshipListViewService> logger, 
            IAccountRelationshipListViewModelFactory accountRelationshipListViewModelFactory)
        {
            m_logger = logger;
            m_accountRelationshipListViewModelFactory = accountRelationshipListViewModelFactory;
        }

        public void Show()
        {
            IAccountRelationshipListViewModel viewModel = m_accountRelationshipListViewModelFactory.Create();
            var window = new AccountRelationshipListWindow(viewModel);
            window.Show();
        }
    }
}
