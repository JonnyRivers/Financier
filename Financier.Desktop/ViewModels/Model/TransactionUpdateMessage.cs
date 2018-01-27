using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public class TransactionUpdateMessage
    {
        public TransactionUpdateMessage(Transaction before, Transaction after)
        {
            Before = before;
            After = after;
        }

        public Transaction Before { get; private set; }
        public Transaction After { get; private set; }
    }
}
