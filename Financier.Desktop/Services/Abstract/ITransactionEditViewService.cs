using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface ITransactionEditViewService
    {
        bool Show(int transactionId, out Transaction updatedTransaction);
    }
}
