using Financier.Services;
using System.Collections.Generic;
using System.Windows.Input;

namespace Financier.Desktop.ViewModels
{
    public interface IBudgetDetailsViewModelFactory
    {
        IBudgetDetailsViewModel Create();
        IBudgetDetailsViewModel Create(int budgetId);
    }

    public interface IBudgetDetailsViewModel
    {
        Budget ToBudget();

        string Name { get; set; }
        BudgetPeriod SelectedPeriod { get; set; }
        IBudgetTransactionListViewModel TransactionListViewModel { get; }

        IEnumerable<BudgetPeriod> Periods { get; }

        ICommand OKCommand { get; }
        ICommand CancelCommand { get; }
    }
}
