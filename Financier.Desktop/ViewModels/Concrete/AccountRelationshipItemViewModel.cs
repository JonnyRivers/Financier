using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class AccountRelationshipItemViewModel : BaseViewModel, IAccountRelationshipItemViewModel
    {
        private ILogger<AccountRelationshipItemViewModel> m_logger;
        private IAccountLinkViewModelFactory m_accountLinkViewModelFactory;

        public AccountRelationshipItemViewModel(
            ILogger<AccountRelationshipItemViewModel> logger,
            IAccountLinkViewModelFactory accountlinkViewModelFactory,
            AccountRelationship accountRelationship)
        {
            m_logger = logger;
            m_accountLinkViewModelFactory = accountlinkViewModelFactory;

            AccountRelationshipId = accountRelationship.AccountRelationshipId;
            SourceAccount = m_accountLinkViewModelFactory.Create(accountRelationship.SourceAccount);
            DestinationAccount = m_accountLinkViewModelFactory.Create(accountRelationship.DestinationAccount);
            Type = accountRelationship.Type;
        }

        public int AccountRelationshipId { get; private set; }
        public IAccountLinkViewModel SourceAccount { get; private set; }
        public IAccountLinkViewModel DestinationAccount { get; private set; }
        public AccountRelationshipType Type { get; private set; }
    }
}
