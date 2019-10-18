using Financier.Services;
using Microsoft.Extensions.Logging;
using System;

namespace Financier.Desktop.ViewModels
{
    public class AccountRelationshipItemViewModelFactory : IAccountRelationshipItemViewModelFactory
    {
        private readonly ILogger<AccountRelationshipItemViewModelFactory> m_logger;
        private readonly IServiceProvider m_serviceProvider;

        public AccountRelationshipItemViewModelFactory(ILogger<AccountRelationshipItemViewModelFactory> logger, IServiceProvider serviceProvider)
        {
            m_logger = logger;
            m_serviceProvider = serviceProvider;
        }

        public IAccountRelationshipItemViewModel Create(AccountRelationship accountRelationship)
        {
            return m_serviceProvider.CreateInstance<AccountRelationshipItemViewModel>(accountRelationship);
        }
    }

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
