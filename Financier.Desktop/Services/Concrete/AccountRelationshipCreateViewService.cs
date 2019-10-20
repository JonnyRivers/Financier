using Financier.Desktop.ViewModels;
using Financier.Desktop.Views;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.Services
{
    public class AccountRelationshipCreateViewService : IAccountRelationshipCreateViewService
    {
        private readonly ILogger<AccountRelationshipCreateViewService> m_logger;
        private readonly IAccountRelationshipDetailsViewModelFactory m_accountRelationshipDetailsViewModelFactory;

        public AccountRelationshipCreateViewService(
            ILoggerFactory loggerFactory,
            IAccountRelationshipDetailsViewModelFactory accountRelationshipDetailsViewModelFactory)
        {
            m_logger = loggerFactory.CreateLogger<AccountRelationshipCreateViewService>();
            m_accountRelationshipDetailsViewModelFactory = accountRelationshipDetailsViewModelFactory;
        }

        public bool Show(AccountRelationship hint, out AccountRelationship accountRelationship)
        {
            accountRelationship = null;

            IAccountRelationshipDetailsViewModel viewModel =
                m_accountRelationshipDetailsViewModelFactory.Create(hint);
            var window = new AccountRelationshipDetailsWindow(viewModel);
            bool? result = window.ShowDialog();

            if (result.HasValue)
            {
                accountRelationship = viewModel.ToAccountRelationship();
                return result.Value;
            }

            return false;
        }
    }
}
