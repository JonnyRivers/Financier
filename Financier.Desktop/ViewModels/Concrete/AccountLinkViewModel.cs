using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class AccountLinkViewModelFactory : IAccountLinkViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;

        public AccountLinkViewModelFactory(ILoggerFactory loggerFactory)
        {
            m_loggerFactory = loggerFactory;
        }

        public IAccountLinkViewModel Create(AccountLink accountLink)
        {
            return new AccountLinkViewModel(m_loggerFactory, accountLink);
        }
    }

    public class AccountLinkViewModel : BaseViewModel, IAccountLinkViewModel
    {
        private ILogger<AccountLinkViewModel> m_logger;

        private string m_name;
        private AccountType m_type;
        private AccountSubType m_subType;

        public AccountLinkViewModel(
            ILoggerFactory loggerFactory,
            AccountLink accountLink)
        {
            m_logger = loggerFactory.CreateLogger<AccountLinkViewModel>();

            AccountId = accountLink.AccountId;
            m_name = accountLink.Name;
            m_type = accountLink.Type;
            m_subType = accountLink.SubType;
        }

        public AccountLink ToAccountLink()
        {
            return new AccountLink
            {
                AccountId = AccountId,
                Name = Name,
                Type = Type,
                SubType = SubType
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
        public AccountSubType SubType
        {
            get { return m_subType; }
            set
            {
                if (m_subType != value)
                {
                    m_subType = value;

                    OnPropertyChanged();
                }
            }
        }
    }
}
