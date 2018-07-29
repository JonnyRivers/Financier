namespace Financier.Desktop.ViewModels
{
    public interface IBalanceSheetItemViewModel
    {
        string Name { get; }
        decimal Balance { get; }
    }
}
