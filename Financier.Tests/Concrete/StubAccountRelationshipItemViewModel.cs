using Financier.Desktop.Services;
using Financier.Desktop.ViewModels;
using Financier.Services;

namespace Financier.Tests.Concrete
{
    internal class StubAccountRelationshipItemViewModel : IAccountRelationshipItemViewModel
    {
        private readonly AccountRelationship m_accountRelationship;
        private readonly IAccountLinkViewModel m_sourceAccount;
        private readonly IAccountLinkViewModel m_destinationAccount;

        internal StubAccountRelationshipItemViewModel(
            IViewModelFactory viewModelFactory,
            AccountRelationship accountRelationship)
        {
            m_accountRelationship = accountRelationship;

            m_sourceAccount = viewModelFactory.CreateAccountLinkViewModel(m_accountRelationship.SourceAccount);
            m_destinationAccount = viewModelFactory.CreateAccountLinkViewModel(m_accountRelationship.DestinationAccount);
        }

        public int AccountRelationshipId => m_accountRelationship.AccountRelationshipId;

        public IAccountLinkViewModel SourceAccount => m_sourceAccount;

        public IAccountLinkViewModel DestinationAccount => m_destinationAccount;

        public AccountRelationshipType Type => m_accountRelationship.Type;
    }
}
