namespace Financier.Desktop.ViewModels
{
    public class TransactionAccountFilterViewModel : ITransactionAccountFilterViewModel
    {
        public TransactionAccountFilterViewModel(int accountId, string name, bool hasLogicalAccounts)
        {
            AccountId = accountId;
            Name = name;
            HasLogicalAccounts = hasLogicalAccounts;
        }

        public int AccountId { get; }
        public string Name { get; }
        public bool HasLogicalAccounts { get; }
    }
}
