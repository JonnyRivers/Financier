using Financier.Desktop.ViewModels;
using Financier.Services;

namespace Financier.Desktop.Tests.Concrete
{
    internal class StubAccountLinkViewModelFactory : IAccountLinkViewModelFactory
    {
        public IAccountLinkViewModel Create(AccountLink accountLink)
        {
            return new StubAccountLinkViewModel(accountLink);
        }
    }

    internal class StubAccountLinkViewModel : IAccountLinkViewModel
    {
        private AccountLink m_accountLink;

        internal StubAccountLinkViewModel(
            AccountLink accountLink)
        {
            m_accountLink = accountLink;
        }

        public int AccountId => m_accountLink.AccountId;

        public string Name { get => m_accountLink.Name; set => m_accountLink.Name = value; }
        public AccountType Type { get => m_accountLink.Type; set => m_accountLink.Type = value; }
        public AccountSubType SubType { get => m_accountLink.SubType; set => m_accountLink.SubType = value; }

        public AccountLink ToAccountLink()
        {
            return m_accountLink;
        }
    }
}
