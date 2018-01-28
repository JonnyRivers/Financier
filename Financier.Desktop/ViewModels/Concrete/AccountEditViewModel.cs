using Financier.Desktop.Commands;
using Financier.Desktop.Services;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class AccountEditViewModel : IAccountEditViewModel
    {
        private ILogger<AccountEditViewModel> m_logger;
        private IAccountService m_accountService;
        private ICurrencyService m_currencyService;
        private int m_accountId;

        public AccountEditViewModel(
            ILogger<AccountEditViewModel> logger,
            IAccountService accountService, 
            ICurrencyService currencyService)
        {
            m_logger = logger;
            m_accountService = accountService;
            m_currencyService = currencyService;

            AccountTypes = Enum.GetValues(typeof(AccountType)).Cast<AccountType>();
            Currencies = m_currencyService.GetAll();
        }

        public void SetupForCreate()
        {
            m_accountId = 0;

            Name = "New Account";
            SelectedAccountType = AccountType.Asset;
            SelectedCurrency = Currencies.Single(c => c.IsPrimary);
        }

        public void SetupForEdit(int accountId)
        {
            m_accountId = accountId;

            Account account = m_accountService.Get(accountId);
            
            Name = account.Name;
            SelectedAccountType = account.Type;
            SelectedCurrency = Currencies.Single(c => c.CurrencyId == account.Currency.CurrencyId);
        }

        public Account ToAccount()
        {
            return new Account
            {
                AccountId = m_accountId,
                Name = Name,
                Type = SelectedAccountType,
                Currency = SelectedCurrency
            };
        }

        public IEnumerable<AccountType> AccountTypes { get; }
        public IEnumerable<Currency> Currencies { get; }

        public int AccountId
        {
            get { return m_accountId; }
        }

        public string Name { get; set; }
        public AccountType SelectedAccountType { get; set; }
        public Currency SelectedCurrency { get; set; }

        public ICommand OKCommand => new RelayCommand(OKExecute);
        public ICommand CancelCommand => new RelayCommand(CancelExecute);

        private void OKExecute(object obj)
        {
            Account account = ToAccount();

            if (m_accountId != 0)
            {
                m_accountService.Update(account);
            }
            else
            {
                m_accountService.Create(account);
                m_accountId = account.AccountId;
            }
        }

        private void CancelExecute(object obj)
        {

        }
    }
}
