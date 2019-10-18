using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IBudgetItemViewModelFactory
    {
        IBudgetItemViewModel Create(Budget budget, Currency currency);
    }

    public interface IBudgetItemViewModel
    {
        int BudgetId { get; }
        string Name { get; }
        BudgetPeriod Period { get; }
        string InitialTransactionHint { get; }
        int Transactions { get; }
    }
}
