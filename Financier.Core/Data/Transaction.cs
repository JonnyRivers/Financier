using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Text;

namespace Financier.Data
{
    public class Transaction
    {
        [Key]
        public int TransactionId { get; set; }
        public int CreditAccountId { get; set; }
        public int DebitAccountId { get; set; }
        [Required]
        public DateTime At { get; set; }
        [Required]
        public decimal CreditAmount { get; set; }
        [Required]
        public decimal DebitAmount { get; set; }

        public Account CreditAccount { get; set; }
        public Account DebitAccount { get; set; }
    }
}
