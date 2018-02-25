using Financier.Desktop.ViewModels;

namespace Financier.Desktop.Tests.Concrete
{
    internal class StubAccountRelationshipTypeFilterViewModel : IAccountRelationshipTypeFilterViewModel
    {
        private AccountRelationshipType? m_type;

        public StubAccountRelationshipTypeFilterViewModel(AccountRelationshipType? type)
        {
            m_type = type;
        }

        public string Name => string.Empty;

        public AccountRelationshipType? Type => m_type;
    }
}
