using Financier.Entities;

namespace Financier.Desktop.ViewModels
{
    public class AccountItemViewModel : IAccountItemViewModel
    {
        public AccountItemViewModel(
            int accountId, 
            string name, 
            AccountType type, 
            string currencyName, 
            decimal balance)
        {
            AccountId = accountId;
            Name = name;
            Type = type;
            CurrencyName = currencyName;
            Balance = balance;
        }

        public int AccountId { get; }
        public string Name { get; }
        public AccountType Type { get; }
        public string CurrencyName { get; }
        public decimal Balance { get; }
    }
}
