namespace Financier.Services
{
    public class IncomeExpenseAccountPeriod
    {
        public IncomeExpenseAccountPeriod(DateTimeRange range, decimal total)
        {
            Range = range;
            Total = total;
        }

        public DateTimeRange Range { get; }
        public decimal Total { get; }
    }
}
