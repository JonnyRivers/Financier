namespace Financier.Services
{
    public class Account
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public AccountType Type { get; set; }
        public AccountSubType SubType { get; set; }
        public Currency Currency { get; set; }
    }
}
