namespace Financier.Services
{
    public class CashflowStatementItem
    {
        public CashflowStatementItem(string name, decimal inflow, decimal outflow)
        {
            Name = name;
            Inflow = inflow;
            Outflow = outflow;
            Cashflow = Inflow - Outflow;
        }

        public string Name { get; }
        public decimal Inflow { get; }
        public decimal Outflow { get; }
        public decimal Cashflow { get; }
    }
}
