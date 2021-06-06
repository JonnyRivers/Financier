using System;

namespace Financier.Services
{
    public class BalanceSheetItem
    {
        internal BalanceSheetItem(string name, decimal balance, DateTime lastTransactionAt)
        {
            Name = name;
            Balance = balance;
            LastTransactionAt = lastTransactionAt;
        }

        public string Name { get; }
        public decimal Balance { get; }
        public DateTime LastTransactionAt { get; }
    }
}
