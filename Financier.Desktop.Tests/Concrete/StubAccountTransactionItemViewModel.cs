using System;
using Financier.Desktop.ViewModels;
using Financier.Services;

namespace Financier.Desktop.Tests.Concrete
{
    internal class StubAccountTransactionItemViewModelFactory : IAccountTransactionItemViewModelFactory
    {
        public IAccountTransactionItemViewModel Create(Transaction transaction)
        {
            return new StubAccountTransactionItemViewModel(
                new StubAccountLinkViewModelFactory(),
                transaction
            );
        }
    }

    internal class StubAccountTransactionItemViewModel : IAccountTransactionItemViewModel
    {
        private Transaction m_transaction;
        decimal m_balance;
        IAccountLinkViewModel m_creditAccount;
        IAccountLinkViewModel m_debitAccount;

        internal StubAccountTransactionItemViewModel(
            IAccountLinkViewModelFactory viewModelFactory,
            Transaction transaction)
        {
            m_transaction = transaction;
            m_balance = 0;

            m_creditAccount = viewModelFactory.Create(m_transaction.CreditAccount);
            m_debitAccount = viewModelFactory.Create(m_transaction.DebitAccount);
        }

        public int TransactionId => m_transaction.TransactionId;

        public IAccountLinkViewModel CreditAccount => m_creditAccount;

        public IAccountLinkViewModel DebitAccount => m_debitAccount;

        public DateTime At => m_transaction.At;

        public decimal Amount => m_transaction.Amount;

        public decimal Balance { get => m_balance; set => m_balance = value; }
    }
}
