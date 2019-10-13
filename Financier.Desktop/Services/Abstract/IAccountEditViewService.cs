using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IAccountEditViewService
    {
        bool Show(int accountId, out Account updatedAccount);
    }
}
