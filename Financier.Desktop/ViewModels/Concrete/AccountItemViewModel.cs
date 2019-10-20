using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class AccountItemViewModelFactory : IAccountItemViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;

        public AccountItemViewModelFactory(ILoggerFactory loggerFactory)
        {
            m_loggerFactory = loggerFactory;
        }

        public IAccountItemViewModel Create(Account account)
        {
            return new AccountItemViewModel(m_loggerFactory, account);
        }
    }

    public class AccountItemViewModel : BaseViewModel, IAccountItemViewModel
    {
        private ILogger<AccountItemViewModel> m_logger;

        private string m_name;
        private AccountType m_type;
        private AccountSubType m_subType;
        private string m_currencyName;

        public AccountItemViewModel(
            ILoggerFactory loggerFactory,
            Account account)
        {
            m_logger = loggerFactory.CreateLogger< AccountItemViewModel>();

            AccountId = account.AccountId;
            m_name = account.Name;
            m_type = account.Type;
            m_subType = account.SubType;
            m_currencyName = account.Currency.Name;
        }

        public int AccountId { get; private set; }
        public string Name
        {
            get { return m_name; }
            private set
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
            private set
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
            private set
            {
                if (m_subType != value)
                {
                    m_subType = value;

                    OnPropertyChanged();
                }
            }
        }
        public string CurrencyName
        {
            get { return m_currencyName; }
            private set
            {
                if (m_currencyName != value)
                {
                    m_currencyName = value;

                    OnPropertyChanged();
                }
            }
        }
    }
}
