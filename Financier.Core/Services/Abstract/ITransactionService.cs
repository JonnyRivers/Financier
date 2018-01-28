using System.Collections.Generic;

namespace Financier.Services
{
    public interface ITransactionService
    {
        bool Any();
        void Create(Transaction transaction);
        void Delete(int transactionId);
        Transaction Get(int transactionId);
        IEnumerable<Transaction> GetAll();
        IEnumerable<Transaction> GetAll(IEnumerable<int> accountIds);// TODO: tests
        void Update(Transaction transaction);
    }
}
