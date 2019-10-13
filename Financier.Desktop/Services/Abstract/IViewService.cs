﻿using Financier.Services;
using System.Collections.Generic;

namespace Financier.Desktop.Services
{
    public interface IViewService
    {
        bool OpenAccountRelationshipCreateView(AccountRelationship hint, out AccountRelationship newAccountRelationship);
        bool OpenAccountRelationshipDeleteConfirmationView();
        bool OpenAccountRelationshipEditView(int accountRelationshipId, out AccountRelationship updatedAccountRelationship);

        bool OpenBudgetCreateView(out Budget newBudget);
        bool OpenBudgetDeleteConfirmationView();
        bool OpenBudgetEditView(int budgetId, out Budget updatedBudget);

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
    }
}
