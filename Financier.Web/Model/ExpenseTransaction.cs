using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Financier.Web.Model
{
    public class ExpenseTransaction
    {
        public int Id { get; set; }
        public DateTime At { get; set; }
        public string OtherAccountName { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }
}
