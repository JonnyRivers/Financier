using System;
using System.Collections.Generic;

namespace Financier.Services
{
    public class CashflowStatement
    {
        public CashflowStatement(
            CashflowPeriod period,
            DateTime startAt,
            DateTime endAt,
            string currencySymbol,
            IEnumerable<CashflowAccount> accounts)
        {
            Period = period;
            StartAt = startAt;
            EndAt = endAt;

            CurrencySymbol = currencySymbol;

            Accounts = new List<CashflowAccount>(accounts);
        }

        public CashflowPeriod Period { get; }
        public DateTime StartAt { get; }
        public DateTime EndAt { get; }

        public string CurrencySymbol { get; }

        public IEnumerable<CashflowAccount> Accounts { get; }
    }
}
