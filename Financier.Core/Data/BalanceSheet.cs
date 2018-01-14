using System.Collections.Generic;
using System.Linq;

namespace Financier.Data
{
    public class BalanceSheet
    {
        internal BalanceSheet(
            string currencySymbol,
            IEnumerable<BalanceSheetItem> assets,
            IEnumerable<BalanceSheetItem> liabilities)
        {
            CurrencySymbol = currencySymbol;

            Assets = assets;
            Liabilities = liabilities;

            TotalAssets = Assets.Sum(a => a.Balance);
            TotalLiabilities = Liabilities.Sum(l => l.Balance);

            NetWorth = TotalAssets - TotalLiabilities;
        }

        public string CurrencySymbol { get; }

        public IEnumerable<BalanceSheetItem> Assets { get; }
        public IEnumerable<BalanceSheetItem> Liabilities { get; }

        public decimal TotalAssets { get; }
        public decimal TotalLiabilities { get; }

        public decimal NetWorth { get; }
    }
}
