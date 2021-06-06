using System;

namespace Financier.Web.Model
{
    public class CreditCardTransaction
    {
        public int Id { get; set; }
        public DateTime At { get; set; }
        public string OtherAccountName { get; set; }
        public decimal Amount { get; set; }
        public decimal RunningBalance { get; set; }
    }
}
