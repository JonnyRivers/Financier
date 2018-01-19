using Financier.Desktop.Commands;
using Financier.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class AccountEditViewModel : IAccountEditViewModel
    {
        public AccountEditViewModel(IAccountService accountService, ICurrencyService currencyService)
        {
            m_accountService = accountService;
            m_currencyService = currencyService;
            m_accountId = 0;

            AccountTypes = Enum.GetValues(typeof(AccountType)).Cast<AccountType>();
            Currencies = m_currencyService.GetAll();

            Name = "New Account";
            SelectedAccountType = AccountType.Asset;
            SelectedCurrency = Currencies.Single(c => c.IsPrimary);
        }

        private IAccountService m_accountService;
        private ICurrencyService m_currencyService;
        private int m_accountId;

        public IEnumerable<AccountType> AccountTypes { get; }
        public IEnumerable<Currency> Currencies { get; }

        public int AccountId
        {
            get { return m_accountId; }
            set
            {
                if(value != m_accountId)
                {
                    m_accountId = value;

                    Account account = m_accountService.Get(m_accountId);
                    Name = account.Name;
                    SelectedAccountType = account.Type;
                    SelectedCurrency = Currencies.Single(c => c.CurrencyId == account.Currency.CurrencyId);
                }
            }
        }

        public string Name { get; set; }
        public AccountType SelectedAccountType { get; set; }
        public Currency SelectedCurrency { get; set; }

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        private void OKExecute(object obj)
        {
            if (m_accountId != 0)
            {
                Account account = m_accountService.Get(m_accountId);
                account.Name = Name;
                account.Type = SelectedAccountType;
                account.Currency = SelectedCurrency;

                m_accountService.Update(account);
            }
            else
            {
                var account = new Account
                {
                    Name = Name,
                    Type = SelectedAccountType,
                    Currency = SelectedCurrency
                };

                m_accountService.Create(account);
            }
        }

        private void CancelExecute(object obj)
        {

        }
    }
}
