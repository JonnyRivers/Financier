namespace Financier.Services
{
    public class BudgetTransaction
    {
        public int BudgetTransactionId { get; set; }
        public AccountLink CreditAccount { get; set; }
        public AccountLink DebitAccount { get; set; }
        public decimal Amount { get; set; }
    }
}
