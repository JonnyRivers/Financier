using System;

namespace Financier.Services
{
    public class AccountExtended
    {
        public int AccountId { get; set; }
        public string Name { get; set; }
        public AccountType Type { get; set; }
        public AccountSubType SubType { get; set; }
        public Currency Currency { get; set; }
        public decimal Balance { get; set; }
        public DateTime? LastTransactionAt { get; set; }
    }
}
