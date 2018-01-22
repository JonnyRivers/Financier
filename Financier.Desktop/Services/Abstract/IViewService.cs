namespace Financier.Desktop.Services
{
    public interface IViewService
    {
        void OpenMainView();

        bool OpenAccountCreateView();
        bool OpenAccountEditView(int accountId);

        bool OpenBudgetCreateView();
        bool OpenBudgetDeleteConfirmationView();
        bool OpenBudgetEditView(int budgetId);

        bool OpenBudgetTransactionDeleteConfirmationView();

        bool OpenTransactionCreateView();
        bool OpenTransactionDeleteConfirmationView();
        bool OpenTransactionEditView(int transactionId);
    }
}
