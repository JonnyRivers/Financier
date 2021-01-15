using System.Collections.Generic;
using System.Linq;

namespace Financier.Services
{
    public class IncomeExpenseAccount
    {
        public IncomeExpenseAccount(string name, string currencySymbol, IEnumerable<IncomeExpenseAccountPeriod> periods)
        {
            Name = name;
            CurrencySymbol = currencySymbol;
            Periods = new List<IncomeExpenseAccountPeriod>(periods);
        }

        public string Name { get; }
        public string CurrencySymbol { get; }
        public IEnumerable<IncomeExpenseAccountPeriod> Periods { get; }
    }
}
