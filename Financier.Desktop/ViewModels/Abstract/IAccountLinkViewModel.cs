using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountLinkViewModel
    {
        // TODO: This shuld be AccountLink
        void Setup(Account account);
        AccountLink ToAccountLink();

        int AccountId { get; }
        string Name { get; }
        AccountType Type { get; }
    }
}
