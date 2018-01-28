using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class AccountItemViewModel : BaseViewModel, IAccountItemViewModel
    {
        private ILogger<AccountItemViewModel> m_logger;

        private string m_name;
        private AccountType m_type;
        private string m_currencyName;

        public AccountItemViewModel(
            ILogger<AccountItemViewModel> logger)
        {
            m_logger = logger;
        }

        public void Setup(Account account)
        {
            AccountId = account.AccountId;
            m_name = account.Name;
            m_type = account.Type;
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
