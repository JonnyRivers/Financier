using System;
using System.Collections.Generic;

namespace Financier.Services
{
    public class IncomeExpenseStatement
    {
        public IncomeExpenseStatement(
            IEnumerable<DateTimeRange> dateTimeRanges,
            IncomeExpensePeriod period,
            DateTime startAt,
            DateTime endAt,
            string currencySymbol,
            IEnumerable<IncomeExpenseAccount> incomeAccounts,
            IEnumerable<IncomeExpenseAccount> expenseAccounts)
        {
            DateTimeRanges = dateTimeRanges;
            Period = period;
            StartAt = startAt;
            EndAt = endAt;

            CurrencySymbol = currencySymbol;

            IncomeAccounts = new List<IncomeExpenseAccount>(incomeAccounts);
            ExpenseAccounts = new List<IncomeExpenseAccount>(expenseAccounts);
        }

        public IEnumerable<DateTimeRange> DateTimeRanges { get; }
        public IncomeExpensePeriod Period { get; }
        public DateTime StartAt { get; }
        public DateTime EndAt { get; }

        public string CurrencySymbol { get; }

        public IEnumerable<IncomeExpenseAccount> IncomeAccounts { get; }
        public IEnumerable<IncomeExpenseAccount> ExpenseAccounts { get; }
    }
}
