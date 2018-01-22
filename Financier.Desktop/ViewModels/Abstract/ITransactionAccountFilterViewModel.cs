namespace Financier.Desktop.ViewModels
{
    // TODO: This is really an IAccountLinkViewModel
    public interface ITransactionAccountFilterViewModel
    {
        int AccountId { get; }
        string Name { get; }
        bool HasLogicalAccounts { get; }
    }
}
