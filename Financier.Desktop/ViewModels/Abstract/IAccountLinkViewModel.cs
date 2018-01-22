using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountLinkViewModel
    {
        void Setup(AccountLink accountLink);
        AccountLink ToAccountLink();

        int AccountId { get; }
        string Name { get; }
        AccountType Type { get; }
    }
}
