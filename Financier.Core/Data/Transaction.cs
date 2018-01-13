using System;
using System.ComponentModel.DataAnnotations;

namespace Financier.Data
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        [Required]
        public int CreditAccountId { get; set; }
        [Required]
        public int DebitAccountId { get; set; }
        [Required]
        public decimal Amount { get; set; }
        [Required]
        public DateTime At { get; set; }

        public Account CreditAccount { get; set; }
        public Account DebitAccount { get; set; }
    }
}
