using Financier.Desktop.Services;
using Financier.Services;
using System.Collections.Generic;

namespace Financier.Tests.Concrete
{
    internal class FakeViewService : IViewService
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

        public bool OpenAccountRelationshipCreateView(AccountRelationship hint, out AccountRelationship transaction)
        {
            throw new System.NotImplementedException();
        }

        public bool OpenAccountRelationshipDeleteConfirmationView()
        {
            throw new System.NotImplementedException();
        }

        public bool OpenAccountRelationshipEditView(int accountRelationshipId)
        {
            throw new System.NotImplementedException();
        }

        public void OpenAccountRelationshipListView()
        {
            
        }

        public bool OpenAccountTransactionsEditView(int accountId)
        {
            return false;
        }

        public bool OpenBudgetCreateView(out Budget budget)
        {
            budget = null;
            return false;
        }

        public bool OpenBudgetDeleteConfirmationView()
        {
            return false;
        }

        public bool OpenBudgetEditView(int budgetId, out Budget budget)
        {
            budget = null;
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

        public void OpenNoPendingCreditCardTransactionsView(string accountName)
        {
            
        }

        public bool OpenPaydayEventStartView(int budgetId, out PaydayStart paydayStart)
        {
            paydayStart = null;
            return false;
        }

        public bool OpenReconcileBalanceView(int accountId, out Transaction transaction)
        {
            throw new System.NotImplementedException();
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
