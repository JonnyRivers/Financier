using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountItemViewModel
    {
        void Setup(Account account);

        int AccountId { get; }
        string Name { get; }
        AccountType Type { get; }
        AccountSubType SubType { get; }
        string CurrencyName { get; }
    }
}
