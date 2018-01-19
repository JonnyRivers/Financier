using System;

namespace Financier.Services
{
    public class Transaction
    {
        public int TransactionId { get; set; }
        public AccountSummary CreditAccount { get; set; }
        public AccountSummary DebitAccount { get; set; }
        public decimal Amount { get; set; }
        public DateTime At { get; set; }
    }
}
