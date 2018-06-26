namespace Financier.Services
{
    public class CashflowAccountPeriod
    {
        public CashflowAccountPeriod(DateTimeRange range, decimal inflow, decimal outflow)
        {
            Range = range;
            Inflow = inflow;
            Outflow = outflow;
            Cashflow = Inflow - Outflow;
        }

        public DateTimeRange Range { get; }
        public decimal Inflow { get; }
        public decimal Outflow { get; }
        public decimal Cashflow { get; }
    }
}
