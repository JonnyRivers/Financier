using System.Collections.Generic;
using System.Linq;

namespace Financier.Data
{
    public class BalanceSheet
    {
        internal BalanceSheet(
            string currencySymbol,
            IEnumerable<BalanceSheetItem> assets,
            IEnumerable<BalanceSheetItem> liabilities,
            IEnumerable<BalanceSheetItem> equities)
        {
            CurrencySymbol = currencySymbol;

            Assets = assets;
            Liabilities = liabilities;
            Equities = equities;

            TotalAssets = Assets.Sum(a => a.Balance);
            TotalLiabilitiesAndEquity = Liabilities.Sum(l => l.Balance) + Equities.Sum(e => e.Balance);
        }

        public string CurrencySymbol { get; }

        public IEnumerable<BalanceSheetItem> Assets { get; }
        public IEnumerable<BalanceSheetItem> Liabilities { get; }
        public IEnumerable<BalanceSheetItem> Equities { get; }

        public decimal TotalAssets { get; }
        public decimal TotalLiabilitiesAndEquity { get; }
    }
}
