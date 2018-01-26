using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountLinkViewModel
    {
        void Setup(AccountLink accountLink);
        // TODO: Bring consistency to ViewModel->Model conversions
        // https://github.com/JonnyRivers/Financier/issues/22
        AccountLink ToAccountLink();

        int AccountId { get; }
        bool HasLogicalAccounts { get; }
        string Name { get; }
        AccountType Type { get; }
    }
}
