using System.Collections.Generic;

namespace Financier.Services
{
    public class Account
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public AccountType Type { get; set; }
        public Currency Currency { get; set; }
        public IEnumerable<Account> LogicalAccounts { get; set;}
        public decimal Balance { get; set; }
    }
}
