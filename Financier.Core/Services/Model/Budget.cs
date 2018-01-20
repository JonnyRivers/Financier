using System.Collections.Generic;

namespace Financier.Services
{
    public class Budget
    {
        public int BudgetId { get; set; }
        public string Name { get; set; }
        public BudgetTransaction InitialTransaction { get; set; }
        public IEnumerable<BudgetTransaction> Transactions { get; set; }
        public BudgetTransaction SurplusTransaction { get; set; }
    }
}
