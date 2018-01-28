using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;

namespace Financier.Desktop.ViewModels
{
    public class AccountLinkViewModel : BaseViewModel, IAccountLinkViewModel
    {
        private ILogger<AccountLinkViewModel> m_logger;

        private string m_name;
        private AccountType m_type;

        public AccountLinkViewModel(
            ILogger<AccountLinkViewModel> logger)
        {
            m_logger = logger;
        }

        public void Setup(AccountLink accountLink)
        {
            AccountId = accountLink.AccountId;
            m_name = accountLink.Name;
            m_type = accountLink.Type;
        }

        public AccountLink ToAccountLink()
        {
            return new AccountLink
            {
                AccountId = AccountId,
                Name = Name,
                Type = Type
            };
        }

        public int AccountId { get; private set; }
        public string Name
        {
            get { return m_name; }
            set
            {
                if (m_name != value)
                {
                    m_name = value;

                    OnPropertyChanged();
                }
            }
        }
        public AccountType Type
        {
            get { return m_type; }
            set
            {
                if (m_type != value)
                {
                    m_type = value;

                    OnPropertyChanged();
                }
            }
        }
    }
}
