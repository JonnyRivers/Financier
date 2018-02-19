using Financier.Desktop.Commands;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class AccountCreateViewModel : IAccountDetailsViewModel
    {
        private ILogger<AccountEditViewModel> m_logger;
        private IAccountService m_accountService;
        private ICurrencyService m_currencyService;
        private int m_accountId;

        public AccountCreateViewModel(
            ILogger<AccountEditViewModel> logger,
            IAccountService accountService, 
            ICurrencyService currencyService)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_currencyService = currencyService;

            AccountTypes = Enum.GetValues(typeof(AccountType)).Cast<AccountType>();
            AccountSubTypes = Enum.GetValues(typeof(AccountSubType)).Cast<AccountSubType>();
            Currencies = m_currencyService.GetAll();

            m_accountId = 0;

            Name = "New Account";
            SelectedAccountType = AccountType.Asset;
            SelectedAccountSubType = AccountSubType.None;
            SelectedCurrency = Currencies.Single(c => c.IsPrimary);
        }

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

        public IEnumerable<AccountType> AccountTypes { get; }
        public IEnumerable<AccountSubType> AccountSubTypes { get; }
        public IEnumerable<Currency> Currencies { get; }

        public int AccountId
        {
            get { return m_accountId; }
        }

        public string Name { get; set; }
        public AccountType SelectedAccountType { get; set; }
        public AccountSubType SelectedAccountSubType { get; set; }
        public Currency SelectedCurrency { get; set; }

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        private void OKExecute(object obj)
        {
            Account account = ToAccount();

            m_accountService.Create(account);
            m_accountId = account.AccountId;
        }

        private void CancelExecute(object obj)
        {

        }
    }
}
