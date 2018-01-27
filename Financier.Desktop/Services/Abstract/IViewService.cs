namespace Financier.Desktop.Services
{
    public interface IViewService
    {
        void OpenMainView();

        int OpenAccountCreateView();
        bool OpenAccountEditView(int accountId);

        int OpenBudgetCreateView();
        bool OpenBudgetDeleteConfirmationView();
        bool OpenBudgetEditView(int budgetId);

        bool OpenBudgetTransactionDeleteConfirmationView();

        int OpenTransactionCreateView();
        bool OpenTransactionDeleteConfirmationView();
        bool OpenTransactionEditView(int transactionId);
    }
}
