namespace Financier.Services
{
    public class BalanceSheetItem
    {
        internal BalanceSheetItem(string name, decimal balance)
        {
            Name = name;
            Balance = balance;
        }

        public string Name { get; }
        public decimal Balance { get; }
    }
}
