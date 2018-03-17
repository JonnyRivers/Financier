using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Financier.Desktop.ViewModels
{
    public class AccountTreeItemViewModel : BaseViewModel, IAccountTreeItemViewModel
    {
        private ILogger<AccountItemViewModel> m_logger;

        private string m_name;
        private AccountType m_type;
        private AccountSubType m_subType;
        private string m_currencyName;

        public AccountTreeItemViewModel(
            ILogger<AccountItemViewModel> logger,
            Account account,
            IEnumerable<IAccountTreeItemViewModel> childAccountVMs)
        {
            m_logger = logger;

            AccountId = account.AccountId;
            m_name = account.Name;
            m_type = account.Type;
            m_subType = account.SubType;
            m_currencyName = account.Currency.Name;

            Balance = 0;
            CurrencySymbol = string.Empty;
            ChildAccountItems = new ObservableCollection<IAccountTreeItemViewModel>(childAccountVMs);
        }

        public int AccountId { get; private set; }

        public decimal Balance { get; }
        public string CurrencySymbol { get; }
        public ObservableCollection<IAccountTreeItemViewModel> ChildAccountItems { get; }

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
