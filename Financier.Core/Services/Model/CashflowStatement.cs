using System;
using System.Collections.Generic;
using System.Linq;

namespace Financier.Services
{
    public class CashflowStatement
    {
        public CashflowStatement(
            DateTime startAt,
            DateTime endAt,
            string currencySymbol,
            IEnumerable<CashflowStatementItem> items)
        {
            StartAt = startAt;
            EndAt = endAt;

            CurrencySymbol = currencySymbol;
            Items = new List<CashflowStatementItem>(items);

            TotalInflow = Items.Sum(i => i.Inflow);
            TotalOutflow = Items.Sum(i => i.Outflow);

            NetCashflow = TotalInflow - TotalOutflow;
        }

        public DateTime StartAt { get; }
        public DateTime EndAt { get; }

        public string CurrencySymbol { get; }

        IEnumerable<CashflowStatementItem> Items { get; }

        decimal TotalInflow { get; }
        decimal TotalOutflow { get; }

        decimal NetCashflow { get; }
    }
}
