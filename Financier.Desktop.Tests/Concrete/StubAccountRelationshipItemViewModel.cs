﻿using Financier.Desktop.ViewModels;
using Financier.Services;

namespace Financier.Desktop.Tests.Concrete
{
    internal class StubAccountRelationshipItemViewModelFactory : IAccountRelationshipItemViewModelFactory
    {
        public IAccountRelationshipItemViewModel Create(AccountRelationship accountRelationship)
        {
            return new StubAccountRelationshipItemViewModel(
                new StubAccountLinkViewModelFactory(),
                accountRelationship
            );
        }
    }

    internal class StubAccountRelationshipItemViewModel : IAccountRelationshipItemViewModel
    {
        private readonly AccountRelationship m_accountRelationship;
        private readonly IAccountLinkViewModel m_sourceAccount;
        private readonly IAccountLinkViewModel m_destinationAccount;

        internal StubAccountRelationshipItemViewModel(
            IAccountLinkViewModelFactory accountLinkViewModelFactory,
            AccountRelationship accountRelationship)
        {
            m_accountRelationship = accountRelationship;

            m_sourceAccount = accountLinkViewModelFactory.Create(m_accountRelationship.SourceAccount);
            m_destinationAccount = accountLinkViewModelFactory.Create(m_accountRelationship.DestinationAccount);
        }

        public int AccountRelationshipId => m_accountRelationship.AccountRelationshipId;

        public IAccountLinkViewModel SourceAccount => m_sourceAccount;

        public IAccountLinkViewModel DestinationAccount => m_destinationAccount;

        public AccountRelationshipType Type => m_accountRelationship.Type;
    }
}
