using System.ComponentModel.DataAnnotations;

namespace Financier.Entities
{
    public class BudgetTransaction
    {
        [Key]
        public int BudgetTransactionId { get; set; }
        [Required]
        public int CreditAccountId { get; set; }
        [Required]
        public int DebitAccountId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public bool IsInitial { get; set; }
        [Required]
        public bool IsSurplus { get; set; }
        [Required]
        public int BudgetId { get; set; }

        public Account CreditAccount { get; set; }
        public Account DebitAccount { get; set; }
        public Budget Budget { get; set; }
    }
}
