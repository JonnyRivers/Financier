using System.Collections.Generic;

namespace Financier.Services
{
    public interface IBudgetService
    {
        void Create(Budget budget);
        IEnumerable<Budget> GetAll();
        Budget Get(int budgetId);
        void Update(Budget budget);
    }
}
