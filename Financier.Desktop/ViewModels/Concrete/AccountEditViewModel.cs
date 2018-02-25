using Financier.Services;
using Microsoft.Extensions.Logging;
using System.Linq;

namespace Financier.Desktop.ViewModels
{
    public class AccountEditViewModel : AccountDetailsBaseViewModel
    {
        private ILogger<AccountEditViewModel> m_logger;

        public AccountEditViewModel(
            ILogger<AccountEditViewModel> logger,
            IAccountService accountService, 
            ICurrencyService currencyService,
            int accountId) : base(accountService, currencyService, accountId)
        {
            m_logger = logger;

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
        }
    }
}
