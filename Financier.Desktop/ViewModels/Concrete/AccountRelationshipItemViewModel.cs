using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class AccountRelationshipItemViewModel : BaseViewModel, IAccountRelationshipItemViewModel
    {
        private ILogger<AccountRelationshipItemViewModel> m_logger;
        private IViewModelFactory m_viewModelFactory;

        public AccountRelationshipItemViewModel(
            ILogger<AccountRelationshipItemViewModel> logger,
            IViewModelFactory viewModelFactory,
            AccountRelationship accountRelationship)
        {
            m_logger = logger;
            m_viewModelFactory = viewModelFactory;

            AccountRelationshipId = accountRelationship.AccountRelationshipId;
            SourceAccount = m_viewModelFactory.CreateAccountLinkViewModel(accountRelationship.SourceAccount);
            DestinationAccount = m_viewModelFactory.CreateAccountLinkViewModel(accountRelationship.DestinationAccount);
            Type = accountRelationship.Type;
        }

        public int AccountRelationshipId { get; private set; }
        public IAccountLinkViewModel SourceAccount { get; private set; }
        public IAccountLinkViewModel DestinationAccount { get; private set; }
        public AccountRelationshipType Type { get; private set; }
    }
}
