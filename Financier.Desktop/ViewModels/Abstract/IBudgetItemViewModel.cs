using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IBudgetItemViewModel
    {
        void Setup(Budget budget, Currency currency);

        int BudgetId { get; }
        string Name { get; }
        BudgetPeriod Period { get; }
        string InitialTransactionHint { get; }
        int Transactions { get; }
    }
}
