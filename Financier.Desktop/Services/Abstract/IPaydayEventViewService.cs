using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IPaydayEventViewService
    {
        bool Show(int budgetId, out PaydayStart paydayStart);
    }
}
