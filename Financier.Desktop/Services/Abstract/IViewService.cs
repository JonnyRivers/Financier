namespace Financier.Desktop.Services
{
    public interface IViewService
    {
        void OpenMainView();

        bool OpenAccountCreateView();
        bool OpenAccountEditView(int accountId);

        bool OpenBudgetCreateView();
        bool OpenBudgetEditView(int budgetId);

        bool OpenTransactionCreateView();
        bool OpenTransactionDeleteConfirmationView();
        bool OpenTransactionEditView(int transactionId);
    }
}
