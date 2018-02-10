using Financier.Services;
using System.Collections.Generic;

namespace Financier.Desktop.Services
{
    public interface IViewService
    {
        void OpenMainView();

        bool OpenAccountCreateView(out Account account);
        bool OpenAccountEditView(int accountId, out Account account);
        void OpenAccountListView();
        bool OpenAccountTransactionsEditView(int accountId);

        bool OpenAccountRelationshipCreateView(AccountRelationship hint, out AccountRelationship transaction);
        bool OpenAccountRelationshipDeleteConfirmationView();
        bool OpenAccountRelationshipEditView(int transactionId);
        void OpenAccountRelationshipListView();

        bool OpenBudgetCreateView(out Budget budget);
        bool OpenBudgetDeleteConfirmationView();
        bool OpenBudgetEditView(int budgetId, out Budget budget);
        void OpenBudgetListView();

        bool OpenBudgetTransactionDeleteConfirmationView();

        bool OpenPaydayEventStartView(int budgetId, out PaydayStart paydayStart);

        void OpenNoPendingCreditCardTransactionsView(string accountName);
        bool OpenTransactionBatchCreateConfirmView(IEnumerable<Transaction> transactions);
        bool OpenTransactionCreateView(Transaction hint, out Transaction transaction);
        bool OpenTransactionDeleteConfirmationView();
        bool OpenTransactionEditView(int transactionId);
        void OpenTransactionListView();
    }
}
