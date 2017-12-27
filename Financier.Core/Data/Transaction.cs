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
        public DateTime At { get; set; }
        public decimal CreditAmount { get; set; }
        public decimal DebitAmount { get; set; }

        public Account CreditAccount { get; set; }
        public Account DebitAccount { get; set; }
    }
}
