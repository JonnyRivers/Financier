using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IMainWindowViewModel
    {
        string DatabaseConnectionSummary { get; }

        ICommand AccountsViewCommand { get; }
        ICommand AccountRelationshipsViewCommand { get; }
        ICommand BudgetsViewCommand { get; }
        ICommand TransactionsViewCommand { get; }
    }
}
