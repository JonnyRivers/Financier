using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Financier.Web.Model
{
    public class ExpenseAccount
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public IEnumerable<ExpenseTransaction> RecentTransactions { get; set; }
    }
}
