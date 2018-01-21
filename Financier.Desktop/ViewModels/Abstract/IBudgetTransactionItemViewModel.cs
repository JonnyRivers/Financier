using Financier.Services;
using System.Collections.ObjectModel;

namespace Financier.Desktop.ViewModels
{
    public interface IBudgetTransactionItemViewModel
    {
        void Setup(
            ObservableCollection<IAccountLinkViewModel> accountLinks,
            BudgetTransaction budgetTransaction, 
            BudgetTransactionType type);
        BudgetTransaction ToBudgetTransaction();

        int BudgetTransactionId { get; set; }
        BudgetTransactionType Type { get; set; }
        IAccountLinkViewModel SelectedCreditAccount { get; set; }
        IAccountLinkViewModel SelectedDebitAccount { get; set; }
        decimal Amount { get; set; }

        // This has to be an ObservableCollection<T>
        // Despite this not being required in other situations, the use of a <GridViewColumn.CellTemplate>
        // in the view makes this necessary.  Otherwise the SelectedItem is not shown on form load.
        ObservableCollection<IAccountLinkViewModel> AccountLinks { get; }
    }
}
