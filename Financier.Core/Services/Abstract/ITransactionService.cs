using System;
using System.Collections.Generic;

namespace Financier.Services
{
    public interface ITransactionService
    {
        bool Any();
        void Create(Transaction transaction);
        void CreateMany(IEnumerable<Transaction> transactions);
        void Delete(int transactionId);
        Transaction Get(int transactionId);
        IEnumerable<Transaction> GetAll();
        IEnumerable<Transaction> GetAll(IEnumerable<int> accountIds);
        IEnumerable<Transaction> GetAll(IEnumerable<int> accountIds, DateTime startAt, DateTime endAt);
        IEnumerable<Payment> GetPendingCreditCardPayments(int accountId);
        void Update(Transaction transaction);
    }
}
