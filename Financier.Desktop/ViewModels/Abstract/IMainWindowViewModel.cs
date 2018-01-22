namespace Financier.Desktop.ViewModels
{
    public interface IMainWindowViewModel
    {
        IAccountListViewModel AccountListViewModel { get; }
        IBudgetListViewModel BudgetListViewModel { get; }
        ITransactionListViewModel TransactionListViewModel { get; }
    }
}
