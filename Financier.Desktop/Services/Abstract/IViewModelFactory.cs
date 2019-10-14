using Financier.Desktop.ViewModels;
using Financier.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Financier.Desktop.Services
{
    public interface IViewModelFactory
    {
        IBalanceSheetItemViewModel CreateBalanceSheetItemViewModel(BalanceSheetItem balanceSheetItem);

        IForeignAmountViewModel CreateForeignAmountViewModel(
            decimal nativeAmount,
            string nativeCurrencyCode,
            string foreignCurrencyCode);

        IBudgetDetailsViewModel CreateBudgetCreateViewModel();
        IBudgetDetailsViewModel CreateBudgetEditViewModel(int budgetId);
        IBudgetItemViewModel CreateBudgetItemViewModel(Budget budget, Currency primaryCurrency);
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
