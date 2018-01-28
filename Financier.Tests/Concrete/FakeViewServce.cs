using Financier.Desktop.Services;
using Financier.Services;
using System.Collections.Generic;

namespace Financier.Tests.Concrete
{
    class FakeViewServce : IViewService
    {
        public bool OpenAccountCreateView(out Account account)
        {
            account = null;
            return false;
        }

        public bool OpenAccountEditView(int accountId, out Account account)
        {
            account = null;
            return false;
        }

        public void OpenAccountListView()
        {
            
        }

        public bool OpenAccountTransactionsEditView(int accountId)
        {
            return false;
        }

        public int OpenBudgetCreateView()
        {
            return 0;
        }

        public bool OpenBudgetDeleteConfirmationView()
        {
            return false;
        }

        public bool OpenBudgetEditView(int budgetId)
        {
            return false;
        }

        public void OpenBudgetListView()
        {
            
        }

        public bool OpenBudgetTransactionDeleteConfirmationView()
        {
            return false;
        }

        public void OpenMainView()
        {
            
        }

        public bool OpenPaydayEventStartView(int budgetId, out PaydayStart paydayStart)
        {
            paydayStart = null;
            return false;
        }

        public bool OpenTransactionBatchCreateConfirmView(IEnumerable<Transaction> transactions)
        {
            return false;
        }

        public bool OpenTransactionCreateView(Transaction hint, out Transaction transaction)
        {
            transaction = null;
            return false;
        }

        public bool OpenTransactionDeleteConfirmationView()
        {
            return false;
        }

        public bool OpenTransactionEditView(int transactionId)
        {
            return false;
        }

        public void OpenTransactionListView()
        {
            
        }
    }
}
