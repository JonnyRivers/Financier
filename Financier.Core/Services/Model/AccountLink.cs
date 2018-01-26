namespace Financier.Services
{
    public class AccountLink
    {
        public int AccountId { get; set; }
        public bool HasLogicalAccounts { get; set; }
        public string Name { get; set; }
        public AccountType Type { get; set; }
    }
}
