using System.Collections.Generic;

namespace Financier.Web.Model
{
    public class CreditCard
    {
        public int Id { get; set; }
        public string Name { get; set; }
        public decimal Balance { get; set; }
        public IEnumerable<CreditCardTransaction> Transactions { get; set; }
    }
}
