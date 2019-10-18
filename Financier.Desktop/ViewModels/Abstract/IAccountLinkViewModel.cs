using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IAccountLinkViewModelFactory
    {
        IAccountLinkViewModel Create(AccountLink accountLink);
    }

    public interface IAccountLinkViewModel
    {
        AccountLink ToAccountLink();

        int AccountId { get; }
        string Name { get; set; }
        AccountType Type { get; set; }
        AccountSubType SubType { get; set; }
    }
}
