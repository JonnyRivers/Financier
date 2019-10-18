using Financier.Services;

namespace Financier.Desktop.Services
{
    public interface IBudgetEditViewService
    {
        bool Show(int budgetId, out Budget updatedBudget);
    }
}
