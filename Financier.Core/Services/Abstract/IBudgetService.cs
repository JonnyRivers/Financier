using System.Collections.Generic;

namespace Financier.Services
{
    public interface IBudgetService
    {
        void Create(Budget budget);
        void Delete(int budgetId);
        IEnumerable<Budget> GetAll();
        Budget Get(int budgetId);
        void Update(Budget budget);

        IEnumerable<Transaction> MakePaydayTransactions(PaydayStart paydayStart);
    }
}
