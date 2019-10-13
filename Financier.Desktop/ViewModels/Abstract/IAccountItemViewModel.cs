using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountItemViewModelFactory
    {
        IAccountItemViewModel Create(Account account);
    }

    public interface IAccountItemViewModel
    {
        int AccountId { get; }
        string Name { get; }
        AccountType Type { get; }
        AccountSubType SubType { get; }
        string CurrencyName { get; }
    }
}
