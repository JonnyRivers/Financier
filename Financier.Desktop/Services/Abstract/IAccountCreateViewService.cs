using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IAccountCreateViewService
    {
        bool Show(out Account newAccount);
    }
}
