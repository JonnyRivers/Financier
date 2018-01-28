﻿using Financier.Services;
using System.Collections.Generic;

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
        void OpenBudgetListView();

        bool OpenBudgetTransactionDeleteConfirmationView();

        bool OpenPaydayEventStartView(int budgetId, out PaydayStart paydayStart);

        bool OpenTransactionBatchCreateConfirmView(IEnumerable<Transaction> transactions);
        bool OpenTransactionCreateView(out Transaction transaction);
        bool OpenTransactionDeleteConfirmationView();
        bool OpenTransactionEditView(int transactionId);
        void OpenTransactionListView();
    }
}
