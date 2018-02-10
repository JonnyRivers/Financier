using System;

namespace Financier.Desktop.ViewModels
{
    public class AccountRelationshipTypeViewModel : IAccountRelationshipTypeViewModel
    {
        public AccountRelationshipTypeViewModel(AccountRelationshipType? type)
        {
            m_type = type;
            m_name = type.ToReadableString();
        }

        private string m_name;
        private AccountRelationshipType? m_type;

        public string Name => m_name;
        public AccountRelationshipType? Type => m_type;
    }
}
