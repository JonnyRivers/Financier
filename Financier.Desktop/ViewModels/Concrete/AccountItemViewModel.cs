using Financier.Services;
using Microsoft.Extensions.Logging;

namespace Financier.Desktop.ViewModels
{
    public class AccountItemViewModel : IAccountItemViewModel
    {
        private ILogger<AccountItemViewModel> m_logger;

        public AccountItemViewModel(
            ILogger<AccountItemViewModel> logger)
        {
            m_logger = logger;
        }

        public void Setup(Account account)
        {
            AccountId = account.AccountId;
            Name = account.Name;
            Type = account.Type;
            CurrencyName = account.Currency.Name;
            Balance = account.Balance;
        }

        public int AccountId { get; private set; }
        public string Name { get; private set; }
        public AccountType Type { get; private set; }
        public string CurrencyName { get; private set; }
        public decimal Balance { get; private set; }
    }
}
