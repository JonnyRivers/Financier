using Financier.Desktop.Commands;
using Financier.Services;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public abstract class AccountDetailsBaseViewModel : IAccountDetailsViewModel
    {
        protected IAccountService m_accountService;
        protected ICurrencyService m_currencyService;

        protected int m_accountId;

        protected AccountDetailsBaseViewModel(
            IAccountService accountService,
            ICurrencyService currencyService,
            int accountId)
        {
            m_accountService = accountService;
            m_currencyService = currencyService;

            m_accountId = accountId;

            AccountTypes = Enum.GetValues(typeof(AccountType)).Cast<AccountType>();
            AccountSubTypes = Enum.GetValues(typeof(AccountSubType)).Cast<AccountSubType>();
            Currencies = m_currencyService.GetAll();
        }

        public IEnumerable<AccountType> AccountTypes { get; }
        public IEnumerable<AccountSubType> AccountSubTypes { get; }
        public IEnumerable<Currency> Currencies { get; }

        public string Name { get; set; }
        public AccountType SelectedAccountType { get; set; }
        public AccountSubType SelectedAccountSubType { get; set; }
        public Currency SelectedCurrency { get; set; }

        public Account ToAccount()
        {
            return new Account
            {
                AccountId = m_accountId,
                Name = Name,
                Type = SelectedAccountType,
                SubType = SelectedAccountSubType,
                Currency = SelectedCurrency
            };
        }

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        protected abstract void OKExecute(object obj);

        private void CancelExecute(object obj)
        {

        }
    }
}
