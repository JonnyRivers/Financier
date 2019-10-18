using Financier.Services;

namespace Financier.Desktop.ViewModels
{
    public interface IBalanceSheetItemViewModelFactory
    {
        IBalanceSheetItemViewModel Create(BalanceSheetItem balanceSheetItem);
    }

    public interface IBalanceSheetItemViewModel
    {
        string Name { get; }
        decimal Balance { get; }
    }
}
