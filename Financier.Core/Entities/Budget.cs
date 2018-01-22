using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;

namespace Financier.Entities
{
    public class Budget
    {
        [Key]
        public int BudgetId { get; set; }
        [Required]
        public string Name { get; set; }
        [Required]
        public BudgetPeriod Period { get; set; }

        public List<BudgetTransaction> Transactions { get; set; }
    }
}
