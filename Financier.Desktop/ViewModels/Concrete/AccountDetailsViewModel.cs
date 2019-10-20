using Financier.Desktop.Commands;
using Financier.Services;
using Microsoft.Extensions.Logging;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public class AccountDetailsViewModelFactory : IAccountDetailsViewModelFactory
    {
        private readonly ILoggerFactory m_loggerFactory;
        private readonly IAccountService m_accountService;
        private readonly ICurrencyService m_currencyService;

        public AccountDetailsViewModelFactory(ILoggerFactory loggerFactory, IAccountService accountService, ICurrencyService currencyService)
        {
            m_loggerFactory = loggerFactory;
            m_accountService = accountService;
            m_currencyService = currencyService;
        }

        public IAccountDetailsViewModel Create()
        {
            return new AccountCreateViewModel(
                m_loggerFactory,
                m_accountService,
                m_currencyService);
        }

        public IAccountDetailsViewModel Create(int accountId)
        {
            return new AccountEditViewModel(
                m_loggerFactory,
                m_accountService,
                m_currencyService,
                accountId);
        }
    }

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

    public class AccountCreateViewModel : AccountDetailsBaseViewModel
    {
        private ILogger<AccountCreateViewModel> m_logger;

        public AccountCreateViewModel(
            ILoggerFactory loggerFactory,
            IAccountService accountService,
            ICurrencyService currencyService) : base(accountService, currencyService, 0)
        {
            m_logger = loggerFactory.CreateLogger<AccountCreateViewModel>();

            Name = "New Account";
            SelectedAccountType = AccountType.Asset;
            SelectedAccountSubType = AccountSubType.None;
            SelectedCurrency = Currencies.Single(c => c.IsPrimary);
        }

        protected override void OKExecute(object obj)
        {
            Account account = ToAccount();

            m_accountService.Create(account);
            m_accountId = account.AccountId;

            m_logger.LogInformation($"Created new account '{account.Name}' with id {account.AccountId}");
        }
    }

    public class AccountEditViewModel : AccountDetailsBaseViewModel
    {
        private ILogger<AccountEditViewModel> m_logger;

        public AccountEditViewModel(
            ILoggerFactory loggerFactory,
            IAccountService accountService,
            ICurrencyService currencyService,
            int accountId) : base(accountService, currencyService, accountId)
        {
            m_logger = loggerFactory.CreateLogger<AccountEditViewModel>();

            Account account = m_accountService.Get(m_accountId);

            Name = account.Name;
            SelectedAccountType = account.Type;
            SelectedAccountSubType = account.SubType;
            SelectedCurrency = Currencies.Single(c => c.CurrencyId == account.Currency.CurrencyId);
        }

        protected override void OKExecute(object obj)
        {
            Account account = ToAccount();

            m_accountService.Update(account);

            m_logger.LogInformation($"Updated account '{account.Name}' with id {account.AccountId}");
        }
    }
}
