using Microsoft.EntityFrameworkCore.Metadata.Internal;
using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace Financier.Entities
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
