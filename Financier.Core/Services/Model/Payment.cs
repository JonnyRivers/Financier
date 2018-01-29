namespace Financier.Services
{
    public class Payment
    {
        public Payment(Transaction originalTransaction, Transaction paymentTransaction)
        {
            OriginalTransaction = originalTransaction;
            PaymentTransaction = paymentTransaction;
        }

        public Transaction OriginalTransaction { get; private set; }
        public Transaction PaymentTransaction { get; private set; }
    }
}
