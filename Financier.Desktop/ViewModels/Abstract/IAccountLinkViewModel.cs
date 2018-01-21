using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountLinkViewModel
    {
        void Setup(Account account);

        int AccountId { get; }
        string Name { get; }
        AccountType Type { get; }
    }
}
