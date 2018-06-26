using System.Collections.Generic;
using System.Linq;

namespace Financier.Services
{
    public class CashflowAccount
    {
        public CashflowAccount(string name, string currencySymbol, IEnumerable<CashflowAccountPeriod> periods)
        {
            Name = name;
            CurrencySymbol = currencySymbol;
            Periods = new List<CashflowAccountPeriod>(periods);

            Inflow = Periods.Sum(p => p.Inflow);
            Outflow = Periods.Sum(p => p.Outflow);
            Cashflow = Periods.Sum(p => p.Cashflow);
        }

        public string Name { get; }
        public string CurrencySymbol { get; }
        public IEnumerable<CashflowAccountPeriod> Periods { get; }

        public decimal Inflow { get; }
        public decimal Outflow { get; }
        public decimal Cashflow { get; }
    }
}
