using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public class TransactionDeleteMessage
    {
        public TransactionDeleteMessage(Transaction transaction)
        {
            Transaction = transaction;
        }

        public Transaction Transaction { get; private set; }
    }
}
