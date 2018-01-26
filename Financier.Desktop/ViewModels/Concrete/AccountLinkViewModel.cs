using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class AccountLinkViewModel : IAccountLinkViewModel
    {
        private ILogger<AccountLinkViewModel> m_logger;

        public AccountLinkViewModel(
            ILogger<AccountLinkViewModel> logger)
        {
            m_logger = logger;
        }

        public void Setup(AccountLink accountLink)
        {
            AccountId = accountLink.AccountId;
            HasLogicalAccounts = accountLink.HasLogicalAccounts;
            Name = accountLink.Name;
            Type = accountLink.Type;
        }

        public AccountLink ToAccountLink()
        {
            return new AccountLink
            {
                AccountId = AccountId,
                HasLogicalAccounts = HasLogicalAccounts,
                Name = Name,
                Type = Type
            };
        }

        public int AccountId { get; private set; }
        public bool HasLogicalAccounts { get; private set; }
        public string Name { get; private set; }
        public AccountType Type { get; private set; }
    }
}
