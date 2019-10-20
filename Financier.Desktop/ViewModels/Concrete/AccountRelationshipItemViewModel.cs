using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class AccountRelationshipItemViewModelFactory : IAccountRelationshipItemViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;
        private IAccountLinkViewModelFactory m_accountLinkViewModelFactory;

        public AccountRelationshipItemViewModelFactory(
            ILoggerFactory loggerFactory,
            IAccountLinkViewModelFactory accountLinkViewModelFactory)
        {
            m_loggerFactory = loggerFactory;
            m_accountLinkViewModelFactory = accountLinkViewModelFactory;
        }

        public IAccountRelationshipItemViewModel Create(AccountRelationship accountRelationship)
        {
            return new AccountRelationshipItemViewModel(
                m_loggerFactory,
                m_accountLinkViewModelFactory,
                accountRelationship);
        }
    }

    public class AccountRelationshipItemViewModel : BaseViewModel, IAccountRelationshipItemViewModel
    {
        private ILogger<AccountRelationshipItemViewModel> m_logger;
        private IAccountLinkViewModelFactory m_accountLinkViewModelFactory;

        public AccountRelationshipItemViewModel(
            ILoggerFactory loggerFactory,
            IAccountLinkViewModelFactory accountlinkViewModelFactory,
            AccountRelationship accountRelationship)
        {
            m_logger = loggerFactory.CreateLogger<AccountRelationshipItemViewModel>();
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
