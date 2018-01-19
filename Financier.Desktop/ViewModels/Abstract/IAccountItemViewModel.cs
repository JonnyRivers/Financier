using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountItemViewModel
    {
        int AccountId { get; }
        string Name { get; }
        AccountType Type { get; }
        string CurrencyName { get; }
        decimal Balance { get; }
    }
}
