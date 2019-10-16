using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IBudgetCreateViewService
    {
        bool Show(out Budget newBudget);
    }
}
