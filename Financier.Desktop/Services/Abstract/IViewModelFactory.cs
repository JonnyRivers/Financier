using Financier.Desktop.ViewModels;
using Financier.Services;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Financier.Desktop.Services
{
    public interface IViewModelFactory
    {
        IMainWindowViewModel CreateMainWindowViewModel();

        IAccountDetailsViewModel CreateAccountCreateViewModel();
        IAccountDetailsViewModel CreateAccountEditViewModel(int accountId);
        IAccountItemViewModel CreateAccountItemViewModel(Account account);
        IAccountLinkViewModel CreateAccountLinkViewModel(AccountLink accountLink);
        IAccountListViewModel CreateAccountListViewModel();
        IAccountTransactionListViewModel CreateAccountTransactionListViewModel(int accountId);
        IAccountTransactionItemViewModel CreateAccountTransactionItemViewModel(Transaction transaction);
        IAccountTreeViewModel CreateAccountTreeViewModel();
        IAccountTreeItemViewModel CreateAccountTreeItemViewModel(
            Account account, 
            IEnumerable<Transaction> transactions);
        IAccountTreeItemViewModel CreateAccountTreeItemViewModel(
            Account account, 
            IEnumerable<Transaction> transactions, 
            IEnumerable<IAccountTreeItemViewModel> childAccountVMs);

        IAccountRelationshipDetailsViewModel CreateAccountRelationshipCreateViewModel(AccountRelationship hint);
        IAccountRelationshipDetailsViewModel CreateAccountRelationshipEditViewModel(int accountRelationshipId);
        IAccountRelationshipItemViewModel CreateAccountRelationshipItemViewModel(AccountRelationship accountRelationship);
        IAccountRelationshipListViewModel CreateAccountRelationshipListViewModel();
        IAccountRelationshipTypeFilterViewModel CreateAccountRelationshipTypeFilterViewModel(AccountRelationshipType? type);

        IBalanceSheetViewModel CreateBalanceSheetViewModel();
        IBalanceSheetItemViewModel CreateBalanceSheetItemViewModel(BalanceSheetItem balanceSheetItem);

        IForeignAmountViewModel CreateForeignAmountViewModel(
            decimal nativeAmount,
            string nativeCurrencyCode,
            string foreignCurrencyCode);

        IBudgetDetailsViewModel CreateBudgetCreateViewModel();
        IBudgetDetailsViewModel CreateBudgetEditViewModel(int budgetId);
        IBudgetItemViewModel CreateBudgetItemViewModel(Budget budget, Currency primaryCurrency);
        IBudgetListViewModel CreateBudgetListViewModel();
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
        ITransactionListViewModel CreateTransactionListViewModel();

        IUnhandledExceptionViewModel CreateUnhandledExceptionViewModel(Exception ex);
    }
}
