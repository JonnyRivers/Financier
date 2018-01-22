using Financier.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IBudgetEditViewModel
    {
        int BudgetId { get; set; }
        string Name { get; set; }
        BudgetPeriod SelectedPeriod { get; set; }
        IBudgetTransactionListViewModel TransactionListViewModel { get; }

        IEnumerable<BudgetPeriod> Periods { get; }

        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
