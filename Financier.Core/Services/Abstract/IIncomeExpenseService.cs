using System;

namespace Financier.Services
{
    public interface IIncomeExpenseService
    {
        IncomeExpenseStatement Generate(IncomeExpensePeriod period, DateTime startAt, DateTime endAt);
    }
}
