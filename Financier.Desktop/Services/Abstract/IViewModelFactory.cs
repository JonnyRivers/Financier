using Financier.Desktop.ViewModels;
using Financier.Services;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace Financier.Desktop.Services
{
    public interface IViewModelFactory
    {
        IMainWindowViewModel CreateMainWindowViewModel();
        
        IAccountEditViewModel CreateAccountEditViewModel(int accountId);
        IAccountItemViewModel CreateAccountItemViewModel(Account account);
        IAccountLinkViewModel CreateAccountLinkViewModel(AccountLink accountLink);
        IAccountListViewModel CreateAccountListViewModel();
        IAccountTransactionListViewModel CreateAccountTransactionListViewModel(int accountId);
        IAccountTransactionItemViewModel CreateAccountTransactionItemViewModel(Transaction transaction);

        IAccountRelationshipDetailsViewModel CreateAccountRelationshipCreateViewModel(AccountRelationship hint);
        IAccountRelationshipDetailsViewModel CreateAccountRelationshipEditViewModel(int accountRelationshipId);
        IAccountRelationshipItemViewModel CreateAccountRelationshipItemViewModel(AccountRelationship accountRelationship);
        IAccountRelationshipListViewModel CreateAccountRelationshipListViewModel();
        IAccountRelationshipTypeFilterViewModel CreateAccountRelationshipTypeFilterViewModel(AccountRelationshipType? type);

        IBudgetEditViewModel CreateBudgetEditViewModel(int budgetId);
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

        ITransactionEditViewModel CreateTransactionCreateViewModel(Transaction hint);
        IReconcileBalanceViewModel CreateReconcileBalanceViewModel(int accountId);
        ITransactionEditViewModel CreateTransactionEditViewModel(int transactionId);
        ITransactionItemViewModel CreateTransactionItemViewModel(Transaction transaction);
        ITransactionListViewModel CreateTransactionListViewModel();
    }
}
