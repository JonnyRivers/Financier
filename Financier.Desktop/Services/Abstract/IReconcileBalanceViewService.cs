using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IReconcileBalanceViewService
    {
        bool Show(int accountId, out Transaction newTransaction);
    }
}
