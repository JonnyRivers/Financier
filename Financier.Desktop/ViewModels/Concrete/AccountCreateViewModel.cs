using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Financier.Desktop.ViewModels
{
    public class AccountCreateViewModel : AccountDetailsBaseViewModel
    {
        private ILogger<AccountCreateViewModel> m_logger;

        public AccountCreateViewModel(
            ILogger<AccountCreateViewModel> logger,
            IAccountService accountService, 
            ICurrencyService currencyService) : base(accountService, currencyService, 0)
        {
            m_logger = logger;

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
        }
    }
}
