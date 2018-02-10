using Financier.Desktop.ViewModels;

namespace Financier.Tests.Concrete
{
    internal class StubAccountRelationshipTypeViewModel : IAccountRelationshipTypeViewModel
    {
        private AccountRelationshipType? m_type;

        public StubAccountRelationshipTypeViewModel(AccountRelationshipType? type)
        {
            m_type = type;
        }

        public string Name => string.Empty;

        public AccountRelationshipType? Type => m_type;
    }
}
