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
        bool OpenAccountRelationshipEditView(int accountRelationshipId);
        void OpenAccountRelationshipListView();

        bool OpenBudgetCreateView(out Budget budget);
        bool OpenBudgetDeleteConfirmationView();
        bool OpenBudgetEditView(int budgetId, out Budget budget);
        void OpenBudgetListView();

        bool OpenBudgetTransactionDeleteConfirmationView();

        bool OpenForeignAmountView(
            decimal nativeAmount,
            string nativeCurrencyCode,
            string foreignCurrencyCode, 
            out decimal exchangedAmount);

        bool OpenPaydayEventStartView(int budgetId, out PaydayStart paydayStart);

        void OpenNoPendingCreditCardTransactionsView(string accountName);
        bool OpenTransactionBatchCreateConfirmView(IEnumerable<Transaction> transactions);
        bool OpenTransactionCreateView(Transaction hint, out Transaction transaction);
        bool OpenReconcileBalanceView(int accountId, out Transaction transaction);
        bool OpenTransactionDeleteConfirmationView();
        bool OpenTransactionEditView(int transactionId);
        void OpenTransactionListView();
    }
}
