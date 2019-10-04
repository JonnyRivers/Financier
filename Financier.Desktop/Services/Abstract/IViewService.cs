using Financier.Services;
using System;
using System.Collections.Generic;

namespace Financier.Desktop.Services
{
    public interface IViewService
    {
        bool OpenAccountCreateView(out Account newAccount);
        bool OpenAccountEditView(int accountId, out Account updatedAccount);
        void OpenAccountListView();
        bool OpenAccountTransactionsEditView(int accountId);
        void OpenAccountTreeView();

        bool OpenAccountRelationshipCreateView(AccountRelationship hint, out AccountRelationship newAccountRelationship);
        bool OpenAccountRelationshipDeleteConfirmationView();
        bool OpenAccountRelationshipEditView(int accountRelationshipId, out AccountRelationship updatedAccountRelationship);
        void OpenAccountRelationshipListView();

        void OpenBalanceSheetView();

        bool OpenBudgetCreateView(out Budget newBudget);
        bool OpenBudgetDeleteConfirmationView();
        bool OpenBudgetEditView(int budgetId, out Budget updatedBudget);
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
        bool OpenTransactionCreateView(Transaction hint, out Transaction newTransaction);
        bool OpenReconcileBalanceView(int accountId, out Transaction newTransaction);
        bool OpenTransactionDeleteConfirmationView();
        bool OpenTransactionEditView(int transactionId, out Transaction updatedTransaction);
        void OpenTransactionListView();

        void OpenUnhandledExceptionView(Exception ex);
    }
}
