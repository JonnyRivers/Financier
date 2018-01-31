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
        IAccountLinkViewModel CreateAccountLinkViewModel(AccountLink accountLink);
        IAccountListViewModel CreateAccountListViewModel();
        IAccountTransactionListViewModel CreateAccountTransactionListViewModel(int accountId);

        IBudgetEditViewModel CreateBudgetEditViewModel(int budgetId);
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
        ITransactionEditViewModel CreateTransactionEditViewModel(int transactionId);
        ITransactionListViewModel CreateTransactionListViewModel();
    }
}
