using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface ITransactionCreateViewService
    {
        bool Show(Transaction hint, out Transaction newTransaction);
    }
}
