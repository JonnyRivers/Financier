using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountLinkViewModel
    {
        void Setup(AccountLink accountLink);
        
        AccountLink ToAccountLink();

        int AccountId { get; }
        bool HasLogicalAccounts { get; set; }
        string Name { get; set; }
        AccountType Type { get; set; }
    }
}
