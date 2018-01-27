using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public class TransactionCreateMessage
    {
        public TransactionCreateMessage(Transaction transaction)
        {
            Transaction = transaction;
        }

        public Transaction Transaction { get; private set; }
    }
}
