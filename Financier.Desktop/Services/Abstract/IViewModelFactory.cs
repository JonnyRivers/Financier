using Financier.Desktop.ViewModels;
using Financier.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Financier.Desktop.Services
{
    public interface IViewModelFactory
    {
        //IAccountRelationshipDetailsViewModel CreateAccountRelationshipCreateViewModel(AccountRelationship hint);
        //IAccountRelationshipDetailsViewModel CreateAccountRelationshipEditViewModel(int accountRelationshipId);
        //IAccountRelationshipItemViewModel CreateAccountRelationshipItemViewModel(AccountRelationship accountRelationship);

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

        ITransactionDetailsViewModel CreateTransactionCreateViewModel(Transaction hint);
        IReconcileBalanceViewModel CreateReconcileBalanceViewModel(int accountId);
        ITransactionDetailsViewModel CreateTransactionEditViewModel(int transactionId);
        ITransactionItemViewModel CreateTransactionItemViewModel(Transaction transaction);
    }
}
