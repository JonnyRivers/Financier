using Financier.Services;
using System;

namespace Financier.Desktop.Tests.Concrete
{
    internal class AlwaysCreateTransactionViewService : FakeViewService
    {
        private ITransactionService m_transactionService;
        private AccountLink m_creditAccount;
        private AccountLink m_debitAccount;
        private decimal m_amount;
        private DateTime m_at;

        internal AlwaysCreateTransactionViewService(ITransactionService transactionService, AccountLink creditAccount, AccountLink debitAccount, decimal amount, DateTime at)
        {
            m_transactionService = transactionService;
            m_creditAccount = creditAccount;
            m_debitAccount = debitAccount;
            m_amount = amount;
            m_at = at;
        }

        public override bool OpenTransactionCreateView(Transaction hint, out Transaction transaction)
        {
            transaction = new Transaction
            {
                CreditAccount = m_creditAccount,
                DebitAccount = m_debitAccount,
                Amount = m_amount,
                At = m_at
            };

            m_transactionService.Create(transaction);

            return true;
        }
    }
}
