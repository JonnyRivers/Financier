using Financier.Desktop.ViewModels;
using Financier.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Financier.Desktop.Services
{
    public interface IViewModelFactory
    {
        IBudgetTransactionListViewModel CreateBudgetTransactionListViewModel(int budgetId);
        IBudgetTransactionItemViewModel CreateBudgetTransactionItemViewModel(
            ObservableCollection<IAccountLinkViewModel> accountLinks,
            BudgetTransaction budgetTransaction,
            BudgetTransactionType type);
        IPaydayEventStartViewModel CreatePaydayEventStartViewModel(int budgetId);
        ITransactionBatchCreateConfirmViewModel CreateTransactionBatchCreateConfirmViewModel(
            IEnumerable<Transaction> transactions);

        IReconcileBalanceViewModel CreateReconcileBalanceViewModel(int accountId);
    }
}
