namespace Financier.Desktop.ViewModels
{
    public interface ITransactionAccountFilterViewModel
    {
        int AccountId { get; }
        string Name { get; }
        bool HasLogicalAccounts { get; }
    }
}
