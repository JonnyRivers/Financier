namespace Financier.Desktop.ViewModels
{
    // TODO: Replace ITransactionAccountFilterViewModel with IAccountLinkViewModel
    // https://github.com/JonnyRivers/Financier/issues/16
    public interface ITransactionAccountFilterViewModel
    {
        int AccountId { get; }
        string Name { get; }
        bool HasLogicalAccounts { get; }
    }
}
