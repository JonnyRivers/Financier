using Financier.Services;
using System.Collections.Generic;

namespace Financier.Desktop.Services
{
    public interface ITransactionBatchCreateConfirmViewService
    {
        bool Show(IEnumerable<Transaction> transactions);
    }
}
