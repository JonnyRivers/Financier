namespace Financier.Desktop.ViewModels
{
    public interface IAccountEditViewModel
    {
        IAccountOverviewViewModel AccountOverviewViewModel { get; }
        ITransactionListViewModel TransactionListViewModel { get; }
    }
}
