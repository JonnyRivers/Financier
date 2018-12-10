using System;

namespace Financier.WebAPI.Model
{
    public class AccountTransaction
    {
        public string DebitAccountName { get; set; }
        public string CreditAccountName { get; set; }
        public DateTime At { get; set; }
        public decimal Amount { get; set; }
        public decimal Balance { get; set; }
    }
}
